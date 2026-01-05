using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.ForgotPassword
{
    public class ResetPasswordDTO
    {
        [Required]
        public string? Token { get; set; }

        [Required]
        [MinLength(8)]
        public string? NewPassword { get; set; }
    }
}
