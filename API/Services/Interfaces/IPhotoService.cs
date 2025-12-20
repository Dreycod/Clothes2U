using Shared.DTO.Photo;
using API.Models.EntityFramework;

namespace API.Services;

public interface IPhotoService
{
    // R�cup�ration d'une photo
    Task<Photo?> GetPhotoAsync(int id);

    // Nouvelles m�thodes avec les nouveaux DTOs
    Task<PhotoResponseDTO> SavePhotoAsync(int annonceId, PhotoUploadDTO photoDto);
    Task<PhotoResponseDTO> SaveComptePhotoAsync(int compteId, PhotoUploadDTO photoDto);
    Task<bool> DeletePhotoAsync(int id);
    Task<Photo> UploadMessagePhotoAsync(PhotoDTO photoDto, int messageId);
}