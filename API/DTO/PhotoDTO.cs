namespace API.DTO;

public class PhotoDTO
{
    public int?  PhotoId { get; set; }
    public IFormFile File { get; set; }
}