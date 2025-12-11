using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_mot_interdit_motint")]
public class MotInterdit : IEntity
{
    [Key]
    [Column("motint_id")]
    public int MotinterditId { get; set; }
    
    [Column("motint_libelle_mot")]
    public string LibelleMot { get; set; }

    public int GetId() => MotinterditId;
}