using BrewMVC.ViewModel.MenuItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BrewMVC.Controllers
{
    [Authorize]
    public class AdminMenuItemController : Controller
    {
        private readonly HttpClient _client;
        public AdminMenuItemController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("BrewAPI");
        }

        // Function to add JWT-token for auth API request 
        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Request.Cookies["jwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            SetAuthorizationHeader();

            try
            {
                var response = await _client.GetAsync("MenuItems");
                if (!response.IsSuccessStatusCode)
                {
                    return View(new List<MenuItemListVM>());
                }

                // Deserialize JSON into a list of menu items - fallback to empty list if null
                var menuItems = await response.Content.ReadFromJsonAsync<List<MenuItemListVM>>()
                                ?? new List<MenuItemListVM>();

                return View(menuItems);
            }
            catch (Exception)
            {
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
            SetAuthorizationHeader();

            if (!ModelState.IsValid)
            {
                return View(newMenuItem);
            }

            try
            {
                var response = await _client.PostAsJsonAsync("MenuItems", newMenuItem);
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to create menu item");
                    return View(newMenuItem);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred\"{ex.Message}");
                return View(newMenuItem);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();

            try
            {
                var response = await _client.GetAsync($"MenuItems/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var menuItemContent = await response.Content.ReadFromJsonAsync<UpdateMenuItemVM>();
                if (menuItemContent == null)
                {
                    return NotFound();
                }

                return View(menuItemContent);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred\"{ex.Message}");
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateMenuItemVM updateMenuItem)
        {
            SetAuthorizationHeader();

            if (!ModelState.IsValid)
            {
                return View(updateMenuItem);
            }

            try
            {
                var response = await _client.PutAsJsonAsync($"MenuItems/{id}", updateMenuItem);
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to update menu item");
                    return View(updateMenuItem);
                }

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(updateMenuItem);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int menuItemId)
        {
            // TempData for errors on Delete since we redirect to Index
            // ModelState is lost on redirect but TempData persists for one reques

            SetAuthorizationHeader();

            try
            {
                var response = await _client.DeleteAsync($"MenuItems/{menuItemId}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to delete table";
                    return RedirectToAction("Index");
                }

                TempData["SuccessMessage"] = "Table deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}