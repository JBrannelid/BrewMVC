// DelegateHandler to add JWT token from cookie to outgoing HTTP requests
// DelegateHandler works as middleware for HttpClient-levels/requests
// This way we don't have to manually add the token in each controller method
// ServiceExtensions.cs configures this handler for HttpClient "BrewAPI"

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;

namespace BrewMVC.Extensions
{
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Get the current HTTP context
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                // Try to get JWT token from cookie
                if (httpContext.Request.Cookies.TryGetValue("jwtToken", out var jwtToken)
                    && !string.IsNullOrEmpty(jwtToken))
                {
                    // Add Authorization header with Bearer token to the outgoing request
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
                }
            }

            // Continue with the request
            var response = await base.SendAsync(request, cancellationToken);

            // Handle 401 Unauthorized responses from API
            if (response.StatusCode == HttpStatusCode.Unauthorized && httpContext != null)
            {
                if (httpContext.User?.Identity?.IsAuthenticated == true)
                {
                    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    httpContext.Response.Cookies.Delete("jwtToken");
                }
            }

            return response;
        }
    }
}
