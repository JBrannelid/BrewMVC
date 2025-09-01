using Microsoft.AspNetCore.Mvc;

namespace BrewMVC.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
