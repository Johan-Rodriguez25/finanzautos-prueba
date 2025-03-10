namespace PublicationMicroservice.Modules.Publications.Infrastructure.Dtos.Response
{
    public class PublicationResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}