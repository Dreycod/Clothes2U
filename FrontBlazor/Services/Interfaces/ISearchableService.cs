using Shared.DTO.Marque;

namespace FrontBlazor.Services.Interfaces;

public interface ISearchableService<T>
{
    Task<List<T>> SearchAsync(string query);
}