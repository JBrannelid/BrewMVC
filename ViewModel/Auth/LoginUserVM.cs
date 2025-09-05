using System.ComponentModel.DataAnnotations;

namespace BrewMVC.ViewModel.Auth
{
    public class LoginUserVM
    {
        [Required(ErrorMessage = "Emailadress is required")]
        [EmailAddress(ErrorMessage = "Emailadress isn't valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
