using Shared.DTO.Utilisateur;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Services.VerificationSrvceV2;

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

    public UtilisateurController(IUtilisateurRepository utilisateurManager, IAbonnementRepository<Abonnement, int> abonnementManager,ICurrentUserService currentUserService, IMapper mapper, INotificationMailService mailService)
    {
        _abonnementManager =  abonnementManager;
        _utilisateurManager = utilisateurManager;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _mailService = mailService;
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

    [HttpPut("{id}")]
    public async Task<IActionResult> PutUtilisateur(int id, [FromBody] UtilisateurPutDTO utilisateurDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        Utilisateur utilisateurToUpdate = await _utilisateurManager.GetByIdAsync(id);
        int oldStatut = utilisateurToUpdate.StatutId;
        if (utilisateurToUpdate == null)
            return NotFound();
        _mapper.Map(utilisateurDTO, utilisateurToUpdate);
        await _utilisateurManager.UpdateAsync(utilisateurToUpdate);
        await _mailService.NotifyUserStatusChangedAsync(utilisateurToUpdate, oldStatut);
        return NoContent();
    }
    [HttpPatch("{id}/PatchSettings")]
    public async Task<IActionResult> PatchUtilisateurSettings(int id, [FromBody] UtilisateurSettingsDTO putDTO)
    {
        if ((await _currentUserService.GetUserId()) != id)
            return Forbid();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        Utilisateur utilisateurToUpdate = await _utilisateurManager.GetByIdAsync(id);
        if (utilisateurToUpdate == null)
            return NotFound();
        _mapper.Map(putDTO, utilisateurToUpdate);
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
    [HttpPut("{id}/notif-mail")]
    public async Task<IActionResult> UpdateNotifMailPreference(
    int id,
    [FromBody] UpdateNotifMailDTO dto)
    {
        // Sécurité : seul l'utilisateur lui-même
        if ((await _currentUserService.GetUserId()) != id)
            return Forbid();

        var utilisateur = await _utilisateurManager.GetByIdAsync(id);
        if (utilisateur == null)
            return NotFound();

        if (!utilisateur.ValidEmail)
            return BadRequest("Email non vérifié");

        utilisateur.PreferenceNotifMail = dto.PreferenceNotifMail;
        await _utilisateurManager.UpdateAsync(utilisateur);

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUtilisateur(int id)
    {
        Utilisateur utilisateur = await _utilisateurManager.GetByIdAsync(id);
        if (utilisateur == null)
        {
            return NotFound();
        }
        await _utilisateurManager.DeleteAsync(utilisateur);
        return NoContent();
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
}