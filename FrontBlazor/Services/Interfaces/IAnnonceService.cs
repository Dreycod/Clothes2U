using Shared.DTO;
using Shared.DTO.Annonce;

namespace FrontBlazor.Services.Interfaces;

public interface IAnnonceService
{
    Task<AnnonceDetailDTO?> GetAnnonceDetailById(int id);
    Task<List<AnnonceDTO>?> GetAnnoncesByUserIdAsync(int id);
    Task<List<AnnonceDTO>?> GetAnnoncesPaginationByUserIdAsync(int id, int page = 1, int pageSize = 8);
    Task<List<AnnonceDTO>?> GetAnnonceByFilter(FilterDTO filterDto, int page = 1, int pageSize = 30);
    Task<List<AnnonceDTO>?> GetByFavorisUtilisateur(int page = 1, int pageSize = 8);
    Task<List<AnnonceDTO>?> GetSimilarAnnonces(int annonceId, int page = 1, int pageSize = 30);
    Task<IEnumerable<AnnonceDTO>> GetAnnoncesByPhotoIDs(IEnumerable<int> PhotoID);
    Task<AnnonceDTO?> CreateAnnonce(CreateAnnonceDTO createAnnonceDto);
    Task<AnnonceDTO?> UpdateAnnonce(int id, PutAnnonceDTO annonce);
    Task ModificationAnnonce(AnnonceDetailDTO annonce);
    Task VendreAnnonce(int annonceId);
    Task<List<AnnonceDTO>> GetRecommendedAnnonces(int page = 1, int pageSize = 30);
    Task DeleteAnnonce(int annonceId);
    Task ChangeEtatAnnonce(int annonceId, int StatutId);
}