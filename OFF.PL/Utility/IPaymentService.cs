using OFF.DAL.Model;
using OFF.PL.ViewModels;

namespace OFF.PL.Utility
{
    //public interface IPaymentService
    //{
    //    Task<Subscription?> CreateOrUpdatePaymentIntent(string subscriptionId);
    //    Task<Payment> UpdatePaymentIntentToSucccedOrFailed(string PaymentIntentId, bool flag);
    //    Task UpdateAsync(Payment payment); 
    //}

    public interface IPaymentService
    {
        Task<List<Payment>> GetPaymentsForSubscription(int subscriptionId);
        Task UpdatePaymentIntentToSucccedOrFailed(string paymentIntentId, bool succeeded);
        Task<List<Payment>> GetPaymentsForUser(string userId);
        Task<List<Payment>> GetAllPayments();


    }
}
