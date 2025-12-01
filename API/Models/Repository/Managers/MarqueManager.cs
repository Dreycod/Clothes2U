using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class MarqueManager : GenericCRUDManager<Marque>, IDataRepository<Marque, int>
{
    public MarqueManager(Clothes2UDbContext context) : base(context){}
}