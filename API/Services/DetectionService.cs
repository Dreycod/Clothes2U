using System.Text;
using System.Text.Json;
using Shared.DTO.Annonce;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Shared.DTO.Photo;
using Shared.DTO.Detection;

public class DetectionService : IDetectionService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DetectionService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly string _fastApiBaseUrl;
    public DetectionService(
        IHttpClientFactory httpClientFactory,
        ILogger<DetectionService> logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _fastApiBaseUrl = configuration["FastApi:BaseUrl"] ?? "http://localhost:8001";
    }

    public async Task<DetectionResultDTO> DetectImageDanger(PhotoUploadDTO Image)
    {
        DetectionResultDTO result = new DetectionResultDTO
        {
            Success = false
        };

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            // 1) Sécuriser et nettoyer le base64
            if (string.IsNullOrWhiteSpace(Image.Base64Data))
            {
                result.ErrorMessage = "Image Base64 vide";
                return result;
            }

            var base64 = Image.Base64Data;
            var commaIndex = base64.IndexOf(',');
            if (commaIndex >= 0)
                base64 = base64[(commaIndex + 1)..];

            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(base64);
            }
            catch
            {
                result.ErrorMessage = "Base64 invalide";
                return result;
            }

            // 2) Construire le multipart/form-data attendu par FastAPI
            using var form = new MultipartFormDataContent();

            var fileContent = new ByteArrayContent(imageBytes);
            fileContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg"); // ou png

            // ⚠️ le nom "file" DOIT matcher UploadFile = File(...)
            form.Add(
                fileContent,
                "file",
                Image.FileName ?? "upload.jpg"
            );

            // 3) Appel vers FastAPI
            var response = await client.PostAsync($"{_fastApiBaseUrl}/image", form);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation(
                "📥 Réponse FastAPI ({StatusCode}) : {Response}",
                response.StatusCode,
                responseContent
            );

            if (!response.IsSuccessStatusCode)
            {
                result.ErrorMessage = responseContent;
                return result;
            }

            // 4) Désérialisation de la réponse
            DetectionResponseDTO? detectionResponse =
                JsonSerializer.Deserialize<DetectionResponseDTO>(
                    responseContent,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (detectionResponse == null)
            {
                result.ErrorMessage = "Réponse FastAPI invalide";
                return result;
            }

            result.IsDangerous = detectionResponse.IsDangerous;
            result.Accuracy = detectionResponse.Accuracy;
            result.Success = true;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erreur lors du calcul de la détection d'image");
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

}