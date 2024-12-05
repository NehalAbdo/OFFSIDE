using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFF.DAL.Model
{
    public class Contact
    {
        public int Id { get; set; }

        public DateTime DateOfMessage { get; set; } = DateTime.Now;
        public string Message { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
    }
}
