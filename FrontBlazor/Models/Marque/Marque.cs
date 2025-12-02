using System.ComponentModel.DataAnnotations.Schema;

namespace FrontBlazor.Models
{
    public class Marque
    {
        public int MarqueId { get; set; }
        public string NomMarque { get; set; } = null!;
        public int GetId()
        {
            return MarqueId;
        }
    }
}
