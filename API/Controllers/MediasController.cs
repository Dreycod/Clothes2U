using API.DTO;
using API.Exceptions;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MediasController : ControllerBase
{
    private readonly IPhotoService _photoService;

    public MediasController(IPhotoService photoService)
    {
        _photoService = photoService;
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
            return StatusCode(500, new { message = "Erreur lors de la récupération de la photo", error = ex.Message });
        }
    }

    [HttpPost("uploadPhotoAnnonce/{annonceId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadPhotoAnnonce([FromForm] PhotoDTO photoDto, int annonceId)
    {
        Console.WriteLine($"🔵 UploadPhotoAnnonce appelé pour annonce {annonceId}");

        if (photoDto?.File == null)
        {
            Console.WriteLine("❌ Fichier null");
            return BadRequest("Fichier requis");
        }

        Console.WriteLine($"📁 Fichier reçu: {photoDto.File.FileName}, Taille: {photoDto.File.Length} bytes");

        try
        {
            var photo = await _photoService.UploadPhotoAnnonceAsync(photoDto, annonceId);
            Console.WriteLine($"✅ Photo uploadée avec ID: {photo.PhotoId}");
            return File(photo.Image, "image/jpeg");
        }
        catch (NotFoundException ex)
        {
            Console.WriteLine($"❌ NotFoundException: {ex.Message}");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception: {ex.Message}");
            Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Erreur lors de l'upload de la photo", error = ex.Message });
        }
    }

    [HttpPost("uploadComptePhoto/{compteId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadComptePhoto([FromForm] PhotoDTO photoDto, int compteId)
    {
        if (photoDto?.File == null)
        {
            return BadRequest(new { message = "Fichier requis" });
        }

        try
        {
            var photo = await _photoService.UploadComptePhotoAsync(photoDto, compteId);
            return File(photo.Image, "image/jpeg");
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
    
    [HttpPost("uploadMessagePhoto/{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadMessagePhoto([FromForm] PhotoDTO photoDto, int messageId)
    {
        if (photoDto?.File == null)
        {
            return BadRequest(new { message = "Fichier requis" });
        }

        try
        {
            var photo = await _photoService.UploadMessagePhotoAsync(photoDto, messageId);
            return File(photo.Image, "image/jpeg");
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
            await _photoService.DeletePhotoAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur lors de la suppression de la photo", error = ex.Message });
        }
    }
    
    
}