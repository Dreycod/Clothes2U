using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_statut_commande_stacom")]
public class StatutCommande : IEntity
{
  [Key]
  [Column("stacom_id")]
  public int StatutCommandeId { get; set; }
  
  [Column("stacom_libelle")]
  public string Libelle { get; set; }
  
  [InverseProperty(nameof(Commande.StatutCommande))]
  public virtual ICollection<Commande> Commandes { get; set; } = new List<Commande>();
  
  public int GetId() => StatutCommandeId;
}