using PublicationMicroservice.Modules.Publications.Domain;
using PublicationMicroservice.Modules.Publications.Domain.Entities;
using PublicationMicroservice.Modules.Publications.Domain.ValueObjects;

namespace PublicationMicroservice.Modules.Publications.CreatePublication
{
    public class CreatePublicationUseCase
    {
        private readonly IPublicationRepository _publicationRepository;

        public CreatePublicationUseCase(IPublicationRepository publicationRepository)
        {
            _publicationRepository = publicationRepository;
        }

        public async Task Run(
            string id,
            string title,
            string content,
            string userId,
            DateTime createdAt,
            DateTime updatedAt
            )
        {
            var publication = new Publication(
                PublicationId.FromString(id),
                PublicationTitle.FromString(title),
                PublicationContent.FromString(content),
                userId,
                CreatedAt.FromDateTime(createdAt),
                UpdatedAt.FromDateTime(updatedAt)
                );

            await _publicationRepository.CreatePublication(publication);
        }
    }
}