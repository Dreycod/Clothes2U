using API.Models.Repository.Managers;
using Shared.DTO.Annonce;

namespace API.Services;

public class AnnonceExtensionService : IAnnonceExtensionService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IFavorisRepository  _favorisRepository;

    public AnnonceExtensionService(
        ICurrentUserService currentUserService,
        IFavorisRepository favorisManager
        )
    {
        _currentUserService = currentUserService;
        _favorisRepository = favorisManager;
    }
    public async Task<IEnumerable<AnnonceDTO>> LikeAnnonces(IEnumerable<AnnonceDTO> annoncesDTO)
    {
        int? userId = await _currentUserService.GetUserId();
        if (userId != null)
        {
            foreach (var annonce in annoncesDTO)
            {
                if (await _favorisRepository.CheckIfLiked((int)userId, annonce.AnnonceId))
                {
                    annonce.IsLikedByCurrentUser = true;
                }
            }
        }
        return annoncesDTO;
        
    }

    public async Task<AnnonceDetailDTO> LikeAnnonceDetail(AnnonceDetailDTO annonceDetailDTO)
    {
        int? userId = await _currentUserService.GetUserId();
        if (userId != null)
        {
            if (await _favorisRepository.CheckIfLiked((int)userId, annonceDetailDTO.AnnonceId))
            {
                annonceDetailDTO.IsLikedByCurrentUser = true;
            }
        }
        return annonceDetailDTO;
    }
}