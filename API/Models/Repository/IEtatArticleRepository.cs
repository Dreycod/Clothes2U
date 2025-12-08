using API.Models.EntityFramework;
using API.Models.Repository.Managers;

namespace API.Models.Repository;

public interface IEtatArticleRepository : IDataRepository<EtatArticle, int>, IFiltrableByIdRepository<EtatArticle, int>
{

}
