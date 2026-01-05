using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTO;
using Stripe;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IConfiguration configuration, ILogger<PaymentController> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        // Configurer la clé secrète Stripe
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    [HttpPost("create-intent")]
    public async Task<ActionResult<PaymentIntentResponseDTO>> CreatePaymentIntent([FromBody] CreatePaymentIntentDTO request)
    {
        try
        {
            _logger.LogInformation($"Creating payment intent for amount: {request.Amount} {request.Currency}");

            var options = new PaymentIntentCreateOptions
            {
                Amount = request.Amount, // Montant en centimes
                Currency = request.Currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
                Metadata = new Dictionary<string, string>
                {
                    { "annonce_id", request.AnnonceId.ToString() },
                    { "user_id", request.UserId.ToString() },
                    { "address_id", request.AddressId.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            _logger.LogInformation($"✅ Payment intent created: {paymentIntent.Id}");

            return Ok(new PaymentIntentResponseDTO
            {
                ClientSecret = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.Id
            });
        }
        catch (StripeException ex)
        {
            _logger.LogError($"❌ Stripe error: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Unexpected error: {ex.Message}");
            return StatusCode(500, new { error = "An error occurred while processing your payment" });
        }
    }

    [HttpPost("confirm/{paymentIntentId}")]
    public async Task<IActionResult> ConfirmPayment(string paymentIntentId)
    {
        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId);

            if (paymentIntent.Status == "succeeded")
            {
                _logger.LogInformation($"✅ Payment confirmed: {paymentIntentId}");
                return Ok(new { success = true });
            }

            _logger.LogWarning($"⚠️ Payment not succeeded: {paymentIntentId} - Status: {paymentIntent.Status}");
            return BadRequest(new { success = false, status = paymentIntent.Status });
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error confirming payment: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("status/{paymentIntentId}")]
    public async Task<ActionResult<PaymentStatusDTO>> GetPaymentStatus(string paymentIntentId)
    {
        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId);

            return Ok(new PaymentStatusDTO
            {
                Status = paymentIntent.Status,
                PaymentIntentId = paymentIntent.Id,
                Amount = paymentIntent.Amount / 100.0 // Convertir centimes en euros
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error getting payment status: {ex.Message}");
            return NotFound(new { error = "Payment not found" });
        }
    }

    [HttpGet("publishable-key")]
    [AllowAnonymous]
    public IActionResult GetPublishableKey()
    {
        var publishableKey = _configuration["Stripe:PublishableKey"];
        return Ok(new { publishableKey });
    }
}
