using Shared.DTO.Photo;

namespace FrontBlazor.Services.GenericIServices
{
    public interface IMediasService<TEntity>
    {
        Task<TEntity> GetPhotoAsync(int id);
        Task<bool> UploadPhotoAnnonceAsync(int annonceId, byte[] imageBytes, string fileName);
        Task<bool> UploadPhotoCompteAsync(int compteId, byte[] imageBytes, string fileName);
        Task<bool> UploadMultiplePhotosAnnonceAsync(int annonceId, List<string> photosDataUrls);
        string GetPhotoUrl(int id);
    }
}