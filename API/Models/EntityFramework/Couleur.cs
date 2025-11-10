using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_couleur_cou")]
public class Couleur
{
    [Key]
    [Column("cou_id")]
    public int CouleurId { get; set; }
    
    [Column("cou_nom")]
    public string Nom { get; set; } = null!;
    
    //relation avec la table est_de_couleur : 
    
    [InverseProperty(nameof(Est_De_Couleur.Couleur))]
    public virtual ICollection<Est_De_Couleur> Annonces { get; set; } = new List<Est_De_Couleur>();
}