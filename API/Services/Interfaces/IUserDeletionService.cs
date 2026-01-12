namespace API.Services.Interfaces
{
    public interface IUserDeletionService
    {
        Task DeleteUtilisateurAsync(int utilisateurId);
    }
}
