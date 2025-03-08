using Microsoft.EntityFrameworkCore;
using UserMicroservice.Modules.Users.Domain;
using UserMicroservice.Modules.Users.Domain.Entities;
using UserMicroservice.Modules.Users.Domain.ValueObjects;
using UserMicroservice.Modules.Users.Infrastructure.SqlServer.Mappers;

namespace UserMicroservice.Modules.Users.Infrastructure.SqlServer
{
    public class SqlServerRepositoryImpl : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SqlServerRepositoryImpl> _logger;

        public SqlServerRepositoryImpl(AppDbContext context, ILogger<SqlServerRepositoryImpl> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Register(User user)
        {
            _logger.LogInformation("Executing database insert for user: {UserId}", user.Id.Value);
            try
            {
                var entity = UserMapper.MapToEntity(user);

                await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogDebug("Successfully inserted user {UserId} into database", user.Id.Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Database error while inserting user {UserId}", user.Id.Value);
                throw;
            }
        }

        public async Task<User?> Login(UserEmail email, string plainPassword)
        {
            _logger.LogInformation("Executing database query for user: {UserEmail}", email.Value);
            try
            {
                var entity = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email.Value));

                if (entity == null)
                {
                    return null;
                }

                bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, entity.Password);

                return isValid ? UserMapper.MapToDomain(entity) : null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Database error while logging in user {UserEmail}", email.Value);
                throw;
            }
        }

        public async Task<User?> GetOneUserById(UserId id)
        {
            _logger.LogInformation("Executing database query for user: {UserId}", id.Value);
            try
            {
                var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id.Value));

                return entity == null ? null : UserMapper.MapToDomain(entity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Database error while querying user {UserId}", id.Value);
                throw;
            }
        }

        public async Task EditUser(User user)
        {
            _logger.LogInformation("Executing database query for user: {UserId}", user.Id.Value);
            try
            {
                var existingEntity = await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(user.Id.Value));
                if (existingEntity != null)
                {
                    // Update the existing entity's properties
                    existingEntity.Name = user.Name.Value;
                    existingEntity.Email = user.Email.Value;
                    existingEntity.Password = user.Password.Value;

                    await _context.SaveChangesAsync();
                    _logger.LogDebug("Successfully edited user {UserId} in database", user.Id.Value);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Database error while editing user {UserId}", user.Id.Value);
                throw;
            }
        }
    }
}
