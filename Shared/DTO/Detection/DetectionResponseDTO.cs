using System.Text.Json.Serialization;

namespace Shared.DTO.Detection;

public class DetectionResponseDTO
{
    [JsonPropertyName("filename")]
    public string FileName { get; set; }
    [JsonPropertyName("accuracy")]
    public float? Accuracy { get; set; }

    [JsonPropertyName("isDangerous")]
    public bool IsDangerous { get; set; }
}
