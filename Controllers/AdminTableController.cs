using BrewMVC.ViewModel.MenuItems;
using BrewMVC.ViewModel.Tabels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace BrewMVC.Controllers
{
    [Authorize]
    public class AdminTableController : Controller
    {
        private readonly HttpClient _client;

        public AdminTableController(IHttpClientFactory clientFactory)
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
        
        public async Task<IActionResult> Index()
        {
            SetAuthorizationHeader();

            try
            {
                var response = await _client.GetAsync("Tables");
                // Deserialize JSON into a list of Tabels - fallback to empty list if null
                var adminTables = await response.Content.ReadFromJsonAsync<List<AdminTableVM>>()
                                  ?? new List<AdminTableVM>();

                return View(adminTables);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(key: "", $"An error occurred\"{ex.Message}");
                return View(model: new List<AdminTableVM>());
            }
        }

        [HttpGet]
        public IActionResult Create ()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTableVM newTable)
        {
            SetAuthorizationHeader();

            if (!ModelState.IsValid)
            {
                return View(newTable);
            }

            try
            {
                var response = await _client.PostAsJsonAsync("Tables", newTable);
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to create table");
                    return View(newTable);
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(key: "", $"An error occurred\"{ex.Message}");
                return View(newTable);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();

            try
            {
                var response = await _client.GetAsync($"Tables/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }
                var tableContent = await response.Content.ReadFromJsonAsync<EditTableVM>();
                if (tableContent == null)
                {
                    return NotFound();
                }

                return View(tableContent);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred\"{ex.Message}");
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTableVM editTable)
        {
            SetAuthorizationHeader();

            if (!ModelState.IsValid)
            {
                return View(editTable);
            }

            try
            {
                var response = await _client.PutAsJsonAsync($"Tables/{editTable.TableId}", editTable);
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to update menu item");
                    return View(editTable);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(key: "", $"An error occurred: {ex.Message}");
                return View(editTable);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int tableId)
        {
            // TempData for errors on Delete since we redirect to Index
            // ModelState is lost on redirect but TempData persists for one reques

            SetAuthorizationHeader();

            try
            {
                var response = await _client.DeleteAsync($"Tables/{tableId}");

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