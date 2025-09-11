using System.ComponentModel.DataAnnotations;

namespace BrewMVC.ViewModel.Bookings
{
    public class CreateBookingVM
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Table is required")]
        [Display(Name = "Table")]
        public int TableId { get; set; }

        [Required(ErrorMessage = "Booking date is required")]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateOnly BookingDate { get; set; }

        [Required(ErrorMessage = "Booking time is required")]
        [Display(Name = "Booking Time")]
        [DataType(DataType.Time)]
        public TimeOnly BookingTime { get; set; }

        [Required(ErrorMessage = "Number of guests is required")]
        [Range(1, 8, ErrorMessage = "Number of guests must be between 1 and 8")]
        [Display(Name = "Number of Guests")]
        public int NumberGuests { get; set; }

        // Additional properties for dropdowns and search results
        public bool HasSearched { get; set; } = false;

        public List<TimeSlotVM> AvailableTimeSlots { get; set; } = new();
        public List<AvailableTableVM> AvailableTables { get; set; } = new();
    }
}