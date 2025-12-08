using FrontBlazor.Models;

namespace FrontBlazor.Services.GenericIServices
{
    public interface IMediasService<TEntity>
    {
        Task<TEntity> GetPhotoAsync(int id);
        Task<TEntity> uploadPhotoAnnonceAsync(int annonceid, Photo image); // pas sur pour le type de retour
        Task<TEntity> uploadPhotoCompteAsync(int compteid, Photo image); // idem
    }
}
