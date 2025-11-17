using API.DTO;
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
    public IActionResult GetPhotos(string fileName)
    {
        var path = _photoManager.GetMediaPath("Images",  fileName);

        if (path == null)
        {
            return NotFound();
        }
        return PhysicalFile(path, "image/jpeg");
    }

    
    [HttpPost("uploadPhotoAnnonce")]
    public async Task<IActionResult> UploadPhotoAnnonce([FromForm] MediaUploadDto dto, int annonceId)
    {
        if (dto.File == null || dto.File.Length == 0)
            return BadRequest("Aucun fichier envoyé.");

        var fileName = await _photoManager.AddPhotoToAnnonceAsync(annonceId, dto.File);

        var url = $"{Request.Scheme}://{Request.Host}/api/medias/images/{fileName}";

        return Ok(new
        {
            FileName = fileName,
            Url = url
        });
    }

}