using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class PhotoManager : GenericCRUDManager<Photo>
{
    public PhotoManager(Clothes2UDbContext context) : base(context){}
}