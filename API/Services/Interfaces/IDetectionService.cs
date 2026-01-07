using API.Models.EntityFramework;
using Shared.DTO.Photo;
using Shared.DTO.Detection;
namespace API.Services;
public interface IDetectionService
{
    Task<DetectionResultDTO> DetectImageDanger(PhotoUploadDTO listPhotoAnnonce);
}
