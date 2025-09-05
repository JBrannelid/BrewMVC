using BrewMVC.Models;
using BrewMVC.ViewModel.MenuItems;
using Microsoft.AspNetCore.Mvc;

namespace BrewMVC.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly HttpClient _client;

        public MenuItemController(IHttpClientFactory clientFactory) =>
            _client = clientFactory.CreateClient("BrewAPI");

        public async Task<IActionResult> Index()
        {
            var menuItems = await _client.GetFromJsonAsync<List<MenuItemModel>>("MenuItems");

            var categoryMenuItem = new CategoryMenuItemVM
            {
                MenuByCategory = menuItems?
                    .GroupBy(item => item.Category ?? "Other")
                    .ToDictionary(g => g.Key, g => g.ToList())
                    // FailSafe: Return an empty dictionary if no items are found
                    ?? new Dictionary<string, List<MenuItemModel>>()
            };

            return View(categoryMenuItem);
        }
    }
}
