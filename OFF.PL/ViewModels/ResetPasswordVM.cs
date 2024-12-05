using System.ComponentModel.DataAnnotations;

namespace OFF.PL.ViewModels
{
    public class ResetPasswordVM
    {
        [Required]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Invalid Password")]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Password Mismatch")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }

        public string Token { get; set; }
    }
}
