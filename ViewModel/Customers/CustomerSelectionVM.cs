using System.ComponentModel.DataAnnotations;
using BrewMVC.ViewModel.Customers;

namespace BrewMVC.ViewModel.Bookings
{
    public class CustomerSelectionVM
    {
        [Required(ErrorMessage = "Please enter a search term")]
        [Display(Name = "Search Customer")]
        public string SearchTerm { get; set; } = string.Empty;

        public List<CustomerSearchVM> AvailableCustomers { get; set; } = new();
    }
}