using FrontBlazor.Services.Interfaces.GenericIServices;
using Shared.DTO.Mesures;
using Shared.DTO.Taille;

namespace FrontBlazor.Services.Interfaces;

public interface ITailleService: ICaracteristiqueService<TailleDTO>, IListableService<TailleDTO>
{
    Task PutTailleMesuresAsync(int tailleid, List<MesureDTO> MesuresDTO);
}

