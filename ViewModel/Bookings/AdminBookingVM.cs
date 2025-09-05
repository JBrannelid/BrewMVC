using BrewMVC.ViewModel;
using BrewMVC.ViewModel.Tabels;

namespace BrewMVC.ViewModel.Bookings
{
    public class AdminBookingVM
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public int TableId { get; set; }
        public DateOnly BookingDate { get; set; }
        public TimeOnly BookingTime { get; set; }
        public int NumberGuests { get; set; }
        public string Status { get; set; } = string.Empty;
        public TimeSpan DurationTime { get; set; }
        public AdminTableVM? Table { get; set; }
    }
}