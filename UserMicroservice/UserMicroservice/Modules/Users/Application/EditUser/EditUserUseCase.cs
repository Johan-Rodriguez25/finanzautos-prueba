using UserMicroservice.Modules.Users.Domain;
using UserMicroservice.Modules.Users.Domain.Entities;
using UserMicroservice.Modules.Users.Domain.ValueObjects;

namespace UserMicroservice.Modules.Users.Application.EditUser
{
    public class EditUserUseCase
    {
        private readonly IUserRepository _userRepository;
        public EditUserUseCase(IUserRepository userRepository)
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

            await _userRepository.EditUser(user);
        }
    }
}
