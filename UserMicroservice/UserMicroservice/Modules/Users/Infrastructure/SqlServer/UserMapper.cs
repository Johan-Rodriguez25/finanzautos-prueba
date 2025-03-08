using UserMicroservice.Modules.Users.Domain.Entities;
using UserMicroservice.Modules.Users.Domain.ValueObjects;
using UserMicroservice.Modules.Users.Infrastructure.SqlServer.Entities;

namespace UserMicroservice.Modules.Users.Infrastructure.SqlServer.Mappers
{
    public static class UserMapper
    {
        public static User MapToDomain(SqlServerUserEntity entity)
        {
            return new User(
                UserId.FromString(entity.Id),
                UserName.FromString(entity.Name),
                UserEmail.FromString(entity.Email),
                PasswordHash.FromString(entity.Password),
                CreatedAt.FromDateTime(entity.CreatedAt)
            );
        }

        public static SqlServerUserEntity MapToEntity(User user)
        {
            return new SqlServerUserEntity
            {
                Id = user.Id.Value,
                Name = user.Name.Value,
                Email = user.Email.Value,
                Password = user.Password.Value,
                CreatedAt = user.CreatedAt.Value
            };
        }
    }
}
