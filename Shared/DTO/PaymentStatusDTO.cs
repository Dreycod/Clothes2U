namespace Shared.DTO;

public class PaymentStatusDTO
{
    public string Status { get; set; } = "";
    public string PaymentIntentId { get; set; } = "";
    public double Amount { get; set; }
}