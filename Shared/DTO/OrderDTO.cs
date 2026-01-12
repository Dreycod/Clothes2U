namespace Shared.DTO;

public class OrderDTO
{
    public int CommandeId { get; set; }
    public DateTime DateCommande { get; set; }
    public double MontantTotal { get; set; }
    public double FraisService { get; set; }
    public double FraisLivraison { get; set; }
    public string Statut { get; set; } = "";
    public string? StripePaymentIntentId { get; set; }
    public string? NumeroSuivi { get; set; }
    public DateTime? DateExpedition { get; set; }
    public DateTime? DateLivraison { get; set; }
    public int ConversationId { get; set; }
    
    // Informations de l'annonce
    public int AnnonceId { get; set; }
    public string TitreAnnonce { get; set; } = "";
    public string? PhotoAnnonce { get; set; }
    public double PrixAnnonce { get; set; }
    
    // Informations acheteur
    public int AcheteurId { get; set; }
    public string NomAcheteur { get; set; } = "";
    public string EmailAcheteur { get; set; } = "";
    
    // Informations vendeur
    public int VendeurId { get; set; }
    public string NomVendeur { get; set; } = "";
    public string EmailVendeur { get; set; }= "";
    
    // Adresse de livraison
    public AdresseLivraisonDTO AdresseLivraison { get; set; } = new();
}