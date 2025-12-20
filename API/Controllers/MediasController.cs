using Shared.DTO.Photo;
using API.Exceptions;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MediasController : ControllerBase
{
    private readonly IPhotoService _photoService;
    private readonly ILogger<MediasController> _logger;

    public MediasController(IPhotoService photoService, ILogger<MediasController> logger)
    {
        _photoService = photoService;
        _logger = logger;
    }

    [HttpGet("Photos/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPhotos(int id)
    {
        try
        {
            var photo = await _photoService.GetPhotoAsync(id);
            if (photo == null)
            {
                return NotFound($"Photo {id} introuvable");
            }
            return File(photo.Image, "image/jpeg");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de la photo {PhotoId}", id);
            return StatusCode(500, new { message = "Erreur lors de la récupération de la photo", error = ex.Message });
        }
    }

    /// <summary>
    /// Upload d'une photo pour une annonce (multipart/form-data)
    /// </summary>
    [HttpPost("uploadPhotoAnnonce/{annonceId}")]
    [RequestSizeLimit(5_242_880)] // 5 MB
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(PhotoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadPhotoAnnonce(int annonceId, IFormFile file)
    {
        _logger.LogInformation("Upload photo pour annonce {AnnonceId}", annonceId);

        if (file == null || file.Length == 0)
        {
            return BadRequest("Fichier requis");
        }

        if (file.Length > 5_242_880) // 5 MB
        {
            return BadRequest("Le fichier est trop volumineux (max 5MB)");
        }

        if (!file.ContentType.StartsWith("image/"))
        {
            return BadRequest("Le fichier doit être une image");
        }

        try
        {
            // Convertir IFormFile en PhotoUploadDTO
            var photoDto = await ConvertFormFileToDTO(file);

            // Sauvegarder via le service
            var savedPhoto = await _photoService.SavePhotoAsync(annonceId, photoDto);

            return Ok(savedPhoto);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Annonce {AnnonceId} introuvable", annonceId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'upload pour annonce {AnnonceId}", annonceId);
            return StatusCode(500, new { message = "Erreur lors de l'upload de la photo", error = ex.Message });
        }
    }

    /// <summary>
    /// Upload d'une photo de profil pour un compte
    /// </summary>
    [HttpPost("uploadComptePhoto/{compteId}")]
    [RequestSizeLimit(5_242_880)] // 5 MB
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(PhotoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadComptePhoto(int compteId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "Fichier requis" });
        }

        if (file.Length > 5_242_880) // 5 MB
        {
            return BadRequest("Le fichier est trop volumineux (max 5MB)");
        }

        if (!file.ContentType.StartsWith("image/"))
        {
            return BadRequest("Le fichier doit être une image");
        }

        try
        {
            // Convertir IFormFile en PhotoUploadDTO
            var photoDto = await ConvertFormFileToDTO(file);

            // Sauvegarder via le service
            var savedPhoto = await _photoService.SaveComptePhotoAsync(compteId, photoDto);

            return Ok(savedPhoto);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'upload pour compte {CompteId}", compteId);
            return StatusCode(500, new { message = "Erreur lors de l'upload de la photo", error = ex.Message });
        }
    }
    
    [HttpPost("uploadMessagePhoto/{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadMessagePhoto([FromForm] PhotoUploadDTO photoDto, int messageId)
    {
        if (photoDto?.Base64Data == null)
        {
            return BadRequest(new { message = "Fichier requis" });
        }

        try
        {
            var photo = await _photoService.UploadMessagePhotoAsync(photoDto, messageId);
            return File(photo.Url, "image/jpeg");
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur lors de l'upload de la photo", error = ex.Message });
        }
    }

    [HttpDelete("Photos/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePhoto(int id)
    {
        try
        {
            var success = await _photoService.DeletePhotoAsync(id);

            if (!success)
            {
                return NotFound($"Photo {id} introuvable");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression de la photo {PhotoId}", id);
            return StatusCode(500, new { message = "Erreur lors de la suppression de la photo", error = ex.Message });
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