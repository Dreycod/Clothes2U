using Shared.DTO;

namespace FrontBlazor.Services.GenericIServices;

public interface IAnnonceService 
{
    Task<List<Annonce>> GetActiveAnnonces();
    Task<AnnonceDetail> GetAnnonceDetailById(int id);
    Task<List<Annonce>?> GetAnnoncesByUserIdAsync(int id);
    Task<List<Annonce>?> GetAnnonceByFilter(FilterDTO filterDto, int page = 1, int pageSize = 30);
    Task<List<Annonce>?> GetByFavorisUtilisateur();
    Task CreateAnnonce(AnnonceCreate annonce);
    Task ModificationAnnonce(AnnonceCreate annonce);

}