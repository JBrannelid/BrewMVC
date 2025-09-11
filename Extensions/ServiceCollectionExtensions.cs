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
            // Register IHttpContextAccessor (needed by JwtAuthorizationHandler) 
            services.AddHttpContextAccessor();

            // Register the JwtAuthorizationHandler as a transient service
            services.AddTransient<JwtAuthorizationHandler>();

            // Configure HttpClient with JWT authorization handler
            services.AddHttpClient("BrewAPI", client =>
            {
                var apiBaseUrl = configuration["ApiSettings:BaseUrl"];
                client.BaseAddress = new Uri(apiBaseUrl);
            })
            .AddHttpMessageHandler<JwtAuthorizationHandler>(); // Add the JWT handler to the HTTP client pipeline

            return services;
        }


        // Configure cookie-based authentication and authorization
        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services)
        {
            // Configure cookie-based authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login"; // Redirect to Login if not authenticated
                    options.LogoutPath = "/Auth/Logout"; // Redirect to logout when logging out
                    options.AccessDeniedPath = "/Auth/AccessDenied"; // Redirect when access is denied
                    options.ExpireTimeSpan = TimeSpan.FromHours(1); // Cookie expiration time 1h
                    options.Cookie.HttpOnly = true; // Security: prevent client-side access
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Require HTTPS
                    options.Cookie.SameSite = SameSiteMode.Strict; // CSRF protection
                });

            services.AddAuthorization(options =>
            {
                // Use Claims-based policies
                options.AddPolicy("AdminOrManager", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("role", "Admin") ||
                        context.User.HasClaim("role", "Manager")));
            });

            return services;
        }

        // Add JWT token validation middleware
        public static IServiceCollection AddJwtTokenValidation(this IServiceCollection services)
        {
            services.AddTransient<BrewMVC.Middleware.JwtTokenValidationMiddleware>();

            return services;
        }
    }
}