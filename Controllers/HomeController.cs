using BrewMVC.Models;
using BrewMVC.ViewModel.MenuItems;
using BrewMVC.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BrewMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _client;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("BrewAPI");
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                // Use the dedicated popular endpoint for better performance
                var popularMenuItems = await _client.GetFromJsonAsync<List<PopularMenuItemVM>>("MenuItems/popular")
                        ?? new List<PopularMenuItemVM>();

                return View(popularMenuItems);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(key: "", $"An error occurred\"{ex.Message}");
                return View(new List<PopularMenuItemVM>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
