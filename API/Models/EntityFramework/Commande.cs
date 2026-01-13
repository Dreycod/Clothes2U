using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_commande_cmd")]
public class Commande : IEntity
{
    [Key]
    [Column("cmd_id")]
    public int CommandeId { get; set; }
    
    [Column("cmd_date_commande")]
    public DateTime DateCommande { get; set; }
    
    [Column("cmd_montant_total")]
    public decimal MontantTotal { get; set; }
    
    [Column("cmd_frais_service")]
    public decimal FraisService { get; set; }
    
    [Column("cmd_frais_livraison")]
    public decimal FraisLivraison { get; set; }
    
    [Column("cmd_statut_commande_id")]
    public int StatutCommandeId { get; set; }
    
    [Column("cmd_stripe_payment_intent_id")]
    [MaxLength(255)]
    public string? StripePaymentIntentId { get; set; }
    
    [Column("cmd_numero_suivi")]
    [MaxLength(100)]
    public string? NumeroSuivi { get; set; }
    
    [Column("cmd_date_expedition")]
    public DateTime? DateExpedition { get; set; }
    
    [Column("cmd_date_livraison")]
    public DateTime? DateLivraison { get; set; }
    
    // Foreign Keys
    [Column("cmd_conversation_id")]
    public int ConversationId { get; set; }
    
    [Column("cmd_annonce_id")]
    public int AnnonceId { get; set; }
    
    [Column("cmd_acheteur_id")]
    public int AcheteurId { get; set; }
    
    [Column("cmd_vendeur_id")]
    public int VendeurId { get; set; }
    
    [Column("cmd_adresse_livraison_id")]
    public int AdresseLivraisonId { get; set; }
    
    // Navigation Properties
    [ForeignKey(nameof(AnnonceId))]
    [InverseProperty(nameof(Annonce.Commandes))]
    public virtual Annonce Annonce { get; set; } = null!;
    
    [ForeignKey(nameof(ConversationId))]
    [InverseProperty(nameof(Conversation.Commandes))]
    public virtual Conversation Conversation { get; set; }
    
    [InverseProperty(nameof(MessageEstPayee.Commande))]
    public virtual MessageEstPayee? MessageEstPayee { get; set; }
    
    [ForeignKey(nameof(AcheteurId))]
    [InverseProperty(nameof(Utilisateur.CommandesAchetees))]
    public virtual Utilisateur Acheteur { get; set; } = null!;
    
    [ForeignKey(nameof(VendeurId))]
    [InverseProperty(nameof(Utilisateur.CommandesVendues))]
    public virtual Utilisateur Vendeur { get; set; } = null!;
    
    [ForeignKey(nameof(AdresseLivraisonId))]
    [InverseProperty(nameof(Adresse.Commandes))]
    public virtual Adresse AdresseLivraison { get; set; } = null!;
    
    [ForeignKey(nameof(StatutCommandeId))]
    [InverseProperty(nameof(StatutCommande.Commandes))]
    public virtual StatutCommande StatutCommande { get; set; }
    
    public int GetId() => CommandeId;
}