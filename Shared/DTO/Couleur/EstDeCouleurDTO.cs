using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Couleur
{
    public class EstDeCouleurDTO
    {
        public int CouleurId { get; set; }
        public int AnnonceId { get; set; }
        public int GetId()
        {
            return CouleurId;
        }
    }
}
