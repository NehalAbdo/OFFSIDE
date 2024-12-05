using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using OFF.BLL.Interfaces;
using OFF.BLL.Repository;
using OFF.DAL.Model;
using OFF.PL.ViewModels;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace OFF.PL.Utility
{

    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IConfiguration _configuration;
        public SubscriptionService(IUnitOfWork unitOfWork, ISubscriptionRepository subscriptionRepository, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _subscriptionRepository = subscriptionRepository;
            _configuration = configuration;
        }
        public async Task<DAL.Model.Subscription?> CreateSubscriptionAndRecordPaymentAsync(string agentId,subscriptionStatus subscriptionStatus, SubscriptionType subscriptionType, decimal amount, SubscriptionVM model)
        {
            if (string.IsNullOrEmpty(agentId))
            {
                throw new ArgumentException("Invalid Agent ID");
            }

            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = CalculateSubscriptionEndDate(subscriptionType, startDate);

            var subscription = new DAL.Model.Subscription
            {

                Amount = 700,
                AgentId = agentId,
                SubscriptionType = subscriptionType,
                StartDate = startDate,
                EndDate = endDate,
                SubscriptionStatus= subscriptionStatus,
                PaymentIntentId = model.PaymentIntentId,
                ClientSecret= model.ClientSecret,
            };
            if (subscription.PaymentIntentId is null)
            {
                await ProcessPaymentThroughStripeAsync(subscription.Id);
                
            } 
            
                await _unitOfWork.Subscriptions.AddAsync(subscription);
                await _unitOfWork.completeAsync();

                var payment = new Payment
                {
                    PaymentIntentId = model.PaymentIntentId,
                    SubscriptionId = subscription.Id,
                    PaymentDate = DateTime.UtcNow,
                    PaymentStatus = "Pending",
                    Amount = 700,
                };
                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.completeAsync();

             var paymentSuccess=  await ProcessPaymentThroughStripeAsync(subscription.Id);
            if (paymentSuccess != null)
            {
                model.PaymentIntentId = paymentSuccess.PaymentIntentId;
                payment.PaymentStatus = "Succeeded";
            }
            else
            {
                payment.PaymentStatus = "Failed";
            }
            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.completeAsync();

            return subscription;

        }

        public async Task<DAL.Model.Subscription?> ProcessPaymentThroughStripeAsync(int subscriptionId)
        {
           
            StripeConfiguration.ApiKey = _configuration["StripeSetting:SecretKey"];

            var Subscribe = await _subscriptionRepository.GetByIdAsync(subscriptionId);
            if (Subscribe is null) return null;
            long amount = (long)700 * 100;
            PaymentIntent paymentIntent;
            var Service = new PaymentIntentService();
            if (string.IsNullOrEmpty(Subscribe.PaymentIntentId))//create
            {
                var CreatedOptions = new PaymentIntentCreateOptions()
                {
                    Amount = amount,
                    Currency = "egp",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                paymentIntent = await Service.CreateAsync(CreatedOptions);
                Subscribe.PaymentIntentId = paymentIntent.Id;
                Subscribe.ClientSecret = paymentIntent.ClientSecret;

            }
            else
            {
                //update
                var UpdatedOptions = new PaymentIntentUpdateOptions()
                {
                    Amount = amount,

                };
                paymentIntent = await Service.UpdateAsync(Subscribe.PaymentIntentId, UpdatedOptions);

                Subscribe.PaymentIntentId = paymentIntent.Id;
                Subscribe.ClientSecret = paymentIntent.ClientSecret;

            }

            await _subscriptionRepository.UpdateAsync(Subscribe);
            return Subscribe;
        }

        //    paymentIntent = await service.CreateAsync(options);
        //    subscription.PaymentIntentedId = paymentIntent.Id;
        //    subscription.ClientSecret = paymentIntent.ClientSecret;
        //}
        //else
        //{
        //    var options = new PaymentIntentUpdateOptions
        //    {
        //        Amount = amount,
        //        PaymentMethod = model.PaymentMethodId,

        //    };

        //    paymentIntent = await service.UpdateAsync(subscription.PaymentIntentedId, options);
        //}

        //        if (paymentIntent.Status == "requires_payment_method")
        //        {

        //            subscription.PaymentIntentRequiresPaymentMethod = true;
        //            subscription.ClientSecret = paymentIntent.ClientSecret;

        //            return false;
        //        }
        //        else if (paymentIntent.Status == "succeeded")
        //        {
        //            subscription.PaymentIntentRequiresPaymentMethod = false;

        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (StripeException e)
        //    {
        //        throw new Exception("Error processing payment through Stripe", e);
        //    }
        //}


        //private async Task<bool> ProcessPaymentThroughStripeAsync(DAL.Model.Subscription subscription, SubscriptionVM model)
        //{

        //    StripeConfiguration.ApiKey = _configuration["SrripeSetting:SecretKey"];
        //    try
        //    {
        //        long amount = (long)700 * 100;
        //        var service = new PaymentIntentService();
        //        PaymentIntent paymentIntent;

        //        if (string.IsNullOrWhiteSpace(subscription.PaymentIntentedId))
        //        {
        //            var options = new PaymentIntentCreateOptions
        //            {
        //                Amount = amount,
        //                Currency = "egp",
        //                PaymentMethodTypes = new List<string> { "card" },
        //                PaymentMethod = model.PaymentMethodId,

        //            };

        //            paymentIntent = await service.CreateAsync(options);
        //            subscription.PaymentIntentedId = paymentIntent.Id;
        //            subscription.ClientSecret = paymentIntent.ClientSecret;
        //        }
        //        else
        //        {
        //            var options = new PaymentIntentUpdateOptions
        //            {
        //                Amount = amount,
        //                PaymentMethod = model.PaymentMethodId,

        //            };

        //            paymentIntent = await service.UpdateAsync(subscription.PaymentIntentedId, options);
        //        }

        //        if (paymentIntent.Status == "requires_payment_method")
        //        {

        //            subscription.PaymentIntentRequiresPaymentMethod = true;
        //            subscription.ClientSecret = paymentIntent.ClientSecret;

        //            return false;
        //        }
        //        else if (paymentIntent.Status == "succeeded")
        //        {
        //            subscription.PaymentIntentRequiresPaymentMethod = false;

        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (StripeException e)
        //    {
        //        throw new Exception("Error processing payment through Stripe", e);
        //    }
        //}

        //public async Task CreateSubscriptionAsync(string agentId, SubscriptionType subscriptionType, decimal amount)
        //{
        //    try
        //    {
        //        DateTime startDate = DateTime.UtcNow;
        //        DateTime endDate = CalculateSubscriptionEndDate(subscriptionType, startDate);

        //        var subscription = new DAL.Model.Subscription
        //        {
        //            AgentId = agentId,
        //            SubscriptionType = subscriptionType,
        //            StartDate = startDate,
        //            EndDate = endDate,
        //            Amount = amount,
        //            SubStatus = subscriptionType == SubscriptionType.FreeTrial ? SubStatus.NotPaid : SubStatus.Paid
        //        };

        //        await _unitOfWork.Subscriptions.AddAsync(subscription);
        //        await _unitOfWork.completeAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("Failed to create subscription", ex);
        //    }
        //}

        //public async Task HandleSuccessfulPayment(Session session)
        //{
        //    try
        //    {
        //        var subscriptions = await _unitOfWork.Subscriptions.GetSubscriptionsByAgentIdAsync(session.CustomerEmail);

        //        foreach (var subscription in subscriptions)
        //        {
        //            subscription.SubStatus = SubStatus.Paid;
        //        }

        //        await _unitOfWork.completeAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("Failed to handle successful payment", ex);
        //    }
        //}

        //private DateTime CalculateSubscriptionEndDate(SubscriptionType subscriptionType, DateTime startDate)
        //{
        //    return subscriptionType switch
        //    {
        //        SubscriptionType.OneMonth => startDate.AddMonths(1),
        //        _ => throw new ArgumentException("Invalid subscription type.")
        //    };
        //}

        //public async Task<DAL.Model.Subscription> CreateSubscriptionAndRecordPaymentAsync(string agentId, SubscriptionType subscriptionType, decimal amount)
        //{
        //    DateTime startDate = DateTime.UtcNow;
        //    DateTime endDate = CalculateSubscriptionEndDate(subscriptionType, startDate);
        //    //    var agentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


        //    var subscription = new DAL.Model.Subscription
        //    {
        //        AgentId = agentId,
        //        SubscriptionType = subscriptionType,
        //        StartDate = startDate,
        //        EndDate = endDate,
        //        Amount = amount,
        //        SubStatus = subscriptionType == SubscriptionType.FreeTrial ? SubStatus.NotPaid : SubStatus.Paid
        //    };

        //    await _unitOfWork.Subscriptions.AddAsync(subscription);
        //    await _unitOfWork.completeAsync();

        //    var payment = new Payment
        //    {
        //        SubscriptionId = subscription.Id,
        //        PaymentDate = DateTime.UtcNow,
        //        Amount = amount,
        //        PaymentStatus = "pending"
        //    };

        //    await _unitOfWork.Payments.AddAsync(payment);
        //    await _unitOfWork.completeAsync();

        //    return subscription;
        //}



        private DateTime CalculateSubscriptionEndDate(SubscriptionType subscriptionType, DateTime startDate)
        {
            switch (subscriptionType)
            {
                case SubscriptionType.OneMonth:
                    return startDate.AddMonths(1);
                case SubscriptionType.FreeTrial:
                    return startDate.AddDays(30);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<DAL.Model.Subscription?> GetSubscriptionById(int id) => await _subscriptionRepository.GetByIdAsync(id);
        
    }
}

