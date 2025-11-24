using API.DTO;
using API.Exceptions;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MediasController : ControllerBase
{
    private readonly IPhotoService _photoManager;

    public MediasController(Clothes2UDbContext context, IPhotoService manager, IWebHostEnvironment env)
    {
        _photoManager = manager;
    }
    [HttpGet("Photos/{fileName}")]
    public async Task<IActionResult> GetPhotos(string fileName)
    {
        try
        {
            var path = await _photoManager.GetMediaPath("Images", fileName);
            return PhysicalFile(path, "image/jpeg");
        }
        catch (PhotoNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur serveur", details = ex.Message });
        }
    }

    
    [HttpPost("uploadPhotoAnnonce")]
    public async Task<IActionResult> UploadPhotoAnnonce([FromForm] MediaUploadDto dto, int annonceId)
    {
        if (dto.File == null || dto.File.Length == 0)
            return BadRequest("Aucun fichier envoyé.");

        try
        {
            var fileName = await _photoManager.AddPhotoToAnnonceAsync(annonceId, dto.File);
            var url = $"{Request.Scheme}://{Request.Host}/api/medias/images/{fileName}";
        
            return Ok(new { FileName = fileName, Url = url });
        }
        catch (AnnonceNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erreur lors de l'upload", details = ex.Message });
        }
    }

    [HttpDelete("Photos/{id}")]
    public async Task<IActionResult> DeletePhoto(int id)
    {
        try
        {
            await _photoManager.DeleteMediaById(id);
            return NoContent();
        }
        catch (PhotoNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

}