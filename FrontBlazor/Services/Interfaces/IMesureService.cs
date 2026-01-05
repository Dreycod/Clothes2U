using Shared.DTO.Mesures;

namespace FrontBlazor.Services.GenericIServices;

public interface IMesureService
{
    Task<List<MesureDTO>?> GetAllMesuresAsync();
    Task<List<int>?> GetTailleIdsByCategorieAsync(int categorieId);
}
