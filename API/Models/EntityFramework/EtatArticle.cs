using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_etatarticle_etaart")]
public class EtatArticle
{
    [Key]
    [Column("etaart_id")]
    public int EtatArticleId { get; set; }
    
    [Column("etaart_nometat")]
    public String NomEtat { get; set; } = null!;
    
    //relation avec la table annonce :
    
    
    [InverseProperty(nameof(Annonce.Etat))]
    public virtual ICollection<Annonce> Annonces { get; set; } = new List<Annonce>();
}