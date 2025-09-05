using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace BrewMVC.Controllers
{
    [Authorize]
    public class AdminBookingController : Controller
    {
        private readonly HttpClient _client;

        public AdminBookingController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("BrewAPI");
        }

        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Request.Cookies["jwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }


        public IActionResult Index()
        {
            SetAuthorizationHeader();

            return View();
        }
    }
}
