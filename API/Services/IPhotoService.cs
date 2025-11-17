namespace API.Services;

public interface IPhotoService
{
    public Task<string> AddPhotoToAnnonceAsync(int annonceId, IFormFile photoFile);
    public string? GetMediaPath(string folder, string fileName);
}