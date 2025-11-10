using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_note_utilisateur_notuti")]
public class NoteUtilisateur
{
    [Key]
    [Column("notuti_id")]
    public int NoteUtilisateurId { get; set; }
    
    [Column("notuti_note")]
    [Range(0, 5, ErrorMessage = "La note doit être comprise entre 0 et 5.")]
    public int Note { get; set; }
    
    [Column("notuti_commentaire")]
    [MaxLength(200)]
    public string Commentaire { get; set; }
    
    [Column("notuti_date")]
    public DateTime DatePublication { get; set; }
    
    //id pour les relations
    [Column("notuti_noteur_id")]
    public int NoteurId { get; set; }
    
    [Column("notuti_note_id")]
    public int NoteId { get; set; }
    
    //relation avec les autres tables
    
    [ForeignKey(nameof(NoteurId))]
    [InverseProperty(nameof(Utilisateur.NotesAuteur))]
    public virtual Utilisateur Auteur { get; set; } = null!;
    
    [ForeignKey(nameof(NoteId))]
    [InverseProperty(nameof(Utilisateur.NotesCible))]
    public virtual Utilisateur Cible { get; set; } = null!;
    
    [InverseProperty(nameof(SignalementAvis.Avis))]
    public virtual ICollection<SignalementAvis> Signalements { get; set; } = new List<SignalementAvis>();

    
}