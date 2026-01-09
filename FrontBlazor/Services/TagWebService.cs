using FrontBlazor.Services.Interfaces.GenericIServices;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using Shared.DTO;
using Shared.DTO.Annonce;
using Shared.DTO.Tag;
using Shared.DTO.Recense;
using System.Net.Http.Json;

namespace FrontBlazor.Services
{
    public class TagWebService : BaseGenericService, ITagService<TagDTO>
    {
        public TagWebService(HttpClient httpClient) : base(httpClient)
        {
        }
        public async Task<List<TagDTO>?> GetAllAsync()
        {
            try
            {
                var response = await GetWithCredentialsAsync("Tag");
                response.EnsureSuccessStatusCode();
                var tags = await response.Content.ReadFromJsonAsync<List<TagDTO>>();
                return tags;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllTags Error: {ex.Message}");
                return null;
            }
        }
        public async Task<TagDTO?> GetByIdAsync(int id)
        {
            try
            {
                var response = await GetWithCredentialsAsync($"Tag/{id}");
                response.EnsureSuccessStatusCode();
                var tag = await response.Content.ReadFromJsonAsync<TagDTO>();
                return tag;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTagById Error: {ex.Message}");
                return null;
            }
        }
        public async Task<TagDTO?> AddAsync(TagDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<TagDTO?> AddAsync(CreateTagDTO entity)
        {
            try
            {
                var body = JsonContent.Create(entity);
                var response = await PostWithCredentialsAsync("Tag", body);
                response.EnsureSuccessStatusCode();
                var createdTag = await response.Content.ReadFromJsonAsync<TagDTO>();
                return createdTag;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddTag Error: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateAsync(TagDTO updatedEntity)
        {
            try
            {
                var body = JsonContent.Create(updatedEntity);
                var response = await PutWithCredentialsAsync($"Tag/id/{updatedEntity.IdTag}", body);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateTag Error: {ex.Message}");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var response = await DeleteWithCredentialsAsync($"Tag/id/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteTag Error: {ex.Message}");
            }
        }

        public async Task<RecenseDTO?> TagToRecense(TagDTO tag, AnnonceDTO annonce)
        {
            try
            {
                CreateRecenseDTO createRecenseDto = new CreateRecenseDTO
                {
                    AnnonceId = annonce.AnnonceId,
                    TagId = tag.IdTag
                };
                var body = JsonContent.Create(createRecenseDto);
                var response = await PostWithCredentialsAsync("Recense", body);
                response.EnsureSuccessStatusCode();
                var recenseDTO = await response.Content.ReadFromJsonAsync<RecenseDTO>();
                return recenseDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TagToRecense Error: {ex.Message}");
                return null;
            }
        }
    }
}
