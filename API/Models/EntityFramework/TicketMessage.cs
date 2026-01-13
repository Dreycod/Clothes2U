using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_ticket_message_ticmes")]
public class TicketMessage : IEntity
{
    [Key]
    [Column("ticmes_id")]
    public int TicketMessageId { get; set; }
    
    [Column("ticmes_utilisateur_id")]
    public int UtilisateurId { get; set; }
    
    [Column("ticmes_date_envoi")]
    public DateTime DateEnvoi { get; set; }
    
    [Column("ticmes_content")]
    public string Content { get; set; }
    
    [Column("ticmes_ticket_id")]
    public int TicketId { get; set; }
    
    //relation avec les autres tables: 
    [ForeignKey(nameof(TicketId))]
    [InverseProperty(nameof(Ticket.Messages))]
    public virtual Ticket? Ticket { get; set; }
    
    [ForeignKey(nameof(UtilisateurId))]
    [InverseProperty(nameof(Utilisateur.MessagesSupport))]
    public virtual Utilisateur? Utilisateur { get; set; }

    
    
    public int GetId() =>  TicketMessageId;
    
}