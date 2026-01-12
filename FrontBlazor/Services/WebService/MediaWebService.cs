using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using Shared.DTO;
using Shared.DTO.Detection;
using Shared.DTO.Moderation;
using Shared.DTO.Photo;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace FrontBlazor.Services;

public class MediaWebService : WritableService<PhotoResponseDTO>, IMediasService
{
    public MediaWebService(HttpClient httpClient) : base(httpClient) { }

    public async Task<PhotoResponseDTO> GetPhotoAsync(int id)
    {
        var response = await GetWithCredentialsAsync($"Medias/Photos/{id}");
        response.EnsureSuccessStatusCode();

        var photo = await response.Content.ReadFromJsonAsync<PhotoResponseDTO>();
        return photo ?? new PhotoResponseDTO();
    }

    public string GetPhotoUrl(int photoId)
    {
        var baseUrl = _httpClient.BaseAddress?.ToString();
        return $"{baseUrl}Medias/Photos/{photoId}" ?? "";
    }
    public async Task<PhotoDTO> GetPhotoDTO(int id)
    {
        var response = await GetWithCredentialsAsync($"Medias/GetPhotoDTO/{id}");
        response.EnsureSuccessStatusCode();

        var photo = await response.Content.ReadFromJsonAsync<PhotoDTO>();
        return photo ?? new PhotoDTO();
    }

    public async Task<bool> UploadPhotoAnnonceAsync(int annonceId, byte[] imageBytes, bool IsDangerous, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();

            var isDangerousValue = IsDangerous ? "true" : "false";
            content.Add(new StringContent(isDangerousValue, Encoding.UTF8, "text/plain"), "IsDangerous");

            using var fileContent = new ByteArrayContent(imageBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent, "File", fileName);

            Console.WriteLine($"Envoi IsDangerous = {isDangerousValue}, fileName = {fileName}, bytes = {imageBytes.Length}");

            var response = await PostWithCredentialsAsync($"Medias/uploadPhotoAnnonce/{annonceId}", content);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur upload photo annonce: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UploadPhotoCompteAsync(int compteId, byte[] imageBytes, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(imageBytes);

            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent, "File", fileName);

            var response = await PostWithCredentialsAsync($"Medias/uploadComptePhoto/{compteId}", content);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur upload photo compte: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UploadMultiplePhotosAnnonceAsync(int annonceId, List<PhotoDataDTO> photoDataDTOs)
    {
        if (photoDataDTOs == null || !photoDataDTOs.Any())
            return true;

        bool allSuccess = true;
        int successCount = 0;

        foreach (var photo in photoDataDTOs)
        {
            try
            {
                // Extraire le type MIME et les données base64
                var parts = photo.PreviewBase64.Split(',');
                if (parts.Length != 2)
                {
                    Console.WriteLine("❌ Format de dataUrl invalide");
                    allSuccess = false;
                    continue;
                }

                var base64Data = parts[1];
                var imageBytes = Convert.FromBase64String(base64Data);

                var success = await UploadPhotoAnnonceAsync(
                    annonceId,
                    imageBytes,
                    photo.IsDangerous,
                    $"photo_{Guid.NewGuid()}.jpg"
                );

                if (success)
                {
                    successCount++;
                    Console.WriteLine($"✅ Photo {successCount}/{photoDataDTOs.Count} uploadée avec succès");
                }
                else
                {
                    allSuccess = false;
                    Console.WriteLine($"❌ Échec upload photo {successCount + 1}/{photoDataDTOs.Count}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la conversion/upload: {ex.Message}");
                allSuccess = false;
            }
        }

        Console.WriteLine($"📊 Résultat: {successCount}/{photoDataDTOs.Count} photos uploadées");
        return allSuccess;
    
    }

    public async Task<bool> UploadPhotoMessageAsync(int messageId, byte[] imageBytes, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(imageBytes);

            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent, "File", fileName);

            var response = await PostWithCredentialsAsync($"Medias/uploadMessagePhoto/{messageId}", content);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur upload photo annonce: {ex.Message}");
            return false;
        }
    }
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

    public async Task<ValidationResponseDTO> ValidationImageAsync(bool Reponse, int Photoid)
    {
        var response = await PostWithCredentialsAsync($"Medias/Validation?Reponse={Reponse}&PhotoID={Photoid}", null);
        response.EnsureSuccessStatusCode();
        var validationResponse = await response.Content.ReadFromJsonAsync<ValidationResponseDTO>();
        return validationResponse!;
    }

    public async Task<List<PhotoDTO>> GetAllPhotosValidation()
    {
        var response =  await GetWithCredentialsAsync("Medias/Photos/GetAllPhotosValidation");
        response.EnsureSuccessStatusCode();
        var photos = await response.Content.ReadFromJsonAsync<List<PhotoDTO>>();
        return photos ?? new List<PhotoDTO>();
    }

}