using API.Models.EntityFramework;
using API.Models.Repository;

namespace API.Services;

public interface IMessageDemandeRepository : IDataRepository<MessageDemande, int>
{
    Task<MessageDemande?> GetByMessageIdAsync(int id);
}