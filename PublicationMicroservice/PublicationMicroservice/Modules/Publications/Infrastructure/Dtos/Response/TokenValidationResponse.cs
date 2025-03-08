namespace PublicationMicroservice.Modules.Publications.Infrastructure.Dtos.Response
{
    public class TokenValidationResponse
    {
        public bool IsValid { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}