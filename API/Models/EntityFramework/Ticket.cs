using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_ticket_tic")]
public class Ticket : IEntity
{
    [Key]
    [Column("tic_id")]
    public int TicketId { get; set; }
    
    [Column("tic_subject")]
    public string TicketSubject { get; set; }
    
    [Column("tic_utilisateur_id")]
    public int UtilisateurId { get; set; }
    
    [Column("tic_date_creation")]
    public DateTime DateCreation { get; set; }
    
    [Column("tic_status")]
    public int Status { get; set; }
    
    //relation avec les autres tables
    [InverseProperty(nameof(TicketMessage.Ticket))]
    public virtual ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    [ForeignKey(nameof(UtilisateurId))]
    [InverseProperty(nameof(Utilisateur.Tickets))]
    public virtual Utilisateur? Utilisateur { get; set; }


    
    
    public int GetId() =>  TicketId;
}