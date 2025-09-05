using BrewMVC.Models;

namespace BrewMVC.ViewModel.MenuItems
{
    public class CategoryMenuItemVM
    {
        public Dictionary<string, List<MenuItemModel>> MenuByCategory { get; set; } = new();
        public List<string> CategoryOrder { get; set; } = new() { "Salads", "Bowls", "Beverages", "Desserts" };
    }
}