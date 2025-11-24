namespace API.Services;

public interface IPhotoService
{
    public Task<string> AddPhotoToAnnonceAsync(int annonceId, IFormFile photoFile);
    public Task<string?> GetMediaPath(string folder, string fileName);
    public Task DeleteMediaById(int id);
}