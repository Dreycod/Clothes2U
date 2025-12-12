
using FrontBlazor.Models;

namespace FrontBlazor.Services.GenericIServices;
public interface ISignalementService : IWritableService<Signalement> 
{
    Task<List<Signalement>> GetAllAsync();
    Task<List<Signalement>> GetAllByType(int typeId);
    Task<SignalementDetails> GetSignalementByIdAsync(int id);
}