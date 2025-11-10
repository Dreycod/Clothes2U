using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_statut_stauti")]
public class StatutUtilisateur
{
    [Key]
    [Column("stauti_id")]
    public int StatutUtilisateurId { get; set; }
    
    [Column("stauti_libelle _statut")]
    public string StatutLibelle { get; set; }
    
    //relation avec la table utilisateur
    
    [InverseProperty(nameof(Utilisateur.Statut))]
    public virtual ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
}