using UserMicroservice.Modules.Users.Domain.Entities;
using UserMicroservice.Modules.Users.Domain.ValueObjects;

namespace UserMicroservice.Modules.Users.Domain
{
    public interface IUserRepository
    {
        Task Register(User user);
        Task<User?> Login(UserEmail email, string password);
        Task<User?> GetOneUserById(UserId id);
        Task EditUser(User user);
    }
}
