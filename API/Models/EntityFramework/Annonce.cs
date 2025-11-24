using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Attributes;

namespace API.Models.EntityFramework;


[Table("t_e_annonce_ann")]
public class Annonce : IEntity
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
    [NavigationProperty]
    public virtual Utilisateur Utilisateur { get; set; } = null!;
    
    [ForeignKey(nameof(EtatId))]
    [InverseProperty(nameof(EtatArticle.Annonces))]
    [NavigationProperty]
    public virtual EtatArticle Etat{ get; set; } = null!;
    
    [InverseProperty(nameof(Est_De_Couleur.Annonce))]
    [NavigationProperty]
    public virtual ICollection<Est_De_Couleur> Couleurs { get; set; } = new List<Est_De_Couleur>();
    
    [ForeignKey(nameof(MarqueId))]
    [InverseProperty(nameof(Marque.Annonces))]
    [NavigationProperty]
    public virtual Marque Marque { get; set; } = null!;
    
    [ForeignKey(nameof(TailleId))]
    [InverseProperty(nameof(Taille.Annonces))]
    [NavigationProperty]
    public virtual Taille Taille { get; set; } = null!;
    
    [ForeignKey(nameof(SousCategorieId))]
    [InverseProperty(nameof(SousCategorie.Annonces))]
    [NavigationProperty]
    public virtual SousCategorie SousCategorie { get; set; } = null!;
    
    [ForeignKey(nameof(CategorieId))]
    [InverseProperty(nameof(Categorie.Annonces))]
    [NavigationProperty]
    public virtual Categorie Categorie { get; set; } = null!;
    
    [ForeignKey(nameof(StatutAnnonceId))]
    [InverseProperty(nameof(StatutAnnonce.Annonces))]
    public virtual StatutAnnonce Statut { get; set; } = null!;
    
    [InverseProperty(nameof(Illustre_Annonce.Annonce))]
    [NavigationProperty]
    public virtual ICollection<Illustre_Annonce> Photos { get; set; } = new List<Illustre_Annonce>();
    
    [InverseProperty(nameof(Decision_suspension.AnnonceSuspendu))]
    public virtual ICollection<Decision_suspension> Decisions { get; set; } = new List<Decision_suspension>();
    
    [InverseProperty(nameof(Favoris.Annonce))]
    [NavigationProperty]
    public virtual ICollection<Favoris> UtilisateursFavoris { get; set; } = new List<Favoris>();
    
    [InverseProperty(nameof(NotificationNouvelleAnnonce.Annonce))]
    public virtual ICollection<NotificationNouvelleAnnonce> NotificationsNouvelleAnnonces { get; set; } = new List<NotificationNouvelleAnnonce>();
    
    [InverseProperty(nameof(NotificationModificationAnnonce.Annonce))]
    public virtual ICollection<NotificationModificationAnnonce> NotificationsModificationAnnonces { get; set; } = new List<NotificationModificationAnnonce>();
    
    [InverseProperty(nameof(SignalementAnnonce.Annonce))]
    public virtual ICollection<SignalementAnnonce> Signalements { get; set; } = new List<SignalementAnnonce>();
    
    [InverseProperty(nameof(Conversation.LAnnonce))]
    public virtual ICollection<Conversation> LesConversations { get; set; } = new List<Conversation>();
    
    [InverseProperty(nameof(Recense.Annonce))]
    [NavigationProperty]
    public virtual ICollection<Recense> Tags { get; set; } = new List<Recense>();
    
    public int GetId() => AnnonceId;
}