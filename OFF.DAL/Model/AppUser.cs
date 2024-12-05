using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OFF.DAL.Model
{
    public class AppUser :IdentityUser
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Nationality { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        //public int? Rate { get; set; }
        //public string? City { get; set; }
        public string? ImageName { get; set; }
        public string? NationalIdName { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>(); 



    }
}
