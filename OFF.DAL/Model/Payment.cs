using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFF.DAL.Model
{
   
        public class Payment
        {
            public int Id { get; set; }
        public string? PaymentIntentId { get; set; }
        public DateTime PaymentDate { get; set; }
            public decimal? Amount { get; set; }
            public int SubscriptionId { get; set; }
            public Subscription Subscription { get; set; }
            public string PaymentStatus { get; set; }

    }

}
