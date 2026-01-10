using FrontBlazor.Services.Interfaces;
using Shared.DTO.Decision;
using Shared.DTO.Detection;
using Shared.DTO.Photo;
using System.Net.Http.Json;
using FrontBlazor.Services.GenericService;
namespace FrontBlazor.Services;

public class DetectionWebService: BaseGenericService, IDetectionService
{
    public DetectionWebService(HttpClient httpClient) : base(httpClient) { }
    public async Task<DetectionResultDTO> DetectImageDanger(PhotoUploadDTO listPhotoAnnonce)
    {
        // base64 -> bytes
        var base64 = listPhotoAnnonce.Base64Data;
        var commaIndex = base64.IndexOf(',');
        if (commaIndex >= 0) base64 = base64[(commaIndex + 1)..];

        byte[] bytes = Convert.FromBase64String(base64);

        using var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

        form.Add(fileContent, "file", listPhotoAnnonce.FileName ?? "upload.jpg");

        var result = await PostWithCredentialsAsync("Medias/detectPhotoDanger", form);
        result.EnsureSuccessStatusCode();

        var detectionResult = await result.Content.ReadFromJsonAsync<DetectionResultDTO>();
        return detectionResult!;
    }
}
