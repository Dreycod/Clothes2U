using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_j_est_de_couleur_edc")]
public class Est_De_Couleur
{
    [Key]
    [Column("edc_id")]
    public int EstDeCouleurId { get; set; }
    
    //id des autres tables
    
    [Column("edc_couleur_id")]
    public int CouleurId { get; set; }
    
    [Column("edc_annonce_id")]
    public int AnnonceId { get; set; }
    
    //relation avec les autres tables 
    
    [ForeignKey(nameof(CouleurId))]
    [InverseProperty(nameof(Couleur.Annonces))]
    public virtual Couleur Couleur { get; set; } = null!;
    
    [ForeignKey(nameof(AnnonceId))]
    [InverseProperty(nameof(Annonce.Couleurs))]
    public virtual Annonce Annonce { get; set; } = null!;
    
}