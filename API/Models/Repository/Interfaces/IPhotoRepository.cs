using Shared.DTO.Photo;
using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public interface IPhotoRepository : IDataRepository<Photo, int>
{
    Task<Photo?> GetByIdWithRelationsAsync(int id);
    Task<PhotoResponseDTO> AddPhotoAsync(PhotoUploadDTO photoDto);
    Task<List<Photo>> GetAllPhotosByAnnonceID(int AnnonceID);

}