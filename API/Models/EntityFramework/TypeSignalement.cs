using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_signalement_type_sigtyp")]
public class TypeSignalement
{
    [Key]
    [Column("sigtype_id")]
    public int SignalementTypeId { get; set; }
    
    [Column("sigtyp_libelle_type")]
    public string SignalementTypeLibelle { get; set; } = null!;
    
    //relation avec les autres  tables : 
    
    [InverseProperty(nameof(Signalement.TypeSignalement))]
    public virtual ICollection<Signalement> Signalements { get; set; } = new List<Signalement>();
}