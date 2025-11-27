using Mos3ef.DAL.Repository.AuthRepository;

namespace Mos3ef.Api.Middleware
{
    public class TokenRevocationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenRevocationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthRepository authRepo)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Replace("Bearer ", "");

                if (await authRepo.IsTokenRevokedAsync(token))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token has been revoked.");
                    return;
                }
            }

            await _next(context);
        }
    }

}
