namespace API.Services.Interfaces
{
    public interface IUserDeletionService
    {
        Task DeleteUtilisateurByAdminAsync(int utilisateurId, int adminId);
    }
}
