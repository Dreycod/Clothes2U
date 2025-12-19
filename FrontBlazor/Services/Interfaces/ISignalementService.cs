
using Shared.DTO;
using Shared.DTO.Signalement;

namespace FrontBlazor.Services.GenericIServices;
public interface ISignalementService : IWritableService<SignalementDTO> 
{
    Task<List<SignalementDTO>> GetAllAsync();
    Task<List<SignalementDTO>> GetAllByType(int typeId);
    Task<SignalementDetailsDTO> GetSignalementByIdAsync(int id);
    Task<SignalementCreateDTO> CreateSignalement(SignalementCreateDTO signalement);
}