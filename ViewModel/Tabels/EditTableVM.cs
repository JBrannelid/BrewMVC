using System.ComponentModel.DataAnnotations;

namespace BrewMVC.ViewModel.Tabels
{
    public class EditTableVM
    {
        public int TableId { get; set; }

        [Required(ErrorMessage = "Table number is required")]
        [Display(Name = "Table Number")]
        public int TableNumber { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 8, ErrorMessage = "Capacity must be between 1 and 8 guests")]
        [Display(Name = "Capacity guests")]
        public int Capacity { get; set; }
    }
}