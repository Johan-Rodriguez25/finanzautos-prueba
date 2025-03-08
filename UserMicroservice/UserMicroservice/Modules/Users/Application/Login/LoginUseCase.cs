using UserMicroservice.Modules.Users.Domain;
using UserMicroservice.Modules.Users.Domain.Entities;
using UserMicroservice.Modules.Users.Domain.ValueObjects;

namespace UserMicroservice.Modules.Users.Application.Login
{
    public class LoginUseCase
    {
        private readonly IUserRepository _userRepository;

        public LoginUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> Run(string email, string password)
        {
            return await _userRepository.Login(UserEmail.FromString(email), password);
        }
    }
}
