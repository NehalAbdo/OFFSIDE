using OFF.DAL.Model;

namespace OFF.PL.ViewModels
{
    public class PostVM
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? VideoName { get; set; }
        public IFormFile? Video { get; set; }
        public string? ImageName { get; set; }
        public IFormFile? Image { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

    }
}
