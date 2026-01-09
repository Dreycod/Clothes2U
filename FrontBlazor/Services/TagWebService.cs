using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using Shared.DTO;
using Shared.DTO.Couleur;
using Shared.DTO.Tag;
using System.Net.Http.Json;

namespace FrontBlazor.Services
{
    public class TagWebService : BaseGenericService, ITagService
    {
        public TagWebService(HttpClient httpClient) : base(httpClient)
        {
        }
        public async Task<List<TagDTO>?> GetAllTagsAsync()
        {
            var response = await GetWithCredentialsAsync("Tag/GetAllTags");
            response.EnsureSuccessStatusCode();
            var tags = await response.Content.ReadFromJsonAsync<List<TagDTO>>();
            return tags ?? new List<TagDTO>();
        }
        
        public async Task<TagDTO?> GetTagById(int id)
        {
            var response = await GetWithCredentialsAsync($"Tag/id/{id}");
            response.EnsureSuccessStatusCode();
            var tag = await response.Content.ReadFromJsonAsync<TagDTO>();
            return tag;
        }

        public async Task<TagDTO?> GetTagByName(string name)
        {
            var response = await GetWithCredentialsAsync($"Tag/name/{name}");
            response.EnsureSuccessStatusCode();
            var tag = await response.Content.ReadFromJsonAsync<TagDTO>();
            return tag;
        }

        public async Task<TagDTO?> AddTagAsync(CreateTagDTO tag)
        {
            var response = await PostWithCredentialsAsync("Tag/AddTag", JsonContent.Create(tag));
            response.EnsureSuccessStatusCode();
            var createdTag = await response.Content.ReadFromJsonAsync<TagDTO>();
            return createdTag;
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            var response = await DeleteWithCredentialsAsync($"Tag/DeleteTag/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
