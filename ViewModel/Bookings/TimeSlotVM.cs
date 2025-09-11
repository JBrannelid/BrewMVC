namespace BrewMVC.ViewModel.Bookings
{
    public class TimeSlotVM
    {
        public TimeOnly Time { get; set; }
        public string DisplayText => $"{Time:HH:mm} - {Time.AddHours(2):HH:mm}";
        public string Value => Time.ToString("HH:mm");
    }

}
