using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Mot de passe actuel requis.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nouveau mot de passe requis.")]
        [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Le mot de passe doit contenir une majuscule, une minuscule, un chiffre et un symbole."
        )]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmation requise.")]
        [Compare(nameof(NewPassword), ErrorMessage = "La confirmation du mot de passe ne correspond pas.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
