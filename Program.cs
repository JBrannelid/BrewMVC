using BrewMVC.Extensions;

namespace BrewMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services using extension methods
            builder.Services.AddMvcServices();
            builder.Services.AddHttpClients(builder.Configuration);
            builder.Services.AddAuthenticationAndAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Enable Authentication & Authorization
            app.UseAuthentication();
            app.UseMiddleware<BrewMVC.Middleware.JwtTokenValidationMiddleware>();
            app.UseAuthorization();

            // Configure routing
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Run the application
            app.Run();
        }
    }
}