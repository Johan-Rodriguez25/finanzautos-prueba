using PublicationMicroservice.Modules.Publications.Domain;
using PublicationMicroservice.Modules.Publications.Domain.ValueObjects;

namespace PublicationMicroservice.Modules.Publications.DeletePublication
{
    public class DeletePublicationUseCase
    {
        private readonly IPublicationRepository _publicationRepository;

        public DeletePublicationUseCase(IPublicationRepository publicationRepository)
        {
            _publicationRepository = publicationRepository;
        }

        public async Task Run(string id)
        {
            await _publicationRepository.DeletePublication(PublicationId.FromString(id));
        }
    }
}
