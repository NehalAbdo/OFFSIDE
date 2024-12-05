using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFF.DAL.Model
{
    public class Agent:AppUser
    {
        public bool VIP { get; set; }
        //public string? ContractsMade { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }

    }
}
