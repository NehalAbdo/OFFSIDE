using System.ComponentModel.DataAnnotations;

namespace OFF.PL.ViewModels
{
    public class LoginVM
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email!!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
