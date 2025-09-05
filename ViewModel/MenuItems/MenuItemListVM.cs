namespace BrewMVC.ViewModel.MenuItems
{
    public class MenuItemListVM
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool IsPopular { get; set; }
        public string? ImageUrl { get; set; }
    }
}
