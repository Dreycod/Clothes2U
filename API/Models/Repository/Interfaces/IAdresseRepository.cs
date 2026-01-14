using API.Models.EntityFramework;

namespace API.Models.Repository.Interfaces;

public interface IAdresseRepository
{
    Task<List<Adresse>> GetUserAddressesAsync(int userId);
    Task<Adresse?> GetByIdAsync(int id);
    Task CreateAsync(Adresse adresse);
    Task UpdateAsync(Adresse adresse); // ✅ Single parameter - the object contains its ID
    Task SetDefaultAsync(int id);
    Task DeleteAsync(int id);
}