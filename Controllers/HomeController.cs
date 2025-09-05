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
            var menuItems = await _client.GetFromJsonAsync<List<MenuItemModel>>("MenuItems") 
                    ?? new List<MenuItemModel>();

            var popularMenuItems = menuItems
                .Where(m => m.IsPopular)
                .Select(m => new PopularMenuItemVM
                {
                    Description = m.Description,
                    ImageUrl = m.ImageUrl
                })
                .ToList();

            return View(popularMenuItems);
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
