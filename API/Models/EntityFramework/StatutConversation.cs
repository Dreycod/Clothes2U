using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Models.EntityFramework;

[Table("t_e_statut_conversation_sta")]
public class StatutConversation
{
    [Key]
    [Column("sta_id")]
    public int StatutConversationId { get; set; }
    
    [Column("sta_libelle")]
    public string StatutConversationLibelle { get; set; }
    
    // lien avec la table conversation
    [InverseProperty(nameof(Conversation.StatutConversation))]
    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
}