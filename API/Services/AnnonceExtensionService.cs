using API.Models.EntityFramework;
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

    public async Task<IEnumerable<AnnonceDTO>> CheckOwnerAnnonce(IEnumerable<AnnonceDTO> annoncesDTO)
    {
        int? userId = await _currentUserService.GetUserId();
        if (userId != null)
        {
            foreach (var annonce in annoncesDTO)
            {
                if (annonce.IdAuteur == userId)
                {
                    annonce.IsOwnerAnnonce = true;
                }
            }
        }
        return annoncesDTO;
    }

    public async Task<AnnonceDetailDTO> CheckOwnerAnnonceDetail(AnnonceDetailDTO annonceDetailDTO)
    {
        int? userId = await _currentUserService.GetUserId();
        if (userId != null)
        {
            if (annonceDetailDTO.UtilisateurId == userId)
                annonceDetailDTO.IsOwnerAnnonce = true;
        }
        return annonceDetailDTO;
    }
}