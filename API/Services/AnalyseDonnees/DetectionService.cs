using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.DTO.Photo;
using Shared.DTO.Detection;
using API.Services;

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
        var result = new DetectionResultDTO
        {
            Success = false
        };

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            // 1) Validate Base64
            if (Image == null)
            {
                result.ErrorMessage = "Image DTO null";
                return result;
            }

            if (string.IsNullOrWhiteSpace(Image.Base64Data))
            {
                result.ErrorMessage = "Image Base64 vide";
                return result;
            }

            // 2) Remove data URL prefix if present: "data:image/jpeg;base64,..."
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

            using var form = new MultipartFormDataContent();

            var fileContent = new ByteArrayContent(imageBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // or image/png
            form.Add(fileContent, "file", Image.FileName ?? "upload.jpg");

            // 3) Call FastAPI with graceful handling when it's OFF
            HttpResponseMessage response;
            string responseContent;

            try
            {
                response = await client.PostAsync($"{_fastApiBaseUrl}/image", form);
                responseContent = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                // Connection refused / DNS / network issues
                _logger.LogWarning(ex, "❌ FastAPI unreachable at {Url}", _fastApiBaseUrl);
                result.ErrorMessage = "Service de détection indisponible (FastAPI unreachable).";
                return result;
            }
            catch (TaskCanceledException ex)
            {
                // Timeout (or cancellation). With HttpClient.Timeout, this is the common timeout path.
                _logger.LogWarning(ex, "⏱️ FastAPI timeout at {Url}", _fastApiBaseUrl);
                result.ErrorMessage = "Service de détection indisponible (timeout).";
                return result;
            }

            _logger.LogInformation(
                "📥 Réponse FastAPI ({StatusCode}) : {Response}",
                response.StatusCode,
                responseContent
            );

            if (!response.IsSuccessStatusCode)
            {
                // FastAPI responded, but with an error code
                result.ErrorMessage = string.IsNullOrWhiteSpace(responseContent)
                    ? $"FastAPI error: {(int)response.StatusCode} {response.ReasonPhrase}"
                    : responseContent;

                return result;
            }

            // 4) Deserialize response
            DetectionResponseDTO? detectionResponse;
            try
            {
                detectionResponse = JsonSerializer.Deserialize<DetectionResponseDTO>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "❌ Invalid JSON returned by FastAPI: {Response}", responseContent);
                result.ErrorMessage = "Réponse FastAPI invalide (JSON).";
                return result;
            }

            if (detectionResponse == null)
            {
                result.ErrorMessage = "Réponse FastAPI invalide";
                return result;
            }

            // 5) Map to result DTO
            result.IsDangerous = detectionResponse.IsDangerous;
            result.DangerAccuracy = detectionResponse.DangerAccuracy;
            result.IsTextile = detectionResponse.IsTextile;
            result.TextileAccuracy = detectionResponse.TextileAccuracy;
            result.Success = true;

            return result;
        }
        catch (Exception ex)
        {
            // Any other unexpected error in your service code
            _logger.LogError(ex, "❌ Erreur lors du calcul de la détection d'image");
            result.ErrorMessage = "Erreur interne lors de la détection d'image.";
            return result;
        }
    }
}
