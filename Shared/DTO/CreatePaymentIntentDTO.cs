namespace Shared;

public class CreatePaymentIntentDTO
{
    public int Amount { get; set; }
    public string Currency { get; set; } = "eur";
    public int AnnonceId { get; set; }
    public int UserId { get; set; }
    public int AddressId { get; set; }
}