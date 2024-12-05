using OFF.DAL.Model;
using OFF.PL.ViewModels;
using Stripe.Checkout;

namespace OFF.PL.Utility
{
    public interface ISubscriptionService
    {
        

        Task<Subscription> CreateSubscriptionAndRecordPaymentAsync(string agentId,subscriptionStatus subscriptionStatus, SubscriptionType subscriptionType,decimal amount,SubscriptionVM model);
        Task<Subscription> GetSubscriptionById(int id);
        //Task CreateSubscriptionAsync(string agentId, SubscriptionType subscriptionType, decimal amount);
        //Task HandleSuccessfulPayment(Session session);
        //Task<Subscription> CreateSubscriptionAndRecordPaymentAsync(string agentId, SubscriptionType subscriptionType, decimal amount);
    }
}
