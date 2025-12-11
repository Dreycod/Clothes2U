using API.Models.EntityFramework;
using API.Models.Repository;

namespace API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUtilisateurRepository _utilisateurManager;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IUtilisateurRepository  utilisateurManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _utilisateurManager = utilisateurManager;
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
}