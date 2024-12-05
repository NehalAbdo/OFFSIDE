using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OFF.DAL.Model
{
    public class Subscription
    {
        public int Id { get; set; }

        public SubscriptionType SubscriptionType { get; set; }
        //public SubStatus SubStatus { get; set; }
        public DateTime StartDate { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public DateTime EndDate { get; set; }
        public decimal? Amount { get; set; }
        public string? AgentId { get; set; }
        public Agent? Agent { get; set; }
        public subscriptionStatus? SubscriptionStatus { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        //public bool? PaymentIntentRequiresPaymentMethod { get; set; }





    }
}

