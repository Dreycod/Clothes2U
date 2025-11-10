using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_utilisateur_uti")]
public class Utilisateur
{
    [Key]
    [Column("uti_id")]
    public int UtilisateurId { get; set; }
    
    [Column("uti_email")]
    [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide.")]
    public string Email { get; set; }
    
    [Column("uti_login")]
    public string Login { get; set; }
    
    [Column("uti_password")]
    public string Password { get; set; }
    
    [Column("uti_dateinscription")]
    public DateTime Dateinscription { get; set; }
    
    [Column("uti_description")]
    public string Description { get; set; }
    
    //id de relation
    
    [Column("uti_adresse_id")]
    public int AdresseId { get; set; }
    
    [Column("uti_statut_id")]
    public int StatutId { get; set; }
    
    
    //relation avec les autres tables : 
    
    [InverseProperty(nameof(Annonce.Utilisateur))]
    public virtual ICollection<Annonce> Annonces { get; set; } = new List<Annonce>();
    
    [InverseProperty(nameof(Bloque.UtilisateurBloqueur))]
    public virtual ICollection<Bloque> UtilisateursBloques { get; set; } = new List<Bloque>();

    [InverseProperty(nameof(Bloque.UtilisateurBloque))]
    public virtual ICollection<Bloque> BloqueParUtilisateurs { get; set; } = new List<Bloque>();
    
    [InverseProperty(nameof(Message.Utilisateur))]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    
    [ForeignKey(nameof(AdresseId))]
    [InverseProperty(nameof(Adresse.Utilisateurs))]
    public virtual Adresse Adresse { get; set; } = null!;
    
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
    
    [InverseProperty(nameof(Decision_suspension.UtilisateurSuspendu))]
    public virtual ICollection<Decision_suspension> LesSuspensions { get; set; } = new List<Decision_suspension>();
    
    [InverseProperty(nameof(Decision_suspension.Decisionnaire))]
    public virtual ICollection<Decision_suspension> LesDecisions { get; set; } = new List<Decision_suspension>();
    
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
}