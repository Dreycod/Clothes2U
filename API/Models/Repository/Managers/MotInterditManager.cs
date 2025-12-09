using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class MotInterditManager : GenericCRUDManager<MotInterdit>
{
    public MotInterditManager(Clothes2UDbContext context) : base(context) {}
}