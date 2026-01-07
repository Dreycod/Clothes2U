using System.Text.Json.Serialization;

namespace Shared.DTO.Detection;

public class DetectionResponseDTO
{
    [JsonPropertyName("accuracy")]
    public int Accuracy { get; set; }

    [JsonPropertyName("isDangerous")]
    public bool IsDangerous { get; set; }
}
