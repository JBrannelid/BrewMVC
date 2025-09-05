using Microsoft.AspNetCore.Authentication.Cookies;

namespace BrewMVC.Extensions
{
    public static class ServiceCollectionExtensions
    {
        // Add MVC Controllers with Views
        public static IServiceCollection AddMvcServices(this IServiceCollection services)
        {
            services.AddControllersWithViews();
            return services;
        }

        // Configure HTTP Clients for API communication
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("BrewAPI", client =>
            {
                var apiBaseUrl = configuration["ApiSettings:BaseUrl"];
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            return services;
        }

        // Configure cookie-based authentication and authorization
        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services)
        {
            // Configure cookie-based authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login"; // Redirect to this path if not authenticated
                    options.LogoutPath = "/Auth/Logout"; // Redirect to this path on logout
                    options.AccessDeniedPath = "/Auth/AccessDenied"; // Redirect when access is denied
                    options.ExpireTimeSpan = TimeSpan.FromHours(1); // Cookie expiration time
                    options.SlidingExpiration = true; // Renew the cookie on each request
                    options.Cookie.HttpOnly = true; // Security: prevent client-side access
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Require HTTPS
                    options.Cookie.SameSite = SameSiteMode.Strict; // CSRF protection
                });

            services.AddAuthorization(options =>
            {
                // FIXAD POLICY - använder claims istället för roller för att fungera med JWT
                options.AddPolicy("AdminOrManager", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("role", "Admin") ||
                        context.User.HasClaim("role", "Manager")));
            });

            return services;
        }
    }
}