using UserMicroservice.Modules.Users.Application.EditUser;
using UserMicroservice.Modules.Users.Application.GetOneUserById;
using UserMicroservice.Modules.Users.Application.Login;
using UserMicroservice.Modules.Users.Application.Register;
using UserMicroservice.Modules.Users.Domain.Entities;

namespace UserMicroservice.Modules.Users.Infrastructure.Services
{
    public class UserService
    {
        private readonly RegisterUseCase _registerUseCase;
        private readonly GetOneUserByIdUseCase _getOneUserByIdUseCase;
        private readonly EditUserUseCase _editUserUseCase;
        private readonly LoginUseCase _loginUseCase;
        private readonly ILogger<UserService> _logger;

        public UserService(
            RegisterUseCase registerUseCase,
            GetOneUserByIdUseCase getOneUserByIdUseCase,
            EditUserUseCase editUserUseCase,
            LoginUseCase loginUseCase,
            ILogger<UserService> logger
            )
        {
            _registerUseCase = registerUseCase;
            _getOneUserByIdUseCase = getOneUserByIdUseCase;
            _editUserUseCase = editUserUseCase;
            _loginUseCase = loginUseCase;
            _logger = logger;
        }

        public async Task Register(User request)
        {
            _logger.LogDebug("Starting user registration process for {Email}", request.Email.Value);
            try
            {
                var plainPassword = request.Password.Value;
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

                await _registerUseCase.Run(
                    request.Id.Value,
                    request.Name.Value,
                    request.Email.Value,
                    hashedPassword,
                    request.CreatedAt.Value
                );

                _logger.LogInformation("Successfully registered user with ID: {UserId}", request.Id.Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to register user with email: {Email}", request.Email.Value);
                throw;
            }
        }

        public async Task<User?> Login(string email, string plainPassword)
        {
            _logger.LogDebug("Starting user login process for {Email}", email);
            try
            {
                return await _loginUseCase.Run(email, plainPassword);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to login user with email: {Email}", email);
                throw;
            }
        }

        public async Task<User?> GetOneUserById(string id)
        {
            _logger.LogDebug("Starting user retrieval process for ID: {id}", id);
            try
            {
                return await _getOneUserByIdUseCase.Run(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to retrieve user with ID: {id}", id);
                throw;
            }
        }

        public async Task EditUser(User request)
        {
            _logger.LogDebug("Starting user update process for ID: {id}", request.Id.Value);
            try
            {
                string hashedPassword = request.Password.Value;

                if (!string.IsNullOrEmpty(hashedPassword) &&
                    !hashedPassword.StartsWith("$2a$") &&
                    !hashedPassword.StartsWith("$2b$") &&
                    !hashedPassword.StartsWith("$2y$")
                    )
                {
                    hashedPassword = BCrypt.Net.BCrypt.HashPassword(hashedPassword);
                }

                await _editUserUseCase.Run(
                    request.Id.Value,
                    request.Name.Value,
                    request.Email.Value,
                    hashedPassword,
                    request.CreatedAt.Value
                );
                _logger.LogInformation("Successfully updated user with ID: {id}", request.Id.Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update user with ID: {id}", request.Id.Value);
                throw;
            }
        }
    }
}
