//using BankingWebAPI.Infrastructure.Services.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;

//namespace BankingWebAPI.Presentation.Controllers
//{
//    [ApiController]
//    [Route("api/webhooks/paystack")]

//    public class PaystackWebhookController : ControllerBase
//    {
//        private readonly ILogger<PaystackWebhookController> _logger;
//        private readonly IPaymentGateway _paymentService;

//        public PaystackWebhookController(
//            ILogger<PaystackWebhookController> logger,
//            IPaymentGateway paymentService)
//        {
//            _logger = logger;
//            _paymentService = paymentService;
//        }

//        [HttpPost]
//        public async Task<IActionResult> HandleWebhook()
//        {
//            var json = await new StreamReader(Request.Body).ReadToEndAsync();
//            var signature = Request.Headers["x-paystack-signature"];

//            // Verify signature here (omitted for brevity)

//            var webhookEvent = JsonConvert.DeserializeObject<PaystackWebhookEvent>(json);

//            _logger.LogInformation($"Received Paystack webhook: {webhookEvent.Event}");

//            if (webhookEvent.Event == "charge.success")
//            {
//                await _paymentService.ProcessSuccessfulPayment(
//                    webhookEvent.Data.Reference,
//                    webhookEvent.Data.Amount);
//            }

//            return Ok();
//        }
//    }
//}
