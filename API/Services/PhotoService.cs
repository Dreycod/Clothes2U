using API.Exceptions;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Photo;
using System.Numerics;

namespace API.Services;

public class PhotoService : IPhotoService
{
    private readonly IPhotoRepository<Photo, int> _photoRepository;
    private readonly IDataRepository<Annonce, int> _annonceRepository;
    private readonly IDataRepository<Utilisateur, int> _utilisateurRepository;
    private readonly ILogger<PhotoService> _logger;

    public PhotoService(
        IPhotoRepository<Photo, int> photoRepository,
        IDataRepository<Annonce, int> annonceRepository,
        IDataRepository<Utilisateur, int> utilisateurRepository,
        ILogger<PhotoService> logger)
    {
        _photoRepository = photoRepository;
        _annonceRepository = annonceRepository;
        _utilisateurRepository = utilisateurRepository;
        _logger = logger;
    }

    public async Task<Photo?> GetPhotoAsync(int id)
    {
        return await _photoRepository.GetByIdAsync(id);
    }

    public async Task<PhotoResponseDTO> SavePhotoAsync(int annonceId, PhotoUploadDTO photoDto)
    {
        // Vérifier que l'annonce existe
        var annonce = await _annonceRepository.GetByIdAsync(annonceId);
        if (annonce == null)
        {
            throw new NotFoundException($"Annonce {annonceId} introuvable");
        }

        // Créer la photo
        var photo = await _photoRepository.AddPhotoAsync(photoDto);

        // Associer la photo à l'annonce
        // (Selon votre modèle, vous devrez peut-être adapter cette partie)
        if (annonce.Photos == null)
        {
            annonce.Photos = new List<Illustre_Annonce>();
        }
        Illustre_Annonce illustre = new Illustre_Annonce
        {
            AnnonceId = annonceId,
            PhotoId = photo.PhotoId
        };
        annonce.Photos.Add(illustre);

        await _annonceRepository.UpdateAsync(annonce);

        _logger.LogInformation("Photo {PhotoId} ajoutée à l'annonce {AnnonceId}", photo.PhotoId, annonceId);

        // Retourner le DTO de réponse
        return new PhotoResponseDTO
        {
            PhotoId = photo.PhotoId,
            Url = $"/api/Medias/Photos/{photo.PhotoId}",
            FileName = photoDto.FileName,
            DateUpload = DateTime.UtcNow
        };
    }

    public async Task<PhotoResponseDTO> SaveComptePhotoAsync(int utilisateurId, PhotoUploadDTO photoDto)
    {
        try
        {
            // Vérifier que le compte existe
            var utilisateur = await _utilisateurRepository.GetByIdAsync(utilisateurId);
            if (utilisateur == null)
            {
                throw new NotFoundException($"Compte {utilisateurId} introuvable");
            }

            // Créer la photo
            var photo = await _photoRepository.AddPhotoAsync(photoDto);

            // Associer la photo au compte
            utilisateur.PhotoId = photo.PhotoId;
            await _utilisateurRepository.UpdateAsync(utilisateur);

            _logger.LogInformation("Photo {PhotoId} associée au compte {CompteId}", photo.PhotoId, utilisateurId);

            await transaction.CommitAsync();
            return photo;
        }catch(NotFoundException ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<Photo> UploadMessagePhotoAsync(PhotoDTO photoDto, int messageId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Validation métier
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null)
            {
                throw new NotFoundException($"Message {messageId} introuvable");
            }
    
            
            // Création de la photo
            var photo = await _photoRepository.AddPhotoAsync(photoDto);
    
            // Mise à jour de l'utilisateur
            var messageContientImage = new MessageContientImage
            {
                MessageId = messageId,
                PhotoId = photo.PhotoId
            };
            
            await _messageContientImageRepository.AddAsync(messageContientImage);
            await transaction.CommitAsync();
            return photo;
        }
        catch
        {
            PhotoId = photo.PhotoId,
            Url = $"/api/Medias/Photos/{photo.PhotoId}",
            FileName = photoDto.FileName,
            DateUpload = DateTime.UtcNow
        };
    }

    public async Task<bool> DeletePhotoAsync(int id)
    {
        var photo = await _photoRepository.GetByIdAsync(id);

        if (photo == null)
        {
            return false;
        }

        await _photoRepository.DeleteAsync(photo);

        _logger.LogInformation("Photo {PhotoId} supprimée", id);

        return true;
    }
}