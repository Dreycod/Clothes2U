
using API.Models.EntityFramework;
using Shared.DTO.Mesures;
namespace API.Models.Repository.Managers;

public interface ITailleRepository: ICaracteristiquesRepository<Taille>
{
    Task<IEnumerable<Mesure>> PutTailleMesuresAsync(int TailleId, List<Mesure> Mesures);
}
