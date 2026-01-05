using API.Models.EntityFramework;

namespace API.Models.Repository
{
    public interface IPasswordResetRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>
    {
        Task<PasswordResetToken?> GetValidToken(string token);
        Task InvalidateUserTokens(int utilisateurId);
    }
}
