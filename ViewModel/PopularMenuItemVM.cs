using System.ComponentModel.DataAnnotations;

namespace BrewMVC.ViewModels
{
    public class PopularMenuItemVM
    {
        public string? Description { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
