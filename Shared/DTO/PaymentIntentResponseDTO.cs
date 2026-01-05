namespace Shared.DTO;

public class PaymentIntentResponseDTO
{
    public string ClientSecret { get; set; } = "";
    public string PaymentIntentId { get; set; } = "";
}