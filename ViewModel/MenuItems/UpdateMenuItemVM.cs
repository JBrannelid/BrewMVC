using System.ComponentModel.DataAnnotations;

namespace BrewMVC.ViewModel.MenuItems
{
    public class UpdateMenuItemVM
    {
        public int MenuItemId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [StringLength(maximumLength: 30, ErrorMessage = "Category cannot exceed 30 characters")]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 9999.99, ErrorMessage = "Price must be between range of: 0.01 - 9999.99 kr")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string Description { get; set; }

        [Display(Name = "Is Popular")]
        public bool IsPopular { get; set; }

        [Display(Name = "Image URL")]
        [Url(ErrorMessage = "ImageUrl must be a valid URL-adress")]
        public string? ImageUrl { get; set; }
    }
}