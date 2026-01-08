using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class EtatArticleManager : GenericCRUDManager<EtatArticle>, IEtatArticleRepository
{
    public EtatArticleManager(Clothes2UDbContext context) : base(context) { }


}