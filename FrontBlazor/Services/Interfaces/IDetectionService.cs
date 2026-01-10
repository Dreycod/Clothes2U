using Shared.DTO.Detection;
using Shared.DTO.Marque;
using Shared.DTO.Photo;

namespace FrontBlazor.Services.Interfaces;

public interface IDetectionService
{
    Task<DetectionResultDTO> DetectImageDanger(PhotoUploadDTO listPhotoAnnonce);
}
