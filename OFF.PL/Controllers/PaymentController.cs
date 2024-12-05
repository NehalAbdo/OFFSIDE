using Microsoft.AspNetCore.Mvc;
using OFF.BLL.Interfaces;
using OFF.DAL.Model;
using OFF.PL.Utility;
using OFF.PL.ViewModels;
using Stripe;
using System.Security.Claims;

namespace OFF.PL.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriptionService _subscriptionService;

        public PaymentController(IPaymentService paymentService, IUnitOfWork unitOfWork, ISubscriptionService subscriptionService)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _subscriptionService = subscriptionService;
        }

        const string endpointSecret = "whsec_81c767ea44406a9ab5c320f9247028999ed37ceafbff23a313b31f853a413b87";


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(SubscriptionVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); 
            }

            try
            {
                var agentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(agentId))
                {
                    ModelState.AddModelError("", "Agent ID is missing.");
                    return View(model); 
                }
                model.AgentId = agentId;
                var subscription = await _subscriptionService.CreateSubscriptionAndRecordPaymentAsync(agentId,model.SubscriptionStatus??subscriptionStatus.Active, model.SubscriptionType ?? SubscriptionType.OneMonth,700,model);
                await _unitOfWork.Agents.UpdateAgentVipStatus(agentId);

                return RedirectToAction("Message", new { subscriptionId = subscription.Id }); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during subscription: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                ModelState.AddModelError("", "Failed to subscribe. Please try again later.");
                return View(model);
            }
        }
        public IActionResult Message(int subscriptionId)
        {
           //SubscriptionId = subscriptionId;
            return View();
        }
        public async Task<IActionResult> PaymentSuccess()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get current user ID

            var payments = await _paymentService.GetPaymentsForUser(userId);

            var viewModel = new PaymentSuccessViewModel
            {
                Payments = payments,
            };

            return View(viewModel);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

           
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    endpointSecret
                );

                switch (stripeEvent.Type)
                {
                    case Events.PaymentIntentSucceeded:
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (paymentIntent != null)
                        {
                            await _paymentService.UpdatePaymentIntentToSucccedOrFailed(paymentIntent.Id, true);
                        }
                        break;
                    case Events.PaymentIntentPaymentFailed:
                        var paymentIntentFailed = stripeEvent.Data.Object as PaymentIntent;
                        if (paymentIntentFailed != null)
                        {
                            await _paymentService.UpdatePaymentIntentToSucccedOrFailed(paymentIntentFailed.Id, false);
                        }
                        break;
                    default:
                        break;
                }

                return Ok();
           
        }
            //// Handle Stripe webhook
            //[HttpPost("webhook")]
            //public async Task<IActionResult> StripeWebhook()
            //{
            //    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            //    try
            //    {

            //        var stripeEvent = EventUtility.ConstructEvent(json,
            //            Request.Headers["Stripe-Signature"], endpointSecret);

            //        var PaymentIntent = stripeEvent.Data.Object as PaymentIntent;
            //        if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
            //        {
            //            await _paymentService.UpdatePaymentIntentToSucccedOrFailed(PaymentIntent.Id, false);
            //        }
            //        else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            //        {
            //            await _paymentService.UpdatePaymentIntentToSucccedOrFailed(PaymentIntent.Id, true);

            //        }

            //        return Ok();
            //    }
            //    catch (StripeException e)
            //    {
            //        return BadRequest();
            //    }
            //}


        

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Subscribe(SubscriptionVM model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var agentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


        //    var subscription = new DAL.Model.Subscription
        //    {
        //        SubscriptionType = SubscriptionType.OneMonth,
        //        StartDate = DateTime.UtcNow,
        //        EndDate = DateTime.UtcNow.AddMonths(1), 
        //        Amount = 700,
        //        AgentId = agentId, 
        //    };

        //    await _subscriptionRepo.AddAsync(subscription);
        //    await _unitOfWork.completeAsync();

        //    var createdSubscription = await _paymentService.CreateOrUpdatePaymentIntent(subscription.Id.ToString());

        //    if (createdSubscription == null)
        //    {
        //        ModelState.AddModelError("", "Failed to create or update payment intent.");
        //        return View(model);
        //    }
        //    return RedirectToAction("Index"); 
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Subscribe(SubscriptionVM model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model); // Return to view with validation errors
        //    }

        //    try
        //    {
        //        var agentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //        // Create a subscription
        //        var subscription = new DAL.Model.Subscription
        //        {
        //            SubscriptionType = model.SubscriptionType ?? SubscriptionType.OneMonth, // Ensure SubscriptionType is set correctly
        //            StartDate = DateTime.UtcNow,
        //            EndDate = DateTime.UtcNow.AddMonths(1), // Adjust end date based on SubscriptionType if needed
        //            Amount = 700,
        //            AgentId = agentId
        //        };

        //        await _subscriptionRepo.AddAsync(subscription);
        //        await _unitOfWork.completeAsync();

        //        // Create or update payment intent
        //        var createdSubscription = await _paymentService.CreateOrUpdatePaymentIntent(subscription.Id.ToString());

        //        if (createdSubscription == null)
        //        {
        //            ModelState.AddModelError("", "Failed to create or update payment intent.");
        //            return View(model); // Return to view with error message
        //        }

        //        // Redirect to payment confirmation or success page
        //        return RedirectToAction("Index", "Payment"); // Adjust route as per your application's structure
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", "Failed to subscribe. Please try again later.");

        //        return View(model); // Return to view with generic error message
        //    }
        //    }
        //    [HttpPost("webhook")]
        //    public async Task<IActionResult> StripeWebhook()
        //    {
        //        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        //        try
        //        {
        //            var stripeEvent = EventUtility.ConstructEvent(json,
        //                Request.Headers["Stripe-Signature"], endpointSecret);

        //            var PaymentIntent = stripeEvent.Data.Object as PaymentIntent;
        //            if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
        //            {
        //                await _paymentService.UpdatePaymentIntentToSucccedOrFailed(PaymentIntent.Id, false);
        //            }
        //            else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
        //            {
        //                await _paymentService.UpdatePaymentIntentToSucccedOrFailed(PaymentIntent.Id, true);

        //            }

        //            return Ok();
        //        }
        //        catch (StripeException e)
        //        {
        //            return BadRequest();
        //        }
        //    }
    }


}

