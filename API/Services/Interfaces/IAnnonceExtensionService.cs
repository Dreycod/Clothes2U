using Shared.DTO.Annonce;

namespace API.Services;

public interface IAnnonceExtensionService
{
    Task<IEnumerable<AnnonceDTO>> LikeAnnonces(IEnumerable<AnnonceDTO> annoncesDTO);
    Task<AnnonceDetailDTO> LikeAnnonceDetail(AnnonceDetailDTO annonceDetailDTO);
    Task<IEnumerable<AnnonceDTO>> CheckOwnerAnnonce(IEnumerable<AnnonceDTO> annoncesDTO);
    Task<AnnonceDetailDTO> CheckOwnerAnnonceDetail(AnnonceDetailDTO annonceDetailDTO);
}