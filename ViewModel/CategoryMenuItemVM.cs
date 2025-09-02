using BrewMVC.Models;

namespace BrewMVC.ViewModel
{
    public class CategoryMenuItemVM
    {
        public Dictionary<string, List<MenuItems> >MenuByCategory { get; set; }
        public List<string> CategoryOrder { get; set; } = new() { "Salads", "Bowls", "Beverages", "Desserts" };
    }
}
