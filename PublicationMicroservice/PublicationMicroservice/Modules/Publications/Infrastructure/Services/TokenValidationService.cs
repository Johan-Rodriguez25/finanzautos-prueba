using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using PublicationMicroservice.Modules.Publications.Infrastructure.Dtos.Response;

namespace PublicationMicroservice.Modules.Publications.Infrastructure.Services
{
    public class TokenValidationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenValidationService> _logger;

        public TokenValidationService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<TokenValidationService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<TokenValidationResponse> ValidateToken(string token)
        {
            try
            {
                _logger.LogInformation("Validating token");

                var userId = ExtractUserIdFromToken(token);
                if (!string.IsNullOrEmpty(userId))
                {
                    _logger.LogInformation($"Successfully extracted userId from token: {userId}");
                    return new TokenValidationResponse { IsValid = true, UserId = userId };
                }

                _logger.LogInformation("Local extraction failed, trying external validation service");
                var validationEndpoint = _configuration["ExternalServices:TokenValidation:Endpoint"];

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.PostAsync(validationEndpoint, null);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Token validation response: {jsonString}");

                    try
                    {
                        var jsonDoc = JsonDocument.Parse(jsonString);
                        if (jsonDoc.RootElement.TryGetProperty("sub", out var subElement))
                        {
                            userId = subElement.GetString();
                            _logger.LogInformation($"Extracted userId from external service: {userId}");

                            return new TokenValidationResponse
                            {
                                IsValid = true,
                                UserId = userId ?? string.Empty
                            };
                        }
                        else
                        {
                            _logger.LogWarning("Sub claim not found in token response");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error extracting sub claim from token response");
                    }
                }

                _logger.LogWarning($"Token validation failed. Status code: {response.StatusCode}");
                return new TokenValidationResponse { IsValid = false };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return new TokenValidationResponse { IsValid = false };
            }
        }

        private string ExtractUserIdFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    var subClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub");

                    if (subClaim != null)
                    {
                        _logger.LogInformation($"Found sub claim in token: {subClaim.Value}");
                        return subClaim.Value;
                    }
                    else
                    {
                        _logger.LogWarning("No sub claim found in token");
                    }
                }
                else
                {
                    _logger.LogWarning("Token is not a valid JWT token");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting user ID from token");
            }

            return string.Empty;
        }
    }
}