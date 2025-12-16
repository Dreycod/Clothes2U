using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace FrontBlazor.Services;

public class MediaWebService : WritableService<Photo>, IMediasService<Photo>
{
    public MediaWebService(HttpClient httpClient) : base(httpClient) { }

    public async Task<Photo> GetPhotoAsync(int id)
    {
        var response = await GetWithCredentialsAsync($"Medias/Photos/{id}");
        response.EnsureSuccessStatusCode();

        var photo = await response.Content.ReadFromJsonAsync<Photo>();
        return photo ?? new Photo();
    }

    public string GetPhotoUrl(int photoId)
    {
        var baseUrl = _httpClient.BaseAddress?.ToString();
        return $"{baseUrl}Medias/Photos/{photoId}";
    }

    public async Task<bool> UploadPhotoAnnonceAsync(int annonceId, byte[] imageBytes, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(imageBytes);

            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent, "File", fileName);

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

    public async Task<bool> UploadMultiplePhotosAnnonceAsync(int annonceId, List<string> photosDataUrls)
    {
        if (photosDataUrls == null || !photosDataUrls.Any())
            return true;

        bool allSuccess = true;
        int successCount = 0;

        foreach (var photoDataUrl in photosDataUrls)
        {
            try
            {
                // Extraire le type MIME et les données base64
                var parts = photoDataUrl.Split(',');
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
                    $"photo_{Guid.NewGuid()}.jpg"
                );

                if (success)
                {
                    successCount++;
                    Console.WriteLine($"✅ Photo {successCount}/{photosDataUrls.Count} uploadée avec succès");
                }
                else
                {
                    allSuccess = false;
                    Console.WriteLine($"❌ Échec upload photo {successCount + 1}/{photosDataUrls.Count}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la conversion/upload: {ex.Message}");
                allSuccess = false;
            }
        }

        Console.WriteLine($"📊 Résultat: {successCount}/{photosDataUrls.Count} photos uploadées");
        return allSuccess;
    }
}