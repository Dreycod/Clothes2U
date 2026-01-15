using Shared.DTO.Signalement;

namespace API.Services.Interfaces;

public interface ISignalementService
{
    Task<SignalementDetailsDTO> CreateSignalementAsync(SignalementCreateDTO dto, int currentUserId);
}
