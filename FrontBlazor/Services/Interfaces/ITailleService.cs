namespace FrontBlazor.Services.GenericIServices;

using FrontBlazor.Services.Interfaces;
using Shared.DTO.Mesures;
using Shared.DTO.Taille;

public interface ITailleService: ICaracteristiqueService<TailleDTO>, IListableService<TailleDTO>
{
    Task PutTailleMesuresAsync(int tailleid, List<MesureDTO> MesuresDTO);
}

