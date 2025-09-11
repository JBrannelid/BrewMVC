using BrewMVC.ViewModel.MenuItems;
using Microsoft.AspNetCore.Mvc;

// Error handling: Helper methods for TempData.
// ModelState is lost on redirect but TempData persists for one http reques
// Same View() errors: ModelState since we return View()
// Redirect errors: TempData since we return RedirectToAction()

namespace BrewMVC.Controllers
{
    public class HomeController : BaseController
    {
        private readonly HttpClient _client;

        public HomeController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("BrewAPI");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var popularMenuItems = await _client.GetFromJsonAsync<List<PopularMenuItemVM>>("MenuItems/popular")
                        ?? new List<PopularMenuItemVM>();

                return View(popularMenuItems);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while loading popular menu items");
                return View(new List<PopularMenuItemVM>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}