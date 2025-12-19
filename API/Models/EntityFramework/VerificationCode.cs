using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Enums;

namespace API.Models.EntityFramework;

[Table("t_e_verification_code_ver")]
public class VerificationCode : IEntity
{
    [Key]
    [Column("ver_id")]
    public int VerificationCodeId { get; set; }
    
    [Column("ver_utilisateur_id")]
    public int UtilisateurId { get; set; }
    
    [Column("ver_code")]
    public string Code { get; set; } = null!;
    
    [Column("ver_type")]
    public VerificationType Type { get; set; }
    
    [Column("ver_date_creation")]
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    
    [Column("ver_date_expiration")]
    public DateTime DateExpiration { get; set; }
    
    [Column("ver_est_utilise")]
    public bool EstUtilise { get; set; } = false;
    
    [Column("ver_tentatives")]
    public int Tentatives { get; set; } = 0;
    
    // Relations
    [ForeignKey(nameof(UtilisateurId))]
    [InverseProperty(nameof(Utilisateur.VerificationCodes))]
    public virtual Utilisateur Utilisateur { get; set; } = null!;
    
    public int GetId() => VerificationCodeId;
}