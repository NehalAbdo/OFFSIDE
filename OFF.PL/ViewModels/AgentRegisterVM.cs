using System.ComponentModel.DataAnnotations;

namespace OFF.PL.ViewModels
{
    public class AgentRegisterVM
    {
        public string? Id { get; set; }
        public string FName { get; set; }
        public bool? VIP { get; set; }
        public string LName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Confirm Password dosen`t match Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Email is required"), EmailAddress(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}$", ErrorMessage = "Invalid phone number format.")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 characters.")]
        public string Phone { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public IFormFile Image { get; set; }
        public string? ImageName { get; set; }

        public IFormFile National { get; set; }
        public string? NationalIDName { get; set; }
       public string? userName {  get; set; }
        public List<PostVM> Posts { get; set; } = new List<PostVM>();

    }
}
