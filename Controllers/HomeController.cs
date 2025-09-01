using BrewMVC.Models;
using BrewMVC.ViewModels;
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
            var response = await _client.GetAsync("MenuItems");
            var menuItems = await response.Content.ReadFromJsonAsync<List<Models.MenuItems>>();
            var vm = menuItems
                .Where(m => m.IsPopular)
                .Select(m => new PopularMenuItemVM
                {
                    Description = m.Description,
                    ImageUrl = m.ImageUrl
                }).ToList();

            return View(vm);
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
