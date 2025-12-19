using Shared.DTO.Annonce;

namespace API.Services;

public interface ISuggestionService
{
    Task CalculSuggestion(int userId);
}