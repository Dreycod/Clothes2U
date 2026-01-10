using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using API.Services;
using API.Services.Interfaces;
using API.Services.VerificationSrvceV2;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTO;
using Shared.DTO.Utilisateur;


namespace API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UtilisateurController :  ControllerBase
{
    private readonly IUtilisateurRepository _utilisateurManager;
    private readonly IAbonnementRepository<Abonnement, int>  _abonnementManager; 
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly INotificationMailService _mailService;
    private readonly INotificationRepository _notificationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserDeletionService _userDeletionService;

    public UtilisateurController(IUtilisateurRepository utilisateurManager, IAbonnementRepository<Abonnement, int> abonnementManager,ICurrentUserService currentUserService, IMapper mapper, INotificationMailService mailService, INotificationRepository notificationRepository,
    IMessageRepository messageRepository, IUserDeletionService userDeletionService)
    {
        _abonnementManager =  abonnementManager;
        _utilisateurManager = utilisateurManager;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _mailService = mailService;
        _notificationRepository = notificationRepository;
        _messageRepository = messageRepository;
        _userDeletionService = userDeletionService;
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
        int oldStatut = utilisateurToUpdate.StatutId;
    
        _mapper.Map(utilisateurDTO, utilisateurToUpdate);
        await _utilisateurManager.UpdateAsync(utilisateurToUpdate);
        await _mailService.NotifyUserStatusChangedAsync(utilisateurToUpdate, oldStatut);
    
        return NoContent();
    }
    [Authorize]
    [HttpPatch("{id}/PatchSettings")]
    public async Task<IActionResult> PatchUtilisateurSettings(int id, [FromBody] UtilisateurSettingsDTO settingsDTO)
    {
        if ((await _currentUserService.GetUserId()) != id)
            return Forbid();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        Utilisateur utilisateurToUpdate = await _utilisateurManager.GetByIdAsync(id);
        if (utilisateurToUpdate == null)
            return NotFound();
    
        _mapper.Map(settingsDTO, utilisateurToUpdate);
        await _utilisateurManager.UpdateAsync(utilisateurToUpdate);
        return NoContent();
    }

    [HttpGet("{id}/GetSettings")]
    public async Task<ActionResult<UtilisateurSettingsDTO>> GetUtilisateurSettings(int id)
    {
        Utilisateur? utilisateur = await _utilisateurManager.GetByIdAsync(id);
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

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUtilisateur(int id)
    {
        int adminId = await _currentUserService.GetUserIdOrThrow();
        await _userDeletionService.DeleteUtilisateurByAdminAsync(id, adminId);
        return NoContent();
    }

    [Authorize]
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

        await _utilisateurManager.DeleteAsync(utilisateur);
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