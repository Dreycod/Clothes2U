using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services.Interfaces;

public class SignalementWebService : ISignalementService
{
    public Task<Signalement> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Signalement?> AddAsync(Signalement entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Signalement updatedEntity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}