    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Detection;

public class DetectionResultDTO
{
    public bool IsDangerous { get; set; }
    public float? DangerAccuracy { get; set; }
    public bool IsTextile { get; set; }
    public float? TextileAccuracy { get; set; }

    // Error handling
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
