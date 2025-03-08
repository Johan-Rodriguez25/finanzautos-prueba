using PublicationMicroservice.Modules.Publications.CreatePublication;
using PublicationMicroservice.Modules.Publications.DeletePublication;
using PublicationMicroservice.Modules.Publications.Domain.Entities;
using PublicationMicroservice.Modules.Publications.GetPublicationById;
using PublicationMicroservice.Modules.Publications.GetPublicationsByUserId;

namespace PublicationMicroservice.Modules.Publications.Infrastructure.Services
{
    public class PublicationService
    {
        private readonly CreatePublicationUseCase _createPublicationUseCase;
        private readonly GetPublicationsByUserIdUseCase _getPublicationsByUserIdUseCase;
        private readonly GetPublicationByIdUseCase _getPublicationByIdUseCase;
        private readonly DeletePublicationUseCase _deletePublicationUseCase;
        private readonly ILogger<PublicationService> _logger;

        public PublicationService(
            CreatePublicationUseCase createPublicationUseCase,
            GetPublicationsByUserIdUseCase getPublicationsByUserIdUseCase,
            GetPublicationByIdUseCase getPublicationByIdUseCase,
            DeletePublicationUseCase deletePublicationUseCase,
            ILogger<PublicationService> logger
            )
            {
                _createPublicationUseCase = createPublicationUseCase;
                _getPublicationsByUserIdUseCase = getPublicationsByUserIdUseCase;
                _getPublicationByIdUseCase = getPublicationByIdUseCase;
                _deletePublicationUseCase = deletePublicationUseCase;
                _logger = logger;
            }

        public async Task CreatePublication(Publication request)
        {
            _logger.LogDebug("Creating publication");
            try
            {
                await _createPublicationUseCase.Run(
                    request.Id.Value,
                    request.Title.Value,
                    request.Content.Value,
                    request.UserId,
                    request.CreatedAt.Value,
                    request.UpdatedAt.Value
                );

                _logger.LogInformation("Publication created");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating publication");
                throw;
            }
        }

        public async Task<IEnumerable<Publication>> GetPublicationsByUserId(string userId)
        {
            _logger.LogDebug($"Getting publications for user: {userId}");
            try
            {
                var publications = await _getPublicationsByUserIdUseCase.Run(userId);
                _logger.LogInformation($"Retrieved publications for user: {userId}");

                return publications;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error getting publications for user: {userId}");
                throw;
            }
        }

        public async Task<Publication?> GetPublicationById(string id)
        {
            _logger.LogDebug("Getting publication by id");
            try
            {
                return await _getPublicationByIdUseCase.Run(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting publication by id");
                throw;
            }
        }

        public async Task DeletePublication(string id)
        {
            _logger.LogDebug("Deleting publication");
            try
            {
                await _deletePublicationUseCase.Run(id);
                _logger.LogInformation("Publication deleted");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting publication");
                throw;
            }
        }
    }
}