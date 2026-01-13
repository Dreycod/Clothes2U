using API.Models.EntityFramework;
using Shared.DTO.Moderation;
using Shared.DTO.Photo;

namespace API.Services;

public interface IPhotoService
{
    // R�cup�ration d'une photo
    Task<Photo?> GetPhotoAsync(int id);

    // Nouvelles m�thodes avec les nouveaux DTOs
    Task<PhotoResponseDTO> SavePhotoAsync(int annonceId, PhotoUploadDTO photoDto);
    Task<List<PhotoDTO>> GetAllPhotosValidation();

    Task<ValidationResponseDTO> ValidationImageAsync(bool Reponse, int Photoid);
    Task<PhotoResponseDTO> SaveComptePhotoAsync(int compteId, PhotoUploadDTO photoDto);
    Task<PhotoResponseDTO> UploadMessagePhotoAsync(PhotoUploadDTO photoDto);
    Task<bool> DeletePhotoAsync(int id);
    Task<bool> DeletePhotosAnnonceAsync(int AnnonceID);
}