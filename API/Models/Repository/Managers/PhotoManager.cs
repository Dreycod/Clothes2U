using Shared.DTO.Photo;
using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class PhotoManager : GenericCRUDManager<Photo>, IPhotoRepository<Photo, int>
{
    public PhotoManager(Clothes2UDbContext context) : base(context) { }

    public async Task<Photo?> GetByIdWithRelationsAsync(int id)
    {
        return await _context.Photos
            .Include(p => p.Annonces)
            .Include(p => p.Utilisateur)
            .FirstOrDefaultAsync(p => p.PhotoId == id);
    }

    public async Task<Photo> AddPhotoAsync(PhotoUploadDTO photoDto)
    {
        // Décoder le Base64 en bytes
        byte[] imageBytes;

        try
        {
            var base64Data = photoDto.Base64Data;
            if (base64Data.Contains(","))
            {
                base64Data = base64Data.Split(',')[1];
            }

            imageBytes = Convert.FromBase64String(base64Data);
        }
        catch (FormatException)
        {
            throw new ArgumentException("Le format Base64 de l'image est invalide");
        }

        var photo = new Photo
        {
            Image = imageBytes
        };

        _context.Photos.Add(photo);
        await _context.SaveChangesAsync();

        return photo;
    }
}