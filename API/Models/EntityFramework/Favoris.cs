using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_j_favoris_fav")]
public class Favoris
{
    [Key]
    [Column("fav_id")]
    public int FavorisId { get; set; }
    
    //id pour les relations
    
    [Column("fav_annonce_id")]
    public int AnnonceId { get; set; }
    
    [Column("fav_utilisateur_id")]
    public int UtilisateurId { get; set; }
    
    //relation avec les autres tables :
    
    [ForeignKey(nameof(AnnonceId))]
    [InverseProperty(nameof(Annonce.UtilisateursFavoris))]
    public virtual Annonce Annonce { get; set; } = null!;
    
    [ForeignKey(nameof(UtilisateurId))]
    [InverseProperty(nameof(Utilisateur.AnnoncesFavorites))]
    public virtual Utilisateur Utilisateur { get; set; } = null!;
}