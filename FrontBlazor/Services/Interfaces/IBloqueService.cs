using FrontBlazor.Models;

namespace FrontBlazor.Services.GenericIServices;

public interface IBloqueService : IWritableService<Bloque>
{
    Task CreateBloque(int utilisateurBloqueId);

}
