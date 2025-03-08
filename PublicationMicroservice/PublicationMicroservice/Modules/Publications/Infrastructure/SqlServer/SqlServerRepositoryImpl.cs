using Microsoft.EntityFrameworkCore;
using PublicationMicroservice.Modules.Publications.Domain;
using PublicationMicroservice.Modules.Publications.Domain.Entities;
using PublicationMicroservice.Modules.Publications.Domain.ValueObjects;
using PublicationMicroservice.Modules.Publications.Infrastructure.SqlServer.Mappers;

namespace PublicationMicroservice.Modules.Publications.Infrastructure.SqlServer
{
    public class SqlServerRepositoryImpl : IPublicationRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SqlServerRepositoryImpl> _logger;

        public SqlServerRepositoryImpl(AppDbContext context, ILogger<SqlServerRepositoryImpl> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CreatePublication(Publication publication)
        {
            _logger.LogInformation($"Creating publication with id: {publication.Id}");
            try
            {
                var entity = PublicationMapper.MapToEntity(publication);

                await _context.Publications.AddAsync(entity);
                await _context.SaveChangesAsync();

                _logger.LogDebug("Publication created");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while creating publication with id: {publication.Id}");
                throw;
            }
        }

        public async Task<IEnumerable<Publication>> GetPublicationsByUserId(string userId)
        {
            _logger.LogInformation($"Getting publications for user: {userId}");
            try
            {
                var entities = await _context.Publications
                    .Where(p => p.UserId == userId)
                    .ToListAsync();

                _logger.LogDebug($"Found {entities.Count} publications for user: {userId}");

                return entities.Select(PublicationMapper.MapToDomain);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while getting publications for user: {userId}");
                throw;
            }
        }

        public async Task<Publication?> GetPublicationById(PublicationId id)
        {
            _logger.LogInformation($"Getting publication with id: {id.Value}");
            try
            {
                var entity = await _context.Publications
                    .FirstOrDefaultAsync(p => p.Id == id.Value);

                if (entity == null)
                {
                    _logger.LogDebug($"Publication with id: {id.Value} not found");
                    return null;
                }

                _logger.LogDebug($"Publication with id {id.Value} found");

                return PublicationMapper.MapToDomain(entity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while getting publication with id: {id.Value}");
                throw;
            }
        }

        public async Task DeletePublication(PublicationId id)
        {
            _logger.LogInformation($"Deleting publication with id: {id.Value}");
            try
            {
                var entity = await _context.Publications
                    .FirstOrDefaultAsync(p => p.Id == id.Value);

                if (entity == null)
                {
                    _logger.LogWarning($"Publication with id {id.Value} not found for deletion");
                    return;
                }

                _context.Publications.Remove(entity);
                await _context.SaveChangesAsync();

                _logger.LogDebug($"Publication with id {id.Value} deleted");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while deleting publication with id: {id.Value}");
                throw;
            }
        }
    }
}