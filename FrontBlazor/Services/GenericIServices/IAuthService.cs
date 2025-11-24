using FrontBlazor.Models;
using System.Net.Http;

namespace FrontBlazor.Services.GenericIServices;

public interface IAuthService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> SignUpAsync(LoginRequest request);
    Task<List<TEntity>?> LoginAsync(LoginRequest request);
}
