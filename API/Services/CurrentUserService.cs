using API.Models.EntityFramework;
using API.Models.Repository;

namespace API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUtilisateurRepository _utilisateurManager;
    private readonly IAbonnementRepository<Abonnement, int>  _abonnementRepository;
    private readonly IBloqueRepository<Bloque, int>  _bloqueRepository;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        IUtilisateurRepository  utilisateurManager,
        IAbonnementRepository<Abonnement, int>  abonnementRepository,
        IBloqueRepository<Bloque, int> bloqueRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _utilisateurManager = utilisateurManager;
        _abonnementRepository = abonnementRepository;
        _bloqueRepository = bloqueRepository;
    }

    public async Task<int?> GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        
        if (user?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = user.FindFirst("userId")?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int id))
            {
                return id;
            }
        }
        
        return null;
    }

    public async  Task<Utilisateur?> GetUser()
    {
        int? userId = await GetUserId();
        if (userId == null)
        {
            return null;
        }
        Utilisateur user = await _utilisateurManager.GetByIdAsync((int)userId);
        return user;
    }
    public async Task<bool> IsFollowedByCurrentUser(int suivisId)
    {
        int? userId = await GetUserId();
        if (userId == null)
        {
            return false;
        }
        return await _abonnementRepository.Exists((int)userId, suivisId);
    }

    public async Task<bool> IsBlockedByCurrentUser(int suivisId)
    {
        int? userId = await GetUserId();
        if (userId == null)
        {
            return false;
        }
        return await _bloqueRepository.Exists((int)userId, suivisId);
    }
}