using System.Collections.Concurrent;

namespace UserMicroservice.Modules.Users.Infrastructure.Services
{
  public class TokenBlacklistService
  {
    private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new ConcurrentDictionary<string, DateTime>();
    private readonly ILogger<TokenBlacklistService> _logger;

    public TokenBlacklistService(ILogger<TokenBlacklistService> logger)
    {
      _logger = logger;
    }

    public void BlacklistToken(string token, DateTime expirationTime)
    {
      _logger.LogInformation($"Blacklisting token: {token}");
      _blacklistedTokens.TryAdd(token, expirationTime);
    }

    public bool IsTokenBlacklisted(string token)
    {
      _logger.LogInformation($"Checking if token is blacklisted: {token}");
      return _blacklistedTokens.ContainsKey(token);
    }

    public void CleanupExpiredTokens()
    {
      _logger.LogInformation("Cleaning up expired tokens");
      var now = DateTime.UtcNow;

      foreach (var token in _blacklistedTokens.Keys)
      {
        if (_blacklistedTokens.TryGetValue(token, out DateTime expirationTime) && expirationTime < now)
        {
          _logger.LogInformation($"Removing expired token: {token}");
          _blacklistedTokens.TryRemove(token, out _);
        }
      }
    }
  }
}