using System.ComponentModel.DataAnnotations;

namespace OFF.PL.ViewModels
{
    public class ContactVM
    {
        public int ID { get; set; }

        //[Required(ErrorMessage = "The Name Is Required")]
        //public string Name { get; set; }
        [Required(ErrorMessage = "Email is Required"), EmailAddress(ErrorMessage = "Invalid")]

        public string Email { get; set; }
        //[Phone]
        //public string Phone { get; set; }
        public string Message { get; set; }
        public string? UserId { get; set; }

    }
       
    
}
