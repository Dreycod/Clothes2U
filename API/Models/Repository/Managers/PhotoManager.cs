using API.DTO;
using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;


namespace API.Models.Repository.Managers;

public class PhotoManager: GenericCRUDManager<Photo>, IPhotoRepository<Photo, int>
{
    public PhotoManager(Clothes2UDbContext context) :  base(context){}

    public async Task<Photo?> GetByIdWithRelationsAsync(int id)
    {
        return await _context.Photos
            .Include(p => p.Annonces)
            .FirstOrDefaultAsync(p => p.PhotoId == id);
    }

    public async Task<Photo> AddPhotoAsync(PhotoDTO photoDto)
    {
        using var ms = new MemoryStream();
        await photoDto.File.CopyToAsync(ms);

        var photo = new Photo
        {
            Image = ms.ToArray()
        };

        _context.Photos.Add(photo);
        await _context.SaveChangesAsync();
        return photo;
    }

}   