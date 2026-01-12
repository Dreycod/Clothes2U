using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Moderation;

public class ValidationResponseDTO
{
    public bool IsValid { get; set; }
    public int PhotoId { get; set; }    
    public bool Success { get; set; }
}
