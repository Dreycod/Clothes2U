using API.DTO;
using API.Exceptions;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class PhotoService : IPhotoService
{
    private readonly IPhotoRepository<Photo, int> _photoRepository;
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceRepository;
    private readonly IDataRepository<Illustre_Annonce, int> _illustreAnnonceRepository;
    private readonly IUtilisateurRepository _utilisateurManager;
    private readonly Clothes2UDbContext _context;

    public PhotoService(
        IPhotoRepository<Photo, int> photoRepository,
        IAnnonceRepository<Annonce, int, FilterDTO> annonceRepository,
        IDataRepository<Illustre_Annonce, int> illustreAnnonceRepository,
        IUtilisateurRepository utilisateurRepository,
        Clothes2UDbContext context)
    {
        _photoRepository = photoRepository;
        _annonceRepository = annonceRepository;
        _illustreAnnonceRepository = illustreAnnonceRepository;
        _utilisateurManager = utilisateurRepository;
        _context = context;
    }

    public async Task<Photo?> GetPhotoAsync(int id)
    {
        return await _photoRepository.GetByIdAsync(id);
    }

    public async Task<Photo> UploadPhotoAnnonceAsync(PhotoDTO photoDto, int annonceId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Validation métier
            var annonce = await _annonceRepository.GetByIdAsync(annonceId);
            if (annonce == null)
            {
                throw new NotFoundException($"Annonce {annonceId} introuvable");
            }

            // TODO: Ajouter validation d'image (taille, format, contenu)
            
            // Création de la photo
            var photo = await _photoRepository.AddPhotoAsync(photoDto);

            // Création de la relation
            var illustre = new Illustre_Annonce
            {
                AnnonceId = annonceId,
                PhotoId = photo.PhotoId
            };
            await _illustreAnnonceRepository.AddAsync(illustre);

            await transaction.CommitAsync();
            return photo;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Photo> UploadComptePhotoAsync(PhotoDTO photoDto, int compteId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Validation métier
            var utilisateur = await _utilisateurManager.GetByIdAsync(compteId);
            if (utilisateur == null)
            {
                throw new NotFoundException($"Utilisateur {compteId} introuvable");
            }

            // TODO: Ajouter validation d'image

            // Création de la photo
            var photo = await _photoRepository.AddPhotoAsync(photoDto);

            // Mise à jour de l'utilisateur
            var utilisateurToUpdate = await _utilisateurManager.GetByIdAsync(compteId);
            utilisateurToUpdate.PhotoId = photo.PhotoId;
            await _utilisateurManager.UpdateAsync(utilisateurToUpdate, utilisateurToUpdate);

            await transaction.CommitAsync();
            return photo;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeletePhotoAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var photo = await _photoRepository.GetByIdWithRelationsAsync(id);
            if (photo == null)
            {
                throw new NotFoundException($"Photo {id} introuvable");
            }
            var illustreAnnonce = await ((IllustreAnnonceRepository<Illustre_Annonce, int>)_illustreAnnonceRepository)
                .GetByPhotoId(id);

            if (illustreAnnonce != null)
            {
                await _illustreAnnonceRepository.DeleteAsync(illustreAnnonce);
            }
            if (photo.Utilisateur != null)
            {
                var utilisateurToUpdate = await _utilisateurManager.GetByIdAsync(photo.Utilisateur.UtilisateurId);
                utilisateurToUpdate.PhotoId = null;
                await _utilisateurManager.UpdateAsync(utilisateurToUpdate, utilisateurToUpdate);
            }
            await _photoRepository.DeleteAsync(photo);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}