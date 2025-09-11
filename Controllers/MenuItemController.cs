using BrewMVC.Models;
using BrewMVC.ViewModel.MenuItems;
using Microsoft.AspNetCore.Mvc;

// Error handling: Helper methods for TempData.
// ModelState is lost on redirect but TempData persists for one http reques
// Same View() errors: ModelState since we return View()
// Redirect errors: TempData since we return RedirectToAction()

namespace BrewMVC.Controllers
{
    public class MenuItemController : BaseController
    {
        private readonly HttpClient _client;

        public MenuItemController(IHttpClientFactory clientFactory) =>
            _client = clientFactory.CreateClient("BrewAPI");

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("MenuItems");
                if (!response.IsSuccessStatusCode)
                {
                    HandleApiError(response, "Failed to load menu items");
                    return View(new CategoryMenuItemVM());
                }

                var menuItems = await response.Content.ReadFromJsonAsync<List<MenuItemModel>>()
                               ?? new List<MenuItemModel>();

                var categoryMenuItem = new CategoryMenuItemVM
                {
                    MenuByCategory = menuItems
                        .GroupBy(item => item.Category ?? "Other")
                        .ToDictionary(g => g.Key, g => g.ToList())
                };

                return View(categoryMenuItem);
            }
            catch (Exception)
            {
                // ✅ SAME-PAGE ERROR: Use ModelState since we return View()
                ModelState.AddModelError("", "An error occurred while loading menu items");
                return View(new CategoryMenuItemVM());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var menuItem = await _client.GetFromJsonAsync<MenuItemModel>($"MenuItems/{id}");

                if (menuItem == null)
                {
                    // ✅ REDIRECT ERROR: Use TempData since we return RedirectToAction()
                    SetErrorMessage("Menu item not found");
                    return RedirectToAction("Index");
                }

                return View(menuItem);
            }
            catch (Exception)
            {
                // ✅ REDIRECT ERROR: Use TempData since we return RedirectToAction()
                SetErrorMessage("An error occurred while loading the menu item");
                return RedirectToAction("Index");
            }
        }
    }
}