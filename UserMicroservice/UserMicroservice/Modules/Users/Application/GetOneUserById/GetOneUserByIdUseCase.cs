using UserMicroservice.Modules.Users.Domain.Entities;
using UserMicroservice.Modules.Users.Domain;
using UserMicroservice.Modules.Users.Domain.ValueObjects;

namespace UserMicroservice.Modules.Users.Application.GetOneUserById
{
    public class GetOneUserByIdUseCase
    {
        private readonly IUserRepository _userRepository;
        public GetOneUserByIdUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> Run(string id)
        {
            return await _userRepository.GetOneUserById(UserId.FromString(id));
        }
    }
}
