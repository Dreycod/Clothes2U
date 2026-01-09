using API.Models.EntityFramework;

namespace API.Models.Repository.Interfaces;

public interface IMessageRepository : IDataRepository<Message, int>
{
    Task<int> GetMessageCountByUserId(int id);
}