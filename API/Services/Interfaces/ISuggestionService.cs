using API.Models.EntityFramework;
using Shared.DTO.Annonce;

namespace API.Services;

public interface ISuggestionService
{
    Task CalculSuggestion(int userId);
    Task<IEnumerable<Annonce>> GetRecommandations(int userId); 
}