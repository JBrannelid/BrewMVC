namespace BrewMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpClient("BrewAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7136/api/");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}


// Referense page:
// https://jarsofdust.com/
// https://se.pinterest.com/pin/coffee-shop-landing-page-design-in-2024--356488126771592941/

// Color picker: 
// https://rgbcolorpicker.com/

// CSS
// https://getbootstrap.com/docs/5.3/getting-started/introduction/