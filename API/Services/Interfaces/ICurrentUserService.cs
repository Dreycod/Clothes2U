using API.Models.EntityFramework;

namespace API.Services;

public interface ICurrentUserService
{
    Task<int?> GetUserId();
    Task<Utilisateur?> GetUser();
    Task<int> GetUserIdOrThrow();
    Task<bool> IsFollowedByCurrentUser(int id);
    Task<bool> IsBlockedByCurrentUser(int id);
}