using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTO.ForgotPassword
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
