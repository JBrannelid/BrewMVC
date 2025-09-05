using Microsoft.AspNetCore.Mvc;

namespace BrewMVC.Controllers
{
    public class BookingController : Controller
    {
        private readonly HttpClient _client;

        public BookingController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("BrewAPI");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
