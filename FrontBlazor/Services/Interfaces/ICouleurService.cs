using FrontBlazor.Services.Interfaces.GenericIServices;
using Shared.DTO.Annonce;
using Shared.DTO.Couleur;

namespace FrontBlazor.Services;

public interface ICouleurService<TEntity> : IService<TEntity> where TEntity : CouleurDTO
{
    Task<EstDeCouleurDTO?> CouleurToEdc(CouleurDTO couleur, AnnonceDTO annonce);
    Task<CouleurDTO?> AddAsync(CreateCouleurDTO couleur);
}