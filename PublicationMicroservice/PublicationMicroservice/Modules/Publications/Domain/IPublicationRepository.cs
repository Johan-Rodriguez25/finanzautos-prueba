using PublicationMicroservice.Modules.Publications.Domain.Entities;
using PublicationMicroservice.Modules.Publications.Domain.ValueObjects;

namespace PublicationMicroservice.Modules.Publications.Domain
{
    public interface IPublicationRepository
    {
        Task CreatePublication(Publication publication);

        Task<IEnumerable<Publication>> GetPublicationsByUserId(string userId);

        Task<Publication?> GetPublicationById(PublicationId id);

        Task DeletePublication(PublicationId id);
    }
}