namespace UserMicroservice.Modules.Users.Infrastructure.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetJwtTokenFromHeader(this HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            return authHeader.Substring("Bearer ".Length).Trim();
        }
    }
}