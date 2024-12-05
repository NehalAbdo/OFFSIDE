using OFF.DAL.Model;

namespace OFF.PL.ViewModels
{
    public class PaymentSuccessViewModel
    {
        public List<Payment> Payments { get; set; }
        public string? PaymentIntentId { get; set; }

    }
}
