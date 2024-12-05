using OFF.DAL.Model;
using System.ComponentModel.DataAnnotations;

namespace OFF.PL.ViewModels
{
    public class SubscriptionVM
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Card Number is required")]
        [Display(Name = "Card Number")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Card Number must be 16 digits")]

        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Cardholder Name is required")]
        [Display(Name = "Cardholder Name")]
        public string CardholderName { get; set; }
        public subscriptionStatus? SubscriptionStatus { get; set; }

        [Required(ErrorMessage = "Card Expiry Date is required")]
        [Display(Name = "Card Expiry Date")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Enter a valid expiry date (MM/YY)")]
        public string CardExpiryDate { get; set; }

        [Required(ErrorMessage = "CVV is required")]
        [Display(Name = "CVV")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVV must be 3 digits")]
        public string CVV { get; set; }
        [Required(ErrorMessage = "Subscription type is required.")]
        public SubscriptionType? SubscriptionType { get; set; }
        public string? AgentId { get; set; }
        public string? PaymentMethodId { get; set; }

        public decimal? Amount { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }




    }
}
