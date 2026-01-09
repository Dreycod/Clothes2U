using Shared.DTO.Photo;

namespace FrontBlazor.Services.Interfaces
{
    public interface IMediasService
    {
        Task<PhotoResponseDTO> GetPhotoAsync(int id);
        Task<bool> UploadPhotoAnnonceAsync(int annonceId, byte[] imageBytes, string fileName); // pas sur pour le type de retour
        Task<bool> UploadPhotoCompteAsync(int compteId, byte[] imageBytes, string fileName); // idem
        Task<bool> UploadMultiplePhotosAnnonceAsync(int annonceId, List<string> photosDataUrls);
        Task<bool> UploadPhotoMessageAsync(int messageId, byte[] imageBytes, string fileName);
        string GetPhotoUrl(int id);
    }
}