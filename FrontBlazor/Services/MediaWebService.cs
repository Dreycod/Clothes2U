using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Globalization;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.WebUtilities;

namespace FrontBlazor.Services;

public class MediaWebService : WritableService<Photo>, IMediasService<Photo>
{
    public MediaWebService(HttpClient httpClient) : base(httpClient) { }

    public async Task<Photo> GetPhotoAsync(int id)
    {
        var response = await GetWithCredentialsAsync($"Medias/Photos/{id}");
        response.EnsureSuccessStatusCode();


        var photo = await response.Content.ReadFromJsonAsync<Photo>();

        return photo ?? new Photo();
    }

    public Task<Photo> uploadPhotoCompteAsync(int compteid, Photo image)
    {
       throw new NotImplementedException();
    }

    public Task<Photo> uploadPhotoAnnonceAsync(int annonceid, Photo image)
    {
        throw new NotImplementedException();
    }
}