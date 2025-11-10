using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_message_texte_mestxt")]
public class MessageTexte : Message
{
    [Column("mextxt_contenue_message")]
    public string ContenuMessage { get; set; }
    
    [InverseProperty(nameof(Photo.MessageTexte))]
    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
}