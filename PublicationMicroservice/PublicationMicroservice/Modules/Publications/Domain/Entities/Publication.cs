using PublicationMicroservice.Modules.Publications.Domain.ValueObjects;

namespace PublicationMicroservice.Modules.Publications.Domain.Entities
{
    public class Publication
    {
        public PublicationId Id { get; private set; }
        public PublicationTitle Title { get; private set; }
        public PublicationContent Content { get; private set; }
        public string UserId { get; private set; }
        public CreatedAt CreatedAt { get; private set; }
        public UpdatedAt UpdatedAt { get; private set; }

        public Publication(PublicationId id, PublicationTitle title, PublicationContent content, string userId, CreatedAt createdAt, UpdatedAt updatedAt)
        {
            Id = id;
            Title = title;
            Content = content;
            UserId = userId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        protected Publication() { }
    }
}
