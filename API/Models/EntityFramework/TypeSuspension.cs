using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_type_suspension_tsu")]
public class TypeSuspension
{
    [Key]
    [Column("tsu_id")]
    public int TypeSuspensionId { get; set; }
    
    [Column("tsu_nomtypesuspension")]
    public string NomTypeSuspension { get; set; }
    
    //relation avec la table suspension :
    [InverseProperty(nameof(Decision_suspension.TypeSuspension))]
    public virtual ICollection<Decision_suspension> Decision_suspensions { get; set; } = new List<Decision_suspension>();
}