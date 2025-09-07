using System.ComponentModel.DataAnnotations;

namespace BrewMVC.ViewModel.Bookings
{
    public class EditBookingVM
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        [Display(Name = "Customer")]
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
        [Range(1, 12, ErrorMessage = "Number of guests must be between 1 and 12")]
        [Display(Name = "Number of Guests")]
        public int NumberGuests { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Duration (hours)")]
        [Range(1, 2, ErrorMessage = "Duration must be between 1 and 2 hours")]
        public int DurationHours { get; set; } = 2;
    }
}