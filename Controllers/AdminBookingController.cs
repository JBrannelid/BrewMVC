using BrewMVC.ViewModel.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _client.GetAsync("Bookings");
                if (!response.IsSuccessStatusCode)
                {
                    return View(new List<BookingListVM>());
                }

                var bookings = await response.Content.ReadFromJsonAsync<List<BookingListVM>>()
                               ?? new List<BookingListVM>();

                return View(bookings);
            }
            catch (Exception)
            {
                return View(new List<BookingListVM>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingVM newBooking)
        {
            if (!ModelState.IsValid)
            {
                return View(newBooking);
            }

            try
            {
                var response = await _client.PostAsJsonAsync("Bookings", newBooking);
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to create booking");
                    return View(newBooking);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(newBooking);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _client.GetAsync($"Bookings/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var booking = await response.Content.ReadFromJsonAsync<EditBookingVM>();
                if (booking == null)
                {
                    return NotFound();
                }

                return View(booking);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditBookingVM editBooking)
        {
            if (!ModelState.IsValid)
            {
                return View(editBooking);
            }

            try
            {
                var response = await _client.PutAsJsonAsync($"Bookings/{id}", editBooking);
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to update booking");
                    return View(editBooking);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(editBooking);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int bookingId)
        {
            try
            {
                var response = await _client.DeleteAsync($"Bookings/{bookingId}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to delete booking";
                    return RedirectToAction("Index");
                }

                TempData["SuccessMessage"] = "Booking deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}