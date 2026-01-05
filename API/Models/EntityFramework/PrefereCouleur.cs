using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_j_prefere_couleur_precou")]
public class PrefereCouleur : IEntity
{
    [Key]
    [Column("precou_id")]
    public int  PreferenceCouleurId { get; set; }
    
    //relation avec annonceSuggestionn
    [Column("precou_annonce_sugestion_id")]
    public int AnnonceSuggestionId { get; set; }
    
    [ForeignKey(nameof(AnnonceSuggestionId))]
    [InverseProperty(nameof(AnnoncePreferenceUtilisateur.Couleurs))]
    public virtual AnnoncePreferenceUtilisateur AnnoncePrefere { get; set; } = null!;
    
    //relation couleurs
    [Column("precou_couleur_id")]
    public int CouleurId { get; set; }
    
    [ForeignKey(nameof(CouleurId))]
    [InverseProperty(nameof(Couleur.AnnoncesPreferent))]
    public virtual Couleur Couleur { get; set; } = null!;

    
    public int GetId() => PreferenceCouleurId;
}