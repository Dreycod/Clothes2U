using System.ComponentModel.DataAnnotations.Schema;

namespace FrontBlazor.Models
{
    public class Marque: IEntity
    {
        public int MarqueId { get; set; }
        public string NomMarque { get; set; } = null!;

        public int? NombreProduits { get; set; }

        public int GetId()
        {
            return MarqueId;
        }
    }
}
