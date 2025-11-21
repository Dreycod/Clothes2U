using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_role_utilisateur_roluti")]
public class RoleUtilisateur
{
    [Key]
    [Column("roluti_id")]
    public int RoleUtilisateurId { get; set; }
    
    [Column("roluti_libelle")]
    public string RoleUtilisateurLibelle { get; set; }
    
    //relation avec les autres tables : 
    
    [InverseProperty(nameof(Utilisateur.Role))]
    public virtual ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();

}