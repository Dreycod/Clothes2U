using API.Models.EntityFramework;

namespace API.Models.Repository;

public interface IMarqueRepository : ICaracteristiquesRepository<Marque>, ISearchableRepository<Marque>
{
    
}