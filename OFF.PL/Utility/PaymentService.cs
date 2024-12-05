using OFF.BLL.Interfaces;
using OFF.BLL.Repository;
using OFF.DAL.Model;
using OFF.PL.ViewModels;
using Stripe;
using Stripe.Checkout;


namespace OFF.PL.Utility
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly ISubscriptionRepository _subscriptionRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentRepository _paymentRepo;

        public PaymentService(IConfiguration configuration, ISubscriptionRepository subscriptionRepo, IUnitOfWork unitOfWork, IPaymentRepository paymentRepo)
        {
            _configuration = configuration;
            _subscriptionRepo = subscriptionRepo;
            _unitOfWork = unitOfWork;
            _paymentRepo = paymentRepo;
        }
        public async Task<List<Payment>> GetPaymentsForUser(string userId)
        {
            return await _paymentRepo.GetPaymentsByUserIdAsync(userId);
        }
        public async Task<List<Payment>> GetAllPayments()
        {
            return await _unitOfWork.Payments.GetAllAsync(); // Adjust repository method according to your implementation
        }
        public async Task<List<Payment>> GetPaymentsForSubscription(int subscriptionId)
        {
            return await _unitOfWork.Payments.GetPaymentsBySubscriptionId(subscriptionId);
        }

        public async Task UpdatePaymentIntentToSucccedOrFailed(string paymentIntentId, bool succeeded)
        {
            var payment = await _unitOfWork.Payments.GetByPaymentIntentId(paymentIntentId);
            if (payment != null)
            {
                payment.PaymentStatus = succeeded ? "succeeded" : "failed";
                 _unitOfWork.Payments.Update(payment);
                await _unitOfWork.completeAsync();
            }
        }
        //public async Task<string> CreateCheckoutSessionAsync(decimal amount, string successUrl, string cancelUrl)
        //{
        //    var options = new SessionCreateOptions
        //    {
        //        PaymentMethodTypes = new List<string> { "card" },
        //        LineItems = new List<SessionLineItemOptions>
        //    {
        //        new SessionLineItemOptions
        //        {
        //            PriceData = new SessionLineItemPriceDataOptions
        //            {
        //                UnitAmount = (long)(amount * 100),
        //                Currency = "egp",
        //                ProductData = new SessionLineItemPriceDataProductDataOptions
        //                {
        //                    Name = "Monthly Subscription",
        //                    Description = "Subscribe to our monthly plan"
        //                }
        //            },
        //            Quantity = 1,
        //        },
        //    },
        //        Mode = "payment",
        //        SuccessUrl = successUrl,
        //        CancelUrl = cancelUrl,
        //    };

        //    var service = new SessionService();
        //    var session = await service.CreateAsync(options);

        //    return session.Id;
        //}
    }
}


