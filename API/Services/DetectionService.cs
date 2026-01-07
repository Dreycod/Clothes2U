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
        DetectionResultDTO result = new DetectionResultDTO();
        result.Success = false;

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var payload = new
            {
                file = Image.Base64Data
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            _logger.LogInformation("📤 JSON envoyé à Python:\n{Json}", json);
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_fastApiBaseUrl}/image", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("📥 Réponse reçue de Python:\n{Response}", responseContent);
                DetectionResponseDTO? detectionReponse = JsonSerializer.Deserialize<DetectionResponseDTO>(
                    responseContent,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });
                if (detectionReponse != null)
                {
                    result.IsDangerous = detectionReponse.IsDangerous;
                    result.Accuracy = detectionReponse.Accuracy;
                    result.Success = true;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning(
                    "❌ Échec de l'appel Python : {StatusCode}\n{Error}",
                    response.StatusCode,
                    errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ Erreur lors du calcul de la détection d'image");
        }
        return result;
    }
}