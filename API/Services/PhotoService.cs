using API.Models.EntityFramework;
using API.Models.Repository;

namespace API.Services;

public class PhotoService :  IPhotoService
{
    private readonly IAnnonceRepository<Annonce, int> _annonceRepository;
    private readonly IDataRepository<Photo, int> _photoRepository;
    private readonly IDataRepository<Illustre_Annonce, int> _illustAnnonceRepository;
    private readonly IWebHostEnvironment _env;
    
    public PhotoService(
        IAnnonceRepository<Annonce, int> annonceRepository,
        IDataRepository<Photo, int> photoRepository,
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
            throw new Exception("Annonce not found");
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
            PhotoId = photo.PhotoId
        };

        await _illustAnnonceRepository.AddAsync(relation); 
        return fileName;
    }
    public string? GetMediaPath(string folder, string fileName)
    {
        var path = Path.Combine(_env.ContentRootPath, "Medias", folder, fileName);

        return File.Exists(path) ? path : null;
    }
}