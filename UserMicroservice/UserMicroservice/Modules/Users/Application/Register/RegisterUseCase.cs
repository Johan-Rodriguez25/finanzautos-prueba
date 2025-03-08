using UserMicroservice.Modules.Users.Domain;
using UserMicroservice.Modules.Users.Domain.Entities;
using UserMicroservice.Modules.Users.Domain.ValueObjects;

namespace UserMicroservice.Modules.Users.Application.Register
{
    public class RegisterUseCase
    {
        private readonly IUserRepository _userRepository;

        public RegisterUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Run(
            string id,
            string name,
            string email,
            string passwordHash,
            DateTime createdAt
            )
        {
            var user = new User(
                UserId.FromString(id),
                UserName.FromString(name),
                UserEmail.FromString(email),
                PasswordHash.FromString(passwordHash),
                CreatedAt.FromDateTime(createdAt)
                );

            await _userRepository.Register(user);
        }
    }
}
