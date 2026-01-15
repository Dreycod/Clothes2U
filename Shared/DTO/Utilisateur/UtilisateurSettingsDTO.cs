using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Utilisateur
{
    public class UtilisateurSettingsDTO
    {
        public int UtilisateurId { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Le nom d'utilisateur ne peut contenir que des lettres et des chiffres.")]
        public string? Login { get; set; }
        public string? Description { get; set; }
        public string? Email { get; set; }
        public int? PhotoProfilId { get; set; }
        public string? Telephone { get; set; }
        public bool? ValidEmail { get; set; }
        public bool? PreferenceNotifMail { get; set; }
    }                   
}
