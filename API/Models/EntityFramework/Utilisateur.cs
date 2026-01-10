using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.DTO.Annonce;

namespace API.Models.EntityFramework;

[Table("t_e_utilisateur_uti")]
public class Utilisateur : IEntity
{
    [Key]
    [Column("uti_id")]
    public int UtilisateurId { get; set; }
    
    [Column("uti_email")]
    [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide.")]
    public string Email { get; set; }
    
    [Column("uti_telephone")]
    [Phone]
    public string? Telephone { get; set; }
    
    [Column("uti_login")]
    public string Login { get; set; }
    
    [Column("uti_password")]
    public string Password { get; set; }
    
    [Column("uti_dateinscription")]
    public DateTime Dateinscription { get; set; }
    
    [Column("uti_description")]
    public string Description { get; set; }
    
    [Column("uti_valid_email")]
    public bool  ValidEmail { get; set; }
    
    [Column("uti_valid_telephone")]
    public bool  ValidTelephone { get; set; }

    [Column("uti_preference_notif_mail")]
    public bool PreferenceNotifMail { get; set; }

    [Column("uti_preference_theme")]
    public bool PreferenceTheme { get; set; }

    [Column("uti_preference_cookies")]
    public bool PreferenceCookies { get; set; }

    [Column("uti_deleted_at")]
    public DateTime? DeletedAt { get; set; }


    //id de relation

    [Column("uti_statut_id")] public int StatutId { get; set; }

    [Column("uti_id_photo")] public int? PhotoId { get; set; }
    
    [Column("uti_role_id")] public int RoleId { get; set; }

    [Column("uti_deleted_by_admin_id")]
    public int? DeletedByAdminId { get; set; }


    //relation avec les autres tables : 
    [ForeignKey(nameof(RoleId))]
    [InverseProperty(nameof(RoleUtilisateur.Utilisateurs))]
    public virtual RoleUtilisateur Role { get; set; } 
    
    
    [ForeignKey(nameof(PhotoId))]
    public virtual Photo PhotoProfil { get; set; }

    [ForeignKey(nameof(DeletedByAdminId))]
    [InverseProperty(nameof(Utilisateur.UtilisateursSupprimes))]
    public virtual Utilisateur? DeletedByAdminNav { get; set; }


    [InverseProperty(nameof(Utilisateur.DeletedByAdminNav))]
    public virtual ICollection<Utilisateur> UtilisateursSupprimes { get; set; } = new List<Utilisateur>();


    [InverseProperty(nameof(Annonce.Utilisateur))]
    public virtual ICollection<Annonce> Annonces { get; set; } = new List<Annonce>();
    
    [InverseProperty(nameof(Bloque.UtilisateurBloqueur))]
    public virtual ICollection<Bloque> UtilisateursBloques { get; set; } = new List<Bloque>();

    [InverseProperty(nameof(Bloque.UtilisateurBloque))]
    public virtual ICollection<Bloque> BloqueParUtilisateurs { get; set; } = new List<Bloque>();
    
    [InverseProperty(nameof(Message.Utilisateur))]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    
    [InverseProperty(nameof(Adresse.Utilisateurs))]
    public virtual ICollection<Adresse> Adresses { get; set; } = new List<Adresse>(); 
    
    [InverseProperty(nameof(NoteUtilisateur.Auteur))]
    public virtual ICollection<NoteUtilisateur> NotesAuteur { get; set; } = new List<NoteUtilisateur>();
    
    [InverseProperty(nameof(NoteUtilisateur.Cible))]
    public virtual ICollection<NoteUtilisateur> NotesCible { get; set; } = new List<NoteUtilisateur>();
    
    [InverseProperty(nameof(Favoris.Utilisateur))]
    public virtual ICollection<Favoris> AnnoncesFavorites { get; set; } = new List<Favoris>();
    
    [InverseProperty(nameof(Abonnement.UtilisateurSuiveur))]
    public virtual ICollection<Abonnement> Abonnements { get; set; } = new List<Abonnement>();
    
    [InverseProperty(nameof(Abonnement.UtilisateurSuivis))]
    public virtual ICollection<Abonnement> Abonnes { get; set; } = new List<Abonnement>();
    
    [ForeignKey(nameof(StatutId))]
    [InverseProperty(nameof(StatutUtilisateur.Utilisateurs))]
    public virtual StatutUtilisateur Statut { get; set; } = null!;
    
    [InverseProperty(nameof(Notification.Utilisateur))]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty(nameof(Signalement.Utilisateur))]
    public virtual ICollection<Signalement> Signalements { get; set; } = new List<Signalement>();
    
    [InverseProperty(nameof(SignalementUtilisateur.UtilisateurSignale))]
    public virtual ICollection<SignalementUtilisateur> SignalementsUtilisateurs { get; set; } = new List<SignalementUtilisateur>();
    
    [InverseProperty(nameof(Achete.UtilisateurAcheteur))]
    public virtual ICollection<Achete> Achats { get; set; } = new List<Achete>();
    [InverseProperty(nameof(Vend.UtilisateurVendeur))]
    public virtual ICollection<Vend> Ventes { get; set; } = new List<Vend>();

    [InverseProperty(nameof(Visualisation.UtilisateurVisu))]
    public virtual ICollection<Visualisation> Visualisations { get; set; } = new List<Visualisation>();
    
    [InverseProperty(nameof(VerificationCode.Utilisateur))]
    public virtual ICollection<VerificationCode> VerificationCodes { get; set; } = new List<VerificationCode>();

    [InverseProperty(nameof(PasswordResetToken.UtilisateurReset))]
    public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    [InverseProperty(nameof(HistoriqueUtilisateur.UserHist))]
    public virtual ICollection<HistoriqueUtilisateur> HistUtilisateurs{ get; set; } = new List<HistoriqueUtilisateur>();

    [InverseProperty(nameof(HistoriqueUtilisateur.Admin))]
    public virtual ICollection<HistoriqueUtilisateur> HistoriquesAdmin { get; set; } = new List<HistoriqueUtilisateur>();

    //moderation
    [InverseProperty(nameof(Decision.Moderateur))]
    public virtual ICollection<Decision> DecisionsModerateur { get; set; } = new List<Decision>();

    [InverseProperty(nameof(Decision.Utilisateur))]
    public virtual ICollection<Decision> DecisionsUtilisateurSanctionne { get; set; } = new List<Decision>();
    
    [InverseProperty(nameof(Commande.Acheteur))]
    public virtual ICollection<Commande> CommandesAchetees { get; set; } = new List<Commande>();

    [InverseProperty(nameof(Commande.Vendeur))]
    public virtual ICollection<Commande> CommandesVendues { get; set; } = new List<Commande>();


    
    //suggestion : 
    
    [InverseProperty(nameof(AnnoncePreferenceUtilisateur.Utilisateur))]
    public virtual ICollection<AnnoncePreferenceUtilisateur> AnnoncesPreferences { get; set; } = new List<AnnoncePreferenceUtilisateur>();
    public int GetId() => UtilisateurId;
}