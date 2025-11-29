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
    private readonly IPhotoRepository<Photo, int> _photoManager;
    private readonly IAnnonceRepository<Annonce, int> _annonceManager;
    private readonly IDataRepository<Illustre_Annonce, int> _illustreAnnonceManager;
    private readonly IDataRepository<Utilisateur, int> _utilisateurManager;

    public MediasController(Clothes2UDbContext context, IPhotoRepository<Photo, int> photoManager,IAnnonceRepository<Annonce, int> annonceManager, IDataRepository<Illustre_Annonce, int> illustreAnnonceManager,IDataRepository<Utilisateur, int> utilisateurManager,  IWebHostEnvironment env)
    {
        _photoManager = photoManager;
        _annonceManager = annonceManager;
        _illustreAnnonceManager = illustreAnnonceManager;
        _utilisateurManager = utilisateurManager;
    }
    [HttpGet("Photos/{id}")]
    public async Task<IActionResult> GetPhotos(int id)
    {
        Photo?  photo = await _photoManager.GetByIdAsync(id);
        if (photo == null)
        {
            return NotFound();
        }

        return File(photo.Image, "image/jpeg");
    }

    
    [HttpPost("uploadPhotoAnnonce")]
    public async Task<IActionResult> UploadPhotoAnnonce([FromForm] PhotoDTO photoDto, int annonceId)
    {
        //ajouter le service de verification d'image
        Annonce? annonce = await  _annonceManager.GetByIdAsync(annonceId);
        if (annonce == null)
        {
            return NotFound();
        }

        Photo photo = await _photoManager.AddPhotoAsync(photoDto);
        Illustre_Annonce illustre = new Illustre_Annonce()
        {
            AnnonceId = annonceId,
            PhotoId = photo.PhotoId
        };
        await _illustreAnnonceManager.AddAsync(illustre);
        return File(photo.Image, "image/jpeg");
    }

    [HttpPost("uploadComptePhoto")]
    public async Task<IActionResult> UploadComptePhoto([FromForm] PhotoDTO dto, int compteId)
    {
        Utilisateur? utilisateur = await _utilisateurManager.GetByIdAsync(compteId);
        if (utilisateur == null)
        {
            return NotFound();
        }
        throw new NotImplementedException();
    }

    [HttpDelete("Photos/{id}")]
    public async Task<IActionResult> DeletePhoto(int id)
    {
        Photo? photo = await _photoManager.GetByIdAsync(id);
        if (photo == null)
        {
            return NotFound();
        }
        await _photoManager.DeleteAsync(photo);
        return NoContent();
    }

    private int CreatePhoto()
    {
        throw new NotImplementedException();
    }
    
    

}