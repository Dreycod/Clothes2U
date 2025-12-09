using API.Models.EntityFramework;

namespace API.Services;

public interface ICurrentUserService
{
    Task<int?> GetUserId();
    Task<Utilisateur?> GetUser();
}