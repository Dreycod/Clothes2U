using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models.EntityFramework;


[Table("t_e_annonce_preference_utilisateur_annprefuti")]
public class AnnoncePreferenceUtilisateur: IEntity
{
    [Key]
    [Column("annprefuti_id")]
    public int AnnoncePreferenceUtilisateurId { get; set; }
    
    [Column("annprefuti_marque")]
    public string NomMarque { get; set; }
    
    [Column("annprefuti_categorie")]
    public string Categorie { get; set; }
    
    [Column("annprefuti_souscategorie")]
    public string SousCategorie { get; set; }
    
    [Column("annprefuti_prix")]
    public double Prix { get; set; }
    
    [Column("annprefuti_taille")]
    public string Taille { get; set; }
    
    [Column("annprefuti_etat")]
    public string EtatArticle { get; set; }
    
    [Column("annprefuti_ponderarion")]
    public int Ponderation { get; set; }
    
    [Column("annprefuti_couleur_dominante")]
    public string CouleurDominante { get; set; }
    
    //relation avec les autres tables : 
    
    [Column("annprefuti_utilisateur_id")]
    public int UtilisateurId { get; set; }
    
    [ForeignKey(nameof(UtilisateurId))]
    [InverseProperty(nameof(Utilisateur.AnnoncesPreferences))]
    public virtual Utilisateur Utilisateur { get; set; } = null!;

    public int GetId() => AnnoncePreferenceUtilisateurId;
}