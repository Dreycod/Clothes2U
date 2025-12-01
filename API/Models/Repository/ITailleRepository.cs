using API.Models.EntityFramework;
using API.Models.Repository.Managers;

namespace API.Models.Repository;

public interface ITailleRepository : IDataRepository<Taille, int>, IFiltrableByIdRepository<Taille, int>
{
    
}