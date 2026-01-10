using Shared.DTO;
using Shared.DTO.Annonce;

namespace FrontBlazor.Services.GenericIServices;

public interface IAnnonceService 
{
    Task<List<AnnonceDTO>> GetActiveAnnonces();
    Task<AnnonceDetailDTO> GetAnnonceDetailById(int id);
    Task<List<AnnonceDTO>?> GetAnnoncesByUserIdAsync(int id);
    Task<List<AnnonceDTO>?> GetAnnonceByFilter(FilterDTO filterDto, int page = 1, int pageSize = 30);
    Task<List<AnnonceDTO>?> GetByFavorisUtilisateur();
    Task<List<AnnonceDTO>?> GetSimilarAnnonces(int annonceId, int page = 1, int pageSize = 30);
    Task CreateAnnonce(CreateAnnonceDTO annonce);
    Task UpdateAnnonce(int id, PutAnnonceDTO annonce);
    Task ModificationAnnonce(AnnonceDetailDTO annonce);
    Task VendreAnnonce(int annonceId);
    Task<List<AnnonceDTO>> GetRecommendedAnnonces(int page = 1, int pageSize = 30);
}