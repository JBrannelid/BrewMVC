using BrewMVC.Models;
using BrewMVC.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BrewMVC.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly HttpClient _client;

        public MenuItemController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("BrewAPI");
        }
        public async Task<IActionResult> Index()
        {
            var response = await _client.GetAsync("MenuItems");
            var menuItems = await response.Content.ReadFromJsonAsync<List<Models.MenuItems>>();
            var categoryMenuItemVM = new CategoryMenuItemVM();

            // Group menu items by category
            categoryMenuItemVM.MenuByCategory = menuItems
                .GroupBy(item => item.Category ?? "Other")
                .ToDictionary(g => g.Key, g => g.ToList());

            return View(categoryMenuItemVM);
        }
    }
}
