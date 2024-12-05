using OFF.DAL.Model;

namespace OFF.PL.ViewModels
{
    public class EditingVM
    
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Nationality { get; set; }
        public string? ImageName { get; set; }
        public IFormFile? Image { get; set; }
        public string? Phone { get; set; }
        public string? Position { get; set; }
        //public string? Experience { get; set; }
        //public int ? Weight { get; set; }
        //public int? Height { get; set; }
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? userName { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

    }

}
