
using FrontBlazor.Models;

namespace FrontBlazor.Services.GenericIServices;
public interface ISignalementService : IReadableService<Signalement>, IWritableService<Signalement> 
{
    Task<List<Signalement>> GetAllAsync();
    Task<List<Signalement>> GetAllByType(int typeId);
}