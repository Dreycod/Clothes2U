using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Utilisateur
{
    public class UtilisateurSettingsDTO
    {
        public int? UtilisateurId { get; set; }
        public string? Login { get; set; }
        public string? Description { get; set; }
        public string? Email { get; set; }
        public int? PhotoProfilId { get; set; }
        public string? Telephone { get; set; }
        public bool? ValidEmail { get; set; }
    }                   
}
