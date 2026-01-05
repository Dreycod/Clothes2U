
using Shared.DTO;
using Shared.DTO.Signalement;

namespace FrontBlazor.Services.GenericIServices;
public interface ISignalementService 
{
    Task<List<SignalementDTO>> GetAllAsync();
    Task<List<SignalementDTO>> GetAllByType(int typeId);
    Task<SignalementDetailsDTO> GetSignalementByIdAsync(int id);
    Task<SignalementDetailsDTO?> CreateSignalement(SignalementCreateDTO signalement);
    Task DeleteAsync(int id);
}