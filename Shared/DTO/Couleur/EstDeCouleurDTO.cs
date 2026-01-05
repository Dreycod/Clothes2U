using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Couleur
{
    public class EstDeCouleurDTO
    {
        public int EstDeCouleurId { get; set; }
        public int CouleurId { get; set; }
        public int AnnonceId { get; set; }
        public int GetId()
        {
            return EstDeCouleurId;
        }
    }

    public class CreateEstDeCouleurDTO
    {
        public int CouleurId { get; set; }
        public int AnnonceId { get; set; }
    }
}
