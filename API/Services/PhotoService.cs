using API.Exceptions;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using API.Models.Repository.Managers;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Moderation;
using Shared.DTO.Photo;
using System.Numerics;
using Twilio.Http;

namespace API.Services;

public class PhotoService : IPhotoService
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IDataRepository<Annonce, int> _annonceRepository;
    private readonly IllustreAnnonceRepository<Illustre_Annonce, int> _illustreAnnonceManager;
    private readonly IDataRepository<Utilisateur, int> _utilisateurRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IDataRepository<MessageContientImage, int> _messageContientImageRepository;
    private readonly ILogger<PhotoService> _logger;

    public PhotoService(
        IPhotoRepository photoRepository,
        IDataRepository<Annonce, int> annonceRepository,
        IDataRepository<Utilisateur, int> utilisateurRepository,
        IMessageRepository messageRepository,
        IllustreAnnonceRepository<Illustre_Annonce, int> illustreAnnonceManger,
        IDataRepository<MessageContientImage, int> messageContientImageRepository,
        ILogger<PhotoService> logger)
    {
        _photoRepository = photoRepository;
        _annonceRepository = annonceRepository;
        _utilisateurRepository = utilisateurRepository;
        _messageRepository = messageRepository;
        _illustreAnnonceManager = illustreAnnonceManger;    
        _messageContientImageRepository = messageContientImageRepository;
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
            DateUpload = DateTime.UtcNow,
            EnAttenteValidation = photo.EnAttenteValidation
        };
    }
    public async Task<List<PhotoDTO>> GetAllPhotosValidation()
    {
        try
        {
            List<PhotoDTO> validationResponse = new List<PhotoDTO>();

            var photos = await _photoRepository.GetAllAsync();
            // filter those with AttenteValidation == true
            photos = photos.Where(p => p.EnAttenteValidation == true).ToList();

            foreach (var photo in photos)
            {
                PhotoDTO photoDto = new PhotoDTO
                {
                    PhotoId = photo.PhotoId,
                    EnAttenteValidation = photo.EnAttenteValidation,
                    Image = photo.Image
                };
                validationResponse.Add(photoDto);
            }

            return validationResponse;
        }
        catch (Exception exception)
        {
            //await transaction.RollbackAsync();
            throw exception;
        }
    }

    public async Task<ValidationResponseDTO> ValidationImageAsync(bool Reponse, int Photoid)
    {
        try
        {

            ValidationResponseDTO validationResponse = new ValidationResponseDTO();
            validationResponse.PhotoId = Photoid;
            validationResponse.IsValid = Reponse;
            validationResponse.Success = false;

            var photo = await _photoRepository.GetByIdAsync(Photoid);
            if (photo == null)
            {
                _logger.LogInformation("Photo {PhotoId} n'existe pas", Photoid);
                return validationResponse;
            }

            Illustre_Annonce? illustreAnnonce = await _illustreAnnonceManager.GetByPhotoId(Photoid);
            if (illustreAnnonce == null)
            {
                _logger.LogInformation("Illustre Annonce pour PhotoID: {PhotoId} n'existe pas", Photoid);
                return validationResponse;
            }

            Annonce? _Annonce = await _annonceRepository.GetByIdAsync(illustreAnnonce.AnnonceId);
            if (_Annonce == null)
            {
                _logger.LogInformation("Annonce pour PhotoID: {PhotoId} n'existe pas", Photoid);
                return validationResponse;
            }

            if (Reponse)
            {
                _logger.LogInformation("Photo {PhotoId} est validé", Photoid);
                photo.EnAttenteValidation = false;
                await _photoRepository.UpdateAsync(photo);
                

            }
            else
            {
                _logger.LogInformation("Photo {PhotoId} n'est pas validé", Photoid);
                photo.EnAttenteValidation = null;
                await _photoRepository.DeleteAsync(photo);


            }

            // Vérifier si toutes les photos de l'annonce sont validées
            var photosAnnonce = _Annonce.Photos;
            bool allValidated = true;

            foreach (var illustre in photosAnnonce)
            {
                var p = await _photoRepository.GetByIdAsync(illustre.PhotoId);
                if (p != null && p.EnAttenteValidation == true)
                {
                    allValidated = false;
                    break;
                }
            }

            if (allValidated)
            {
                _Annonce.StatutAnnonceId = 1;
                await _annonceRepository.UpdateAsync(_Annonce);
                _logger.LogInformation("Annonce {AnnonceId} est validée car toutes les photos sont validées", _Annonce.AnnonceId);
            }

            validationResponse.Success = true;
            return validationResponse;
        }
        catch (Exception exception)
        {
            //await transaction.RollbackAsync();
            throw exception;
        }
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
            //await transaction.CommitAsync();
            return photo;
        }
        catch(Exception exception)
        {
            //await transaction.RollbackAsync();
            throw exception;
        }
    }
    
    public async Task<PhotoResponseDTO> UploadMessagePhotoAsync(PhotoUploadDTO photoDto)
    {
        try
        {
            var photo = await _photoRepository.AddPhotoAsync(photoDto);
    
            
            return photo;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw; 
        }
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