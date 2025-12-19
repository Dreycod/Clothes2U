using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Photo;
using API.Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly ILogger<PhotosController> _logger;

        public PhotosController(IPhotoService photoService, ILogger<PhotosController> logger)
        {
            _photoService = photoService;
            _logger = logger;
        }

        /// <summary>
        /// Upload de photos via formulaire (multipart/form-data)
        /// Utilisé quand le client envoie des fichiers directement
        /// </summary>
        [HttpPost("upload")]
        [RequestSizeLimit(10_485_760)] // 10 MB
        public async Task<ActionResult<List<PhotoResponseDTO>>> UploadPhotos(
            [FromForm] int annonceId,
            [FromForm] List<IFormFile> files)
        {
            if (files == null || !files.Any())
            {
                return BadRequest("Aucune photo fournie");
            }

            if (files.Count > 5)
            {
                return BadRequest("Maximum 5 photos autorisées");
            }

            try
            {
                var uploadedPhotos = new List<PhotoResponseDTO>();

                foreach (var file in files)
                {
                    // Validation
                    if (file.Length == 0)
                    {
                        _logger.LogWarning("Fichier vide ignoré: {FileName}", file.FileName);
                        continue;
                    }

                    if (file.Length > 5_242_880) // 5 MB
                    {
                        return BadRequest($"Le fichier {file.FileName} est trop volumineux (max 5MB)");
                    }

                    if (!file.ContentType.StartsWith("image/"))
                    {
                        return BadRequest($"Le fichier {file.FileName} n'est pas une image");
                    }

                    // Convertir IFormFile en DTO
                    var photoDto = await ConvertFormFileToDTO(file);

                    // Sauvegarder via le service
                    var savedPhoto = await _photoService.SavePhotoAsync(annonceId, photoDto);

                    if (savedPhoto != null)
                    {
                        uploadedPhotos.Add(savedPhoto);
                    }
                }

                return Ok(uploadedPhotos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'upload des photos");
                return StatusCode(500, "Erreur lors de l'upload");
            }
        }

        /// <summary>
        /// Upload de photos via Base64 (depuis Blazor)
        /// Le client envoie directement les DTOs avec Base64
        /// </summary>
        [HttpPost("upload-base64")]
        public async Task<ActionResult<List<PhotoResponseDTO>>> UploadPhotosBase64(
            [FromBody] AnnoncePhotoDTO request)
        {
            if (request.Photos == null || !request.Photos.Any())
            {
                return BadRequest("Aucune photo fournie");
            }

            if (request.Photos.Count > 5)
            {
                return BadRequest("Maximum 5 photos autorisées");
            }

            try
            {
                var uploadedPhotos = new List<PhotoResponseDTO>();

                foreach (var photoDto in request.Photos)
                {
                    // Validation
                    if (string.IsNullOrWhiteSpace(photoDto.Base64Data))
                    {
                        _logger.LogWarning("Photo Base64 vide ignorée");
                        continue;
                    }

                    if (photoDto.FileSize > 5_242_880) // 5 MB
                    {
                        return BadRequest($"La photo {photoDto.FileName} est trop volumineuse (max 5MB)");
                    }

                    // Sauvegarder via le service
                    var savedPhoto = await _photoService.SavePhotoAsync(request.AnnonceId, photoDto);

                    if (savedPhoto != null)
                    {
                        uploadedPhotos.Add(savedPhoto);
                    }
                }

                return Ok(uploadedPhotos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'upload Base64");
                return StatusCode(500, "Erreur lors de l'upload");
            }
        }

        /// <summary>
        /// Supprime une photo
        /// </summary>
        [HttpDelete("{photoId}")]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            try
            {
                var success = await _photoService.DeletePhotoAsync(photoId);

                if (!success)
                {
                    return NotFound($"Photo {photoId} introuvable");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la photo {PhotoId}", photoId);
                return StatusCode(500, "Erreur lors de la suppression");
            }
        }

        /// <summary>
        /// Convertit un IFormFile en PhotoUploadDTO
        /// </summary>
        private async Task<PhotoUploadDTO> ConvertFormFileToDTO(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            var base64 = Convert.ToBase64String(bytes);

            return new PhotoUploadDTO
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Base64Data = base64,
                FileSize = file.Length
            };
        }
    }
}