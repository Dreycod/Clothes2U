using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Moderation
{
    public class CreateAvertissementRequestDTO
    {
        public string MessageAvertissement { get; set; }
        public int UtilisateurId { get; set; }
    }
}
