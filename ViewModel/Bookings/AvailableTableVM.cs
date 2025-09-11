namespace BrewMVC.ViewModel.Bookings
{
    public class AvailableTableVM
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public string DisplayText => $"Table {TableNumber} (Capacity: {Capacity})";
    }
}
