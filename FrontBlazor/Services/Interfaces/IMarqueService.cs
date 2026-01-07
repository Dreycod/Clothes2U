using Shared.DTO.Marque;

namespace FrontBlazor.Services.Interfaces;

public interface IMarqueService : ICaracteristiqueService<MarqueDTO>, ISearchableService<MarqueDTO>
{
    
}