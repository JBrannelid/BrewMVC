using BrewMVC.ViewModel.Tabels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Error handling: Helper methods for TempData.
// ModelState is lost on redirect but TempData persists for one http reques
// Same View() errors: ModelState since we return View()
// Redirect errors: TempData since we return RedirectToAction()

namespace BrewMVC.Controllers
{
    [Authorize]
    public class AdminTableController : BaseController
    {
        private readonly HttpClient _client;

        public AdminTableController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("BrewAPI");
        }
        
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("Tables");

                if (!response.IsSuccessStatusCode)
                {
                    HandleApiError(response, "Failed to load tables");
                    return View(new List<AdminTableVM>());
                }

                // Deserialize JSON into a list of Tabels - fallback to empty list if null
                var adminTables = await response.Content.ReadFromJsonAsync<List<AdminTableVM>>()
                                  ?? new List<AdminTableVM>();

                return View(adminTables);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while loading tables");
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
            if (!ModelState.IsValid)
            {
                return View(newTable);
            }

            try
            {
                var response = await _client.PostAsJsonAsync("Tables", newTable);
                if (!response.IsSuccessStatusCode)
                {
                    HandleApiError(response, "Failed to create table");
                    return View(newTable);
                }

                SetSuccessMessage("Table created successfully");
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the table");
                return View(newTable);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _client.GetAsync($"Tables/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    HandleApiErrorWithTempData(response, "Table not found");
                    return RedirectToAction("Index");
                }

                var tableContent = await response.Content.ReadFromJsonAsync<EditTableVM>();
                if (tableContent == null)
                {
                    SetErrorMessage("Table not found");
                    return RedirectToAction("Index");
                }

                return View(tableContent);
            }
            catch (Exception ex)
            {
                SetErrorMessage("An error occurred while loading the table");
                return RedirectToAction(actionName: "Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTableVM editTable)
        {
            if (!ModelState.IsValid)
            {
                return View(editTable);
            }

            try
            {
                var response = await _client.PutAsJsonAsync($"Tables/{editTable.TableId}", editTable);
                if (!response.IsSuccessStatusCode)
                {
                    HandleApiError(response, "Failed to update table");
                    return View(editTable);
                }

                SetSuccessMessage("Table updated successfully");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the table");
                return View(editTable);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int tableId)
        {
            try
            {
                var response = await _client.DeleteAsync($"Tables/{tableId}");

                if (!response.IsSuccessStatusCode) 
                {
                    HandleApiErrorWithTempData(response, "Failed to delete table");
                    return RedirectToAction("Index");
                }

                SetSuccessMessage("Table deleted successfully");
            }
            catch (Exception ex)
            {
                SetErrorMessage("An error occurred while deleting the table");
            }

            return RedirectToAction("Index");
        }
    }
}