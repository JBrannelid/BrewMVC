using BrewMVC.Models;
using BrewMVC.ViewModel.Bookings;
using BrewMVC.ViewModel.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Error handling: Helper methods for TempData.
// ModelState is lost on redirect but TempData persists for one http reques
// Same View() errors: ModelState since we return View()
// Redirect errors: TempData since we return RedirectToActio

namespace BrewMVC.Controllers
{
    [Authorize]
    public class AdminBookingController : BaseController
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
                    HandleApiError(response, "Failed to load bookings");
                    return View(new List<BookingListVM>());
                }

                var bookings = await response.Content.ReadFromJsonAsync<List<BookingListVM>>()
                               ?? new List<BookingListVM>();

                return View(bookings);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while loading bookings");
                return View(new List<BookingListVM>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(int customerId)
        {
            if (customerId <= 0)
            {
                SetErrorMessage("Invalid customer selection");
                return RedirectToAction("SelectCustomer");
            }

            var viewModel = new CreateBookingVM
            {
                CustomerId = customerId
            };

            try
            {
                var response = await _client.GetAsync("Bookings/time-slots");
                if (response.IsSuccessStatusCode)
                {
                    var timeSlots = await response.Content.ReadFromJsonAsync<List<TimeOnly>>()
                                   ?? new List<TimeOnly>();
                    viewModel.AvailableTimeSlots = timeSlots.Select(t => new TimeSlotVM { Time = t }).ToList();
                }
                else
                {
                    HandleApiError(response, "Failed to load available time slots");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while loading time slots");
                viewModel.AvailableTimeSlots = new List<TimeSlotVM>();
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingVM newBooking, string action = "create")
        {
            // Reload time slots for any return to view
            try
            {
                var response = await _client.GetAsync("Bookings/time-slots");
                if (response.IsSuccessStatusCode)
                {
                    var timeSlots = await response.Content.ReadFromJsonAsync<List<TimeOnly>>()
                                   ?? new List<TimeOnly>();
                    newBooking.AvailableTimeSlots = timeSlots.Select(t => new TimeSlotVM { Time = t }).ToList();
                }
            }
            catch (Exception)
            {
                newBooking.AvailableTimeSlots = new List<TimeSlotVM>();
            }

            // Check availability action 
            if (action == "checkAvailability")
            {
                if (newBooking.BookingDate == default || newBooking.BookingTime == default || newBooking.NumberGuests <= 0)
                {
                    ModelState.AddModelError("", "Please select date, time, and number of guests first");
                    return View(newBooking);
                }

                try
                {
                    // Helper Bool for UI to display available table
                    newBooking.HasSearched = true;
                    var response = await _client.GetAsync($"Bookings/availability?date={newBooking.BookingDate:yyyy-MM-dd}&time={newBooking.BookingTime:HH:mm}&numberOfGuests={newBooking.NumberGuests}");
                    if (response.IsSuccessStatusCode)
                    {
                        var availability = await response.Content.ReadFromJsonAsync<BookingAvailabilityModel>()
                                         ?? new BookingAvailabilityModel();
                        newBooking.AvailableTables = availability.AvailableTables?.Select(t => new AvailableTableVM
                        {
                            TableId = t.TableId,
                            TableNumber = t.TableNumber,
                            Capacity = t.Capacity
                        }).ToList() ?? new List<AvailableTableVM>();
                    }
                    else
                    {
                        HandleApiError(response, "Failed to check table availability");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An error occurred while checking availability");
                    newBooking.AvailableTables = new List<AvailableTableVM>();
                }

                return View(newBooking);
            }

            // Create booking action 
            if (action == "create")
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
                        HandleApiError(response, "Failed to create booking - table may no longer be available");
                        return View(newBooking);
                    }

                    SetSuccessMessage("Booking created successfully with Pending status");
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An error occurred while creating the booking");
                    return View(newBooking);
                }
            }

            return View(newBooking);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int bookingId)
        {
            try
            {
                var response = await _client.DeleteAsync($"Bookings/{bookingId}");

                if (!response.IsSuccessStatusCode)
                {
                    HandleApiErrorWithTempData(response, "Failed to delete booking");
                    return RedirectToAction("Index");
                }

                SetSuccessMessage("Booking deleted successfully");
            }
            catch (Exception)
            {
                SetErrorMessage("An error occurred while deleting the booking");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult SelectCustomer()
        {
            return View(new CustomerSelectionVM());
        }

        [HttpPost]
        public async Task<IActionResult> SelectCustomer(CustomerSelectionVM model)
        {
            if (string.IsNullOrWhiteSpace(model.SearchTerm))
            {
                ModelState.AddModelError("SearchTerm", "Please enter a search term");
                return View(model);
            }

            try
            {
                var response = await _client.GetAsync($"Customers/search?searchTerm={Uri.EscapeDataString(model.SearchTerm)}");
                if (response.IsSuccessStatusCode)
                {
                    var customers = await response.Content.ReadFromJsonAsync<List<CustomerSearchVM>>()
                                   ?? new List<CustomerSearchVM>();
                    model.AvailableCustomers = customers;
                }
                else
                {
                    HandleApiError(response, "Failed to search for customers");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while searching for customers");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Replace(int bookingId, int customerId)
        {
            try
            {
                var response = await _client.DeleteAsync($"Bookings/{bookingId}");

                if (!response.IsSuccessStatusCode)
                {
                    HandleApiErrorWithTempData(response, "Failed to delete old booking");
                    return RedirectToAction("Index");
                }

                SetSuccessMessage("Old booking deleted. Create a new one below.");
                return RedirectToAction("Create", new { customerId = customerId });
            }
            catch (Exception)
            {
                SetErrorMessage("An error occurred while replacing the booking");
                return RedirectToAction("Index");
            }
        }
    }
}