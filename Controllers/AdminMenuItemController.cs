using BrewMVC.ViewModel.MenuItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Error handling: Helper methods for TempData.
// ModelState is lost on redirect but TempData persists for one http reques
// Same View() errors: ModelState since we return View()
// Redirect errors: TempData since we return RedirectToActio

namespace BrewMVC.Controllers
{
    [Authorize]
    public class AdminMenuItemController : BaseController
    {
        private readonly HttpClient _client;
        public AdminMenuItemController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("BrewAPI");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("MenuItems");
                if (!response.IsSuccessStatusCode)
                {
                    HandleApiError(response, "Failed to load menu items");
                    return View(new List<MenuItemListVM>());
                }

                // Deserialize JSON into a list of menu items - fallback to empty list if null
                var menuItems = await response.Content.ReadFromJsonAsync<List<MenuItemListVM>>()
                                ?? new List<MenuItemListVM>();

                return View(menuItems);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while loading menu items");
                return View(new List<MenuItemListVM>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMenuItemVM newMenuItem)
        {
            if (!ModelState.IsValid)
            {
                return View(newMenuItem);
            }

            try
            {
                var response = await _client.PostAsJsonAsync("MenuItems", newMenuItem);
                if (!response.IsSuccessStatusCode)
                {
                    HandleApiError(response, "Failed to create menu item");
                    return View(newMenuItem);
                }

                SetSuccessMessage("Menu item created successfully");
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while creating the menu item");
                return View(newMenuItem);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _client.GetAsync($"MenuItems/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    HandleApiErrorWithTempData(response, "Menu item not found");
                    return RedirectToAction("Index");
                }

                var menuItemContent = await response.Content.ReadFromJsonAsync<UpdateMenuItemVM>();
                if (menuItemContent == null)
                {
                    SetErrorMessage("Menu item not found");
                    return RedirectToAction("Index");
                }

                return View(menuItemContent);
            }
            catch (Exception)
            {
                SetErrorMessage("An error occurred while loading the menu item");
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateMenuItemVM updateMenuItem)
        {
            if (!ModelState.IsValid)
            {
                return View(updateMenuItem);
            }

            try
            {
                var response = await _client.PutAsJsonAsync($"MenuItems/{id}", updateMenuItem);
                if (!response.IsSuccessStatusCode)
                {
                    HandleApiError(response, "Failed to update menu item");
                    return View(updateMenuItem);
                }

                SetSuccessMessage("Menu item updated successfully");
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while updating the menu item");
                return View(updateMenuItem);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int menuItemId)
        {
            try
            {
                var response = await _client.DeleteAsync($"MenuItems/{menuItemId}");

                if (!response.IsSuccessStatusCode)
                {
                    HandleApiErrorWithTempData(response, "Failed to delete menu item");
                    return RedirectToAction("Index");
                }

                SetSuccessMessage("Menu item deleted successfully");
            }
            catch (Exception)
            {
                SetErrorMessage("An error occurred while deleting the menu item");
            }

            return RedirectToAction("Index");
        }
    }
}