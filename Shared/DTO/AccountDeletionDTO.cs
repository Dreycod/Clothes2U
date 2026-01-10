using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public class AccountDeletionDTO
    {
        [Required]
        public string? Password { get; set; }
    }
}
