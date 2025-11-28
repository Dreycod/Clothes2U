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
    private readonly IDataRepository<Photo, int> _photoManager;
    private readonly IAnnonceRepository<Annonce, int> _annonceManager;
    private readonly IDataRepository<Illustre_Annonce, int> _illustreAnnonceManager;

    public MediasController(Clothes2UDbContext context, IDataRepository<Photo, int> photoManager,IAnnonceRepository<Annonce, int> annonceManager, IDataRepository<Illustre_Annonce, int> illustreAnnonceManager, IWebHostEnvironment env)
    {
        _photoManager = photoManager;
        _annonceManager = annonceManager;
        _illustreAnnonceManager = illustreAnnonceManager;
    }
    [HttpGet("Photos/{fileName}")]
    public async Task<IActionResult> GetPhotos(string fileName)
    {
        throw new NotImplementedException();
    }

    
    [HttpPost("uploadPhotoAnnonce")]
    public async Task<IActionResult> UploadPhotoAnnonce([FromForm] MediaUploadDto dto, int annonceId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("uploadComptePhoto")]
    public async Task<IActionResult> UploadComptePhoto([FromForm] MediaUploadDto dto, int compteId)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("Photos/{id}")]
    public async Task<IActionResult> DeletePhoto(int id)
    {
        throw new NotImplementedException();
    }

}