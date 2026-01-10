using FrontBlazor.Services.Interfaces.GenericIServices;
using Stripe;

namespace FrontBlazor.Services.Interfaces;

public interface ICaracteristiqueService<TEntity> : IListableService<TEntity>, IWritableService<TEntity> where TEntity : class 
{
    
}