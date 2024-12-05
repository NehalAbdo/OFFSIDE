using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OFF.DAL.Model
{
    public class Post
    {
        public string Id { get; set; }
        //public string? Title { get; set; }
        public string? Content { get; set; }
        public string? VideoName { get; set; }
        public string? ImageName { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
