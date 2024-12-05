//using Microsoft.AspNetCore.Mvc;
//using OFF.PL.Utility;
//using Stripe;
//using Stripe.Checkout;

//namespace OFF.PL.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class StripeWebhookController : ControllerBase
//    {
//        private readonly ISubscriptionService _subscriptionService;
//        private readonly string _stripeWebhookSecret;

//        public StripeWebhookController(ISubscriptionService subscriptionService, IConfiguration configuration)
//        {
//            _subscriptionService = subscriptionService;
//            _stripeWebhookSecret = configuration["Stripe:WebhookSecret"];
//        }

//        [HttpPost]
//        public async Task<IActionResult> Index()
//        {
//            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

//            try
//            {
//                var stripeEvent = EventUtility.ConstructEvent(
//                    json,
//                    Request.Headers["Stripe-Signature"],
//                    _stripeWebhookSecret
//                );

//                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
//                {
//                    var session = stripeEvent.Data.Object as Session;
//                    await _subscriptionService.HandleSuccessfulPayment(session);
//                }

//                return Ok();
//            }
//            catch (StripeException e)
//            {
//                return BadRequest(e.Message);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
//            }
//        }
//    }

//}