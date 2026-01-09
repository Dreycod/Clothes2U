using FrontBlazor.Services.Interfaces.GenericIServices;
using Shared.DTO.Tag;
using Shared.DTO.Annonce;
using Shared.DTO.Recense;

namespace FrontBlazor.Services.Interfaces
{
    public interface ITagService<TEntity> : IService<TEntity> where TEntity : TagDTO
    {
        Task<RecenseDTO?> TagToRecense(TagDTO tag, AnnonceDTO annonce);
        Task<TagDTO?> AddAsync(CreateTagDTO tag);
    }
}
