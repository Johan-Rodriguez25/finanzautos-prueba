using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserMicroservice.Modules.Users.Domain.Entities;

namespace UserMicroservice.Modules.Users.Infrastructure.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly TokenBlacklistService _tokenBlacklistService;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            IConfiguration configuration,
            TokenBlacklistService tokenBlacklistService,
            ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _tokenBlacklistService = tokenBlacklistService;
            _logger = logger;
        }

        public string GenerateJwtToken(User user)
        {
            _logger.LogDebug("Starting token generation process for user ID: {userId}", user.Id.Value);
            try
            {
                var jwtKey = _configuration["Jwt:Key"];

                if (string.IsNullOrEmpty(jwtKey))
                {
                    throw new ArgumentNullException(nameof(jwtKey), "JWT key cannot be null or empty");
                }

                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

                var origIat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

                _logger.LogInformation("Generating token for user ID: {userId}", user.Id.Value);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("orig_iat", origIat)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                _logger.LogInformation("Token generation successful for user ID: {userId}", user.Id.Value);

                return tokenString;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error generating JWT token for user ID: {userId}", user.Id.Value);
                throw;
            }
        }

        public async Task<bool> ValidateToken(string token)
        {
            _logger.LogDebug("Validating token");
            try
            {
                if (_tokenBlacklistService.IsTokenBlacklisted(token))
                {
                    return false;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtKey = _configuration["Jwt:Key"];
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };

                // Validate token and return result
                await Task.Run(() =>
                    tokenHandler.ValidateToken(
                        token,
                        validationParameters,
                        out SecurityToken validatedToken)
                    );

                _logger.LogInformation("Token validation successful");

                return true;
            }
            catch
            {
                _logger.LogError("Token validation failed");
                return false;
            }
        }

        public (string newToken, bool success, string errorMessage) RefreshToken(string oldTokenString)
        {
            _logger.LogDebug("Refreshing token");
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var oldToken = tokenHandler.ReadJwtToken(oldTokenString);

                if (_tokenBlacklistService.IsTokenBlacklisted(oldTokenString))
                {
                    return (null, false, "Token has been revoked");
                }

                var origIatClaim = oldToken.Claims.FirstOrDefault(c => c.Type == "orig_iat");
                if (origIatClaim == null)
                {
                    return (null, false, "The token does not contain the original issuance time");
                }

                if (!long.TryParse(origIatClaim.Value, out long origIatunix))
                {
                    return (null, false, "The token contains an invalid time format");
                }

                var origIat = DateTimeOffset.FromUnixTimeSeconds(origIatunix).UtcDateTime;
                var refreshWindow = TimeSpan.FromHours(8);

                if (DateTime.UtcNow - origIat > refreshWindow)
                {
                    return (null, false, "The token can no longer be refreshed, the allowed period has been exceeded");
                }

                var claims = oldToken.Claims.ToList();
                var jtiClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);

                if (jtiClaim != null)
                {
                    claims.Remove(jtiClaim);
                }

                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                var jwtKey = _configuration["Jwt:Key"];
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var newExpiration = oldToken.ValidTo.AddMinutes(1);

                var newToken = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: newExpiration,
                    signingCredentials: credentials
                );

                _tokenBlacklistService.BlacklistToken(oldTokenString, oldToken.ValidTo);

                _logger.LogInformation("Token refresh successful");
                return (tokenHandler.WriteToken(newToken), true, null);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error refreshing token");
                return (null, false, e.Message);
            }
        }

        public bool RevokeToken(string token)
        {
            _logger.LogDebug("Revoking token");
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                if (_tokenBlacklistService.IsTokenBlacklisted(token))
                {
                    return true;
                }

                _tokenBlacklistService.BlacklistToken(token, jwtToken.ValidTo);
                _logger.LogInformation("Token revocation successful");

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error revoking token");
                return false;
            }
        }
    }
}