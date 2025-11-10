using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_annonce_ann")]
public class Annonce
{
    [Key]
    [Column("ann_id")]
    public int AnnonceId { get; set; }
    
    [Column("ann_titre")]
    public string Title { get; set; }
    
    [Column("ann_dateannonce")]
    public DateTime DateAnnonce { get; set; }
    
    [Column("ann_negociable")]
    public bool Negociable { get; set; }
    
    [Column("ann_prix")]
    public decimal Prix { get; set; }
    
    //id des tables externes
    
    [Column("ann_utilisateur_id")]
    public int UtilisateurId { get; set; }
    
    [Column("ann_etat_id")]
    public int EtatId { get; set; }
    
    [Column("ann_couleur_id")]
    public int CouleurId { get; set; }
    
    [Column("ann_marque_id")]
    public int MarqueId { get; set; }
    
    [Column("ann_taille_id")]
    public int TailleId { get; set; }
    
    [Column("ann_sous_categorie_id")]
    public int SousCategorieId { get; set; }
    
    [Column("ann_categorie_id")]
    public int CategorieId { get; set; }
    
    [Column("ann_statut_annonce_id")]
    public int StatutAnnonceId { get; set; }
    
    //relation avec les autres tables : 
    
    [ForeignKey(nameof(UtilisateurId))]
    [InverseProperty(nameof(Utilisateur.Annonces))]
    public virtual Utilisateur Utilisateur { get; set; } = null!;
    
    [ForeignKey(nameof(EtatId))]
    [InverseProperty(nameof(EtatArticle.Annonces))]
    public virtual EtatArticle Etat{ get; set; } = null!;
    
    [ForeignKey(nameof(CouleurId))]
    [InverseProperty(nameof(Couleur.Annonces))]
    public virtual Couleur Couleur { get; set; } = null!;
    
    [ForeignKey(nameof(MarqueId))]
    [InverseProperty(nameof(Marque.Annonces))]
    public virtual Marque Marque { get; set; } = null!;
    
    [ForeignKey(nameof(TailleId))]
    [InverseProperty(nameof(Taille.Annonces))]
    public virtual Taille Taille { get; set; } = null!;
    
    [ForeignKey(nameof(SousCategorieId))]
    [InverseProperty(nameof(SousCategorie.Annonces))]
    public virtual SousCategorie SousCategorie { get; set; } = null!;
    
    [ForeignKey(nameof(CategorieId))]
    [InverseProperty(nameof(Categorie.Annonces))]
    public virtual SousCategorie Categorie { get; set; } = null!;
    
    [ForeignKey(nameof(StatutAnnonceId))]
    [InverseProperty(nameof(StatutAnnonce.Annonces))]
    public virtual StatutUtilisateur Statut { get; set; } = null!;
    
    [InverseProperty(nameof(Illustre_Annonce.Annonce))]
    public virtual ICollection<Illustre_Annonce> Photos { get; set; } = new List<Illustre_Annonce>();
    
    [InverseProperty(nameof(Favoris.Annonce))]
    public virtual ICollection<Favoris> UtilisateursFavoris { get; set; } = new List<Favoris>();
    
    [InverseProperty(nameof(NotificationNouvelleAnnonce.Annonce))]
    public virtual ICollection<NotificationNouvelleAnnonce> NotificationsNouvelleAnnonces { get; set; } = new List<NotificationNouvelleAnnonce>();
    
    [InverseProperty(nameof(NotificationModificationAnnonce.Annonce))]
    public virtual ICollection<NotificationModificationAnnonce> NotificationsModificationAnnonces { get; set; } = new List<NotificationModificationAnnonce>();
}