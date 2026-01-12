using Shared.DTO.Detection;
using Shared.DTO.Photo;
using Shared.DTO.Moderation;
namespace FrontBlazor.Services.Interfaces
{
    public interface IMediasService
    {
        Task<PhotoResponseDTO> GetPhotoAsync(int id);
        Task<bool> UploadPhotoAnnonceAsync(int annonceId, byte[] imageBytes, bool IsDangerous, string fileName); // pas sur pour le type de retour
        Task<bool> UploadPhotoCompteAsync(int compteId, byte[] imageBytes, string fileName); // idem
        Task<bool> UploadMultiplePhotosAnnonceAsync(int annonceId, List<PhotoDataDTO> photosDataUrls);
        Task<bool> UploadPhotoMessageAsync(int messageId, byte[] imageBytes, string fileName);
        Task<DetectionResultDTO> DetectImageDanger(PhotoUploadDTO listPhotoAnnonce);
        Task<ValidationResponseDTO> ValidationImageAsync(bool Reponse, int Photoid);
        Task<PhotoDTO> GetPhotoDTO(int id);
        Task<List<PhotoDTO>> GetAllPhotosValidation();
        string GetPhotoUrl(int id);
    }
}