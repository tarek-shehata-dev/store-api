using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.APIs.Errors;
using Store.Core.Services.Contract;
using Stripe;
using System.Threading.Tasks;

namespace Store.APIs.Controllers
{
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public PaymentController(IPaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }



        [HttpPost("{basketId}")]
        [Authorize]
        public async Task<IActionResult> CreatePaymentIntent(string basketId)
        {
            if(basketId is  null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));

            var basket = await _paymentService.CreatOrUpdatePaymentIntentAsync(basketId);

            if (basket is null) return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest));

            return Ok(basket);
        }


        [HttpPost("webhook")] 
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var endpointSecret = _configuration["Stripe:WebhookSecret"];

            if (string.IsNullOrWhiteSpace(endpointSecret))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

   
                if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
                {
                  
                   await _paymentService.UpdatePaymentIntentForSuccessOrFailed(paymentIntent.Id,false);
                }
                else if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
                {
                  
                    await _paymentService.UpdatePaymentIntentForSuccessOrFailed(paymentIntent.Id, true);
                }
              
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
