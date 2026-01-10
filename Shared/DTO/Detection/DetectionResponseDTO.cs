using System.Text.Json.Serialization;

namespace Shared.DTO.Detection;

public class DetectionResponseDTO
{
    [JsonPropertyName("filename")]
    public string FileName { get; set; }
    [JsonPropertyName("danger_accuracy")]
    public float? DangerAccuracy { get; set; }

    [JsonPropertyName("isDangerous")]
    public bool IsDangerous { get; set; }
    [JsonPropertyName("textile_accuracy")]
    public float? TextileAccuracy { get; set; }

    [JsonPropertyName("isTextile")]
    public bool IsTextile { get; set; }
}
