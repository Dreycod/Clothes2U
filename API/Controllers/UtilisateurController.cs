using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using API.Models.Repository.Managers;
using API.Services;
using API.Services.Interfaces;
using API.Services.Interfaces;
using API.Services.VerificationSrvceV2;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTO;
using Shared.DTO.Annonce;
using Shared.DTO.Utilisateur;
using System.Net.Mail;


namespace API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UtilisateurController :  ControllerBase
{
    private readonly IUtilisateurRepository _utilisateurManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly INotificationRepository _notificationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserDeletionService _userDeletionService;
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceManager;
    private readonly IAnnonceExtensionService _annonceExtensionService;
    private readonly IMotInterditService _motInterditService;

    public UtilisateurController(
        IUtilisateurRepository utilisateurManager,
        ICurrentUserService currentUserService,
        IMapper mapper,
        INotificationRepository notificationRepository,
        IMessageRepository messageRepository,
        IUserDeletionService userDeletionService,
        IAnnonceRepository<Annonce, int, FilterDTO> annonceManager,
        IAnnonceExtensionService annonceExtensionService,
        IMotInterditService motInterditService
        )
    {
        _utilisateurManager = utilisateurManager;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _notificationRepository = notificationRepository;
        _messageRepository = messageRepository;
        _annonceExtensionService =  annonceExtensionService;
        _userDeletionService = userDeletionService;
        _annonceManager = annonceManager;
        _motInterditService = motInterditService;
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<UtilisateurViewDTO>> GetUtilisateur(int id)
    {
        Utilisateur? utilisateur = await _utilisateurManager.GetByIdAsync(id);
        if (utilisateur == null)
        {
            return NotFound();
        }
        UtilisateurViewDTO utilisateurDTO = _mapper.Map<UtilisateurViewDTO>(utilisateur);
        utilisateurDTO.FolloweddByCurrentUser = await _currentUserService.IsFollowedByCurrentUser(id);
        utilisateurDTO.BlockedByCurrentUser = await _currentUserService.IsBlockedByCurrentUser(id);
        return Ok(utilisateurDTO);
    }
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUtilisateur([FromBody] UtilisateurPutDTO utilisateurDTO)
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        if (userId != utilisateurDTO.UtilisateurId)
        {
            return Forbid();
        }
    
        Utilisateur utilisateurToUpdate = await _utilisateurManager.GetByIdAsync(userId);
        if (utilisateurToUpdate == null)
            return NotFound();
        _mapper.Map(utilisateurDTO, utilisateurToUpdate);
        await _utilisateurManager.UpdateAsync(utilisateurToUpdate);
    
        return NoContent();
    }
    [Authorize]
    [HttpPatch("PatchSettings")]
    public async Task<IActionResult> PatchUtilisateurSettings([FromBody] UtilisateurSettingsDTO settingsDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int userId = await _currentUserService.GetUserIdOrThrow();
        Utilisateur utilisateurToUpdate = await _utilisateurManager.GetByIdAsync(userId);

        if (utilisateurToUpdate == null)
            return NotFound(APIResponse<object>.ErrorResponse("Erreur du serveur, veuillez-vous reconnecter."));

        if (string.IsNullOrWhiteSpace(settingsDTO.Login))
        {
            settingsDTO.Login = utilisateurToUpdate.Login;
        }
        else if (settingsDTO.Login != utilisateurToUpdate.Login) 
        {
            var utilisateurByLogin = await _utilisateurManager.GetUtilisateurByLogin(settingsDTO.Login);
            if (utilisateurByLogin != null && utilisateurByLogin.UtilisateurId != userId)
            {
                return BadRequest(APIResponse<object>.ErrorResponse("Ce pseudo est déjà utilisé par un utilisateur"));
            }
        }

        if (String.IsNullOrWhiteSpace(settingsDTO.Description))
            settingsDTO.Description = utilisateurToUpdate.Description;

        if (string.IsNullOrWhiteSpace(settingsDTO.Email))
        {
            settingsDTO.Email = utilisateurToUpdate.Email;
        }
        else if (settingsDTO.Email != utilisateurToUpdate.Email)
        {
            try
            {
                _ = new System.Net.Mail.MailAddress(settingsDTO.Email);
                var utilisateurByEmail = await _utilisateurManager.GetUtilisateurByEmail(settingsDTO.Email);

                if (utilisateurByEmail != null && utilisateurByEmail.UtilisateurId != userId)
                {
                    return BadRequest(
                        APIResponse<object>.ErrorResponse("Cet email est déjà utilisé par un autre utilisateur")
                    );
                }
            }
            catch
            {
                return BadRequest(
                    APIResponse<object>.ErrorResponse("Votre mail n'est pas valide.")
                );
            }
        }

        // Vérification des mots interdits
        var isForbidden = await _motInterditService.ContientMotInterdit(settingsDTO.Login) ||
                         await _motInterditService.ContientMotInterdit(settingsDTO.Description);
        if (isForbidden)
        {
            return BadRequest(APIResponse<object>.ErrorResponse("Votre pseudo ou description contient un(des) mots interdits!"));
        }

        _mapper.Map(settingsDTO, utilisateurToUpdate);
        await _utilisateurManager.UpdateAsync(utilisateurToUpdate);

        return Ok(APIResponse<object>.SuccessResponse(null));
    }

    [HttpGet("GetSettings")]
    public async Task<ActionResult<UtilisateurSettingsDTO>> GetUtilisateurSettings()
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        Utilisateur? utilisateur = await _utilisateurManager.GetByIdAsync(userId);
        if (utilisateur == null)
            return NotFound();
        UtilisateurSettingsDTO settingsDTO = _mapper.Map<UtilisateurSettingsDTO>(utilisateur);
        return Ok(settingsDTO);
    }
    [Authorize]
    [HttpPut("notif-mail")]
    public async Task<IActionResult> UpdateNotifMailPreference(
    [FromBody] UpdateNotifMailDTO dto)
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        Utilisateur user = await _utilisateurManager.GetByIdAsync(userId);
        if (!user.ValidEmail)
            return BadRequest("Email non v�rifi�");
        user.PreferenceNotifMail = dto.PreferenceNotifMail;
        await _utilisateurManager.UpdateAsync(user);
        return NoContent();
    }
    [HttpDelete("suppressionCompte")]
    public async Task<IActionResult> SuppressionCompte([FromBody] AccountDeletionDTO accountDeletionDTO)
    {
        int userId = await _currentUserService.GetUserIdOrThrow();

        Utilisateur utilisateur = await _utilisateurManager.GetByIdAsync(userId);
        if (utilisateur == null)
        {
            return NotFound();
        }

        if (!BCrypt.Net.BCrypt.Verify(accountDeletionDTO.Password, utilisateur.Password))
        {
            return Unauthorized(APIResponse<object>.ErrorResponse("Votre mot de passe est incorrecte!"));
        }

        // get annonces
        var annonces = await _annonceExtensionService.GetAnnoncesByUserId(userId); 
        foreach (var annonce in annonces)
        {
            if (annonce.StatutAnnonceId == 1)
            {
                var annonceToDelete = await _annonceManager.GetByIdAsync(annonce.AnnonceId);
                await _annonceManager.DeleteAsync(annonceToDelete);
            }
        }

        await _userDeletionService.DeleteUtilisateurAsync(userId);
        Response.Cookies.Delete("authToken");
        // deconnexion, bye bye user

        return Ok(APIResponse<object>.SuccessResponse(null));
    }

    [HttpGet("login/{login}")]
    [AllowAnonymous]
    public async Task<ActionResult<UtilisateurViewDTO>> GetByLogin(string login)
    {
        Utilisateur? utilisateur = await _utilisateurManager.GetUtilisateurByLogin(login);
        if (utilisateur == null)
            return NotFound();

        UtilisateurViewDTO utilisateurDTO = _mapper.Map<UtilisateurViewDTO>(utilisateur);
        utilisateurDTO.FolloweddByCurrentUser = await _currentUserService.IsFollowedByCurrentUser(utilisateurDTO.UtilisateurId);
        utilisateurDTO.BlockedByCurrentUser = await _currentUserService.IsBlockedByCurrentUser(utilisateurDTO.UtilisateurId);
        return Ok(utilisateurDTO);
    }

    [HttpGet("notificationAndMessagesCount")]
    [Authorize]
    public async Task<ActionResult<NewsDTO>> NotificationAndMessagesCount()
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        NewsDTO returnObject = new NewsDTO();
        returnObject.MessagesCount = await _messageRepository.GetMessageCountByUserId(userId);
        returnObject.NotificationsCount = await _notificationRepository.GetNotificationsUnreadCountByUserId(userId);
        return Ok(returnObject);
    }
}