namespace Shared.DTO;

public class CreateOrderDTO
{
    public int AnnonceId { get; set; }
    public int AcheteurId { get; set; }
    public int VendeurId { get; set; }
    public int AdresseLivraisonId { get; set; }
    public decimal MontantTotal { get; set; }
    public decimal FraisService { get; set; }
    public decimal FraisLivraison { get; set; }
    public string StripePaymentIntentId { get; set; } = "";
    public string StatutCommande { get; set; } = "EnAttente";
}