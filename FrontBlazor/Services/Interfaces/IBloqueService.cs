using Shared.DTO;
using Shared.DTO.Bloque;

namespace FrontBlazor.Services.GenericIServices;

public interface IBloqueService : IWritableService<BloqueDTO>
{
    Task CreateBloque(int utilisateurBloqueId);
    Task<List<BloqueDetailDTO>> GetUsersBloquee(int? id);
}
