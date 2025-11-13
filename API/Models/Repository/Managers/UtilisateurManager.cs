using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class UtilisateurManager : IDataRepository<Utilisateur, int>
{
    public async Task<IEnumerable<Utilisateur>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Utilisateur?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Utilisateur entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Utilisateur entityToUpdate, Utilisateur entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Utilisateur entity)
    {
        throw new NotImplementedException();
    }
}