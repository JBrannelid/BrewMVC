using System.ComponentModel.DataAnnotations;

namespace BrewMVC.ViewModel.Bookings
{
    public class CreateBookingVM
    {
        [Required(ErrorMessage = "Customer is required")]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Table is required")]
        [Display(Name = "Table")]
        public int TableId { get; set; }

        [Required(ErrorMessage = "Booking date is required")]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateOnly BookingDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required(ErrorMessage = "Booking time is required")]
        [Display(Name = "Booking Time")]
        [DataType(DataType.Time)]
        public TimeOnly BookingTime { get; set; } = new TimeOnly(18, 0); // Default 6:00 PM

        [Required(ErrorMessage = "Number of guests is required")]
        [Range(1, 8, ErrorMessage = "Number of guests must be between 1 and 8")]
        [Display(Name = "Number of Guests")]
        public int NumberGuests { get; set; } = 2;

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Confirmed";

        [Display(Name = "Duration (hours)")]
        [Range(1, 8, ErrorMessage = "Duration must be between 1 and 8 hours")]
        public int DurationHours { get; set; } = 2;
    }
}