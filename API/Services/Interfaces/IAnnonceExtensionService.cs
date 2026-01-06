using Shared.DTO.Annonce;

namespace API.Services;

public interface IAnnonceExtensionService
{
    Task<IEnumerable<AnnonceDTO>> LikeAnnonces(IEnumerable<AnnonceDTO> annoncesDTO);
    Task<AnnonceDetailDTO> LikeAnnonceDetail(AnnonceDetailDTO annonceDetailDTO);
}