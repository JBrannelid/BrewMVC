namespace BrewMVC.Models
{
    public class BookingAvailabilityModel
    {
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public int NumberGuests { get; set; }
        public List<AvailableTableModel> AvailableTables { get; set; } = new();
    }
}