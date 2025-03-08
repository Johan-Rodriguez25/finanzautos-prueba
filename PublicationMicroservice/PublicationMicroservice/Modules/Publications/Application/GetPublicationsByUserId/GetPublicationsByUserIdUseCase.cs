using PublicationMicroservice.Modules.Publications.Domain;
using PublicationMicroservice.Modules.Publications.Domain.Entities;

namespace PublicationMicroservice.Modules.Publications.GetPublicationsByUserId
{
    public class GetPublicationsByUserIdUseCase
    {
        private readonly IPublicationRepository _publicationRepository;

        public GetPublicationsByUserIdUseCase(IPublicationRepository publicationRepository)
        {
            _publicationRepository = publicationRepository;
        }

        public async Task<IEnumerable<Publication>> Run(string userId)
        {
            return await _publicationRepository.GetPublicationsByUserId(userId);
        }
    }
}