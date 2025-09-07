using BrewMVC.Models;

namespace BrewMVC.ViewModel.Bookings
{
    public class BookingListVM
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public int TableId { get; set; }
        public DateOnly BookingDate { get; set; }
        public TimeOnly BookingTime { get; set; }
        public int NumberGuests { get; set; }
        public string Status { get; set; } = string.Empty;
        public TimeSpan DurationTime { get; set; }
        public CustomerModel? Customer { get; set; }
        public TableModel? Table { get; set; }
    }
}