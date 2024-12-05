using System.ComponentModel.DataAnnotations;

namespace OFF.PL.ViewModels
{
    public class ForgetPasswordVM
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email!!")]
        public string Email { get; set; }

    }
}
