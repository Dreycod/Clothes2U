using API.Exceptions;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using Microsoft.AspNetCore.Mvc;

namespace API.Services;

public class PhotoService :  IPhotoService
{
    private readonly IAnnonceRepository<Annonce, int> _annonceRepository;
    private readonly IPhotoRepository<Photo, int> _photoRepository;
    private readonly IDataRepository<Illustre_Annonce, int> _illustAnnonceRepository;
    private readonly IWebHostEnvironment _env;
    
    public PhotoService(
        IAnnonceRepository<Annonce, int> annonceRepository,
        IPhotoRepository<Photo, int> photoRepository,
        IDataRepository<Illustre_Annonce, int> illustAnnonceRepository,
        IWebHostEnvironment env) 
    {
        _annonceRepository = annonceRepository ?? throw new ArgumentNullException(nameof(annonceRepository));
        _photoRepository = photoRepository ?? throw new ArgumentNullException(nameof(photoRepository));
        _illustAnnonceRepository = illustAnnonceRepository ?? throw new ArgumentNullException(nameof(illustAnnonceRepository));
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    public async Task<string> AddPhotoToAnnonceAsync(int annonceId, IFormFile photoFile)
    {
        //ajouter l'analyse de données. 
        
        
        
        Annonce? annonce = await _annonceRepository.GetByIdAsync(annonceId);
        if (annonce == null)
        {
            throw new AnnonceNotFoundException(annonceId);
        }
   
        var directoryPath = Path.Combine(_env.ContentRootPath, "Medias/Images");

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
   
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photoFile.FileName)}";
        var filePath = Path.Combine(directoryPath, fileName);
   
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await photoFile.CopyToAsync(stream);
        }

        Photo photo = new Photo()
        {
            PhotoUri = fileName
        };

        await _photoRepository.AddAsync(photo); 
        Illustre_Annonce relation = new Illustre_Annonce()
        {
            AnnonceId = annonceId,
            Photo = photo
        };

        await _illustAnnonceRepository.AddAsync(relation); 
        return fileName;
    }
    public Task<string> GetMediaPath(string folder, string fileName)
    {
        var path = Path.Combine(_env.ContentRootPath, "Medias", folder, fileName);

        if (!File.Exists(path))
            throw new PhotoNotFoundException(fileName);

        return Task.FromResult(path);
    }

    public async Task DeleteMediaById(int id)
    {
        // Récupérer la photo avec ses Illustre_Annonce
        Photo? photo = await _photoRepository.GetByIdWithRelationsAsync(id);
        if (photo == null)
            throw new PhotoNotFoundException(id.ToString());

        // Supprimer toutes les relations Illustre_Annonce
        var illustRelations = photo.Annonces.ToList();
        foreach (var relation in illustRelations)
        {
            await _illustAnnonceRepository.DeleteAsync(relation);
        }

        // Supprimer le fichier physique
        var path = Path.Combine(_env.ContentRootPath, "Medias", "Images", photo.PhotoUri);
        if (File.Exists(path))
            File.Delete(path);

        // Supprimer l'entité Photo
        await _photoRepository.DeleteAsync(photo);
    }

}