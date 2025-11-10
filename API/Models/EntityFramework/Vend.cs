using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_j_vend_ven")]
public class Vend
{
    [Key]
    [Column("ven_id")]
    public int VendId { get; set; }
    
    // id des relations
    [Column("ven_utilisateur_vendeur_id")]
    public int UtilisateurVendeurId { get; set; }
    
    [Column("ven_conversation_id")]
    public int ConversationId { get; set; }
    
    //relation avec les autres tables
    [ForeignKey(nameof(UtilisateurVendeurId))]
    [InverseProperty(nameof(Utilisateur.Ventes))]
    public virtual Utilisateur UtilisateurVendeur { get; set; }
    
    [ForeignKey(nameof(ConversationId))]
    [InverseProperty(nameof(Conversation.Vendeur))]
    public virtual Conversation LaConversation { get; set; }
}