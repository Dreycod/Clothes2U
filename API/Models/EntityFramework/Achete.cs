using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Models.EntityFramework;

[Table("t_j_achete_ach")]
public class Achete
{
    [Key]
    [Column("ach_id")]
    public int AcheteId { get; set; }
    
    // id des relations
    [Column("ach_utilisateur_acheteur_id")]
    public int UtilisateurAcheteurId { get; set; }
    
    [Column("ach_conversation_id")]
    public int ConversationId { get; set; }
    
    //relations avec les autres tables
    
    [ForeignKey(nameof(ConversationId))]
    [InverseProperty(nameof(Conversation.Acheteur))]
    public virtual Conversation Conversation { get; set; }

    [ForeignKey(nameof(UtilisateurAcheteurId))]
    [InverseProperty(nameof(Utilisateur.Achats))]
    public virtual Utilisateur UtilisateurAcheteur { get; set; }
}