using API.DTO;
using API.Models.EntityFramework;

namespace API.Services;

public interface IPhotoService
{
    Task<Photo?> GetPhotoAsync(int id);
    Task<Photo> UploadPhotoAnnonceAsync(PhotoDTO photoDto, int annonceId);
    Task<Photo> UploadComptePhotoAsync(PhotoDTO photoDto, int compteId);
    Task DeletePhotoAsync(int id);
}