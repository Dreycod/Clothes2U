using Shared;
using Shared.DTO.DemandeRestauration;

namespace FrontBlazor.Services.Interfaces;

public interface IDemandeRestaurationService
{
    Task<List<DemandeRestaurationDTO>> GetAllDemandeRestaurations();
    Task<DemandeRestaurationDetailDTO> GetDemandeRestaurationDetail(int demandeRestaurationId);
    Task<APIResponse<DemandeRestaurationDetailDTO>> AddDemandeRestauration(string demande);
}