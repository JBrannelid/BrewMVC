namespace BrewMVC.Models
{
    public class CustomerModel
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; }
    }
}