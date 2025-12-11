using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Attributes;

namespace API.Models.EntityFramework
{
    [Table("t_e_genre_gen")]
    public class Genre : IEntity
    {
        [Key]
        [Column("gen_id")]
        public int GenreId { get; set; }

        [Column("gen_nomgenre")]
        public string? NomGenre { get; set; }

        [InverseProperty(nameof(Annonce.GenreAnnonce))]
        public virtual ICollection<Annonce> Annonces { get; set; } = new List<Annonce>();

        public int GetId() => GenreId;

    }
}
