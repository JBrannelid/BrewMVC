using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;

namespace BrewMVC.Middleware
{
    public class JwtTokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtTokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (IsAuthenticated(context))
            {
                var jwtToken = GetTokenFromCookie(context);

                if (!string.IsNullOrEmpty(jwtToken))
                {
                    if (HasTokenExpired(jwtToken))
                    {
                        await SignOutAndRedirect(context, sessionExpired: true);
                        return;
                    }
                }
                else
                {
                    await SignOutAndRedirect(context);
                    return;
                }
            }

            await _next(context);
        }

        private bool IsAuthenticated(HttpContext context)
        {
            return context.User?.Identity?.IsAuthenticated == true;
        }

        private string? GetTokenFromCookie(HttpContext context)
        {
            context.Request.Cookies.TryGetValue("jwtToken", out var token);
            return token;
        }

        private bool HasTokenExpired(string jwtToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);
                return token.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true; 
            }
        }

        private async Task SignOutAndRedirect(HttpContext context, bool sessionExpired = false)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.Response.Cookies.Delete("jwtToken");

            if (sessionExpired)
            {
                context.Response.Cookies.Append("SessionExpired", "true", new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(1)
                });
            }

            if (!context.Response.HasStarted)
            {
                context.Response.Redirect("/");
            }
        }
    }
}
