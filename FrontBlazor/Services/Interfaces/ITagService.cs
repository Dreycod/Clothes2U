using FrontBlazor.Services.GenericIServices;
using Shared.DTO.Tag;

namespace FrontBlazor.Services.Interfaces
{
    public interface ITagService
    {
        Task<List<TagDTO>?> GetAllTagsAsync();
        Task<TagDTO?> GetTagById(int id);
        Task<TagDTO?> GetTagByName(string name);
        Task<TagDTO?> AddTagAsync(CreateTagDTO tag);
        Task<bool> DeleteTagAsync(int id);
    }
}
