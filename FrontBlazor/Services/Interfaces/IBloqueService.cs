using Shared.DTO;
using Shared.DTO.Bloque;
using FrontBlazor.Services.Interfaces.GenericIServices;

namespace FrontBlazor.Services.Interfaces;

public interface IBloqueService : IWritableService<BloqueDTO>
{
    Task CreateBloque(int utilisateurBloqueId);
    Task<List<BloqueDetailDTO>> GetUsersBloquee(int? id);
}
