using API.DTO.Utilisateur;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UtilisateurController :  ControllerBase
{
    private readonly IUtilisateurRepository _utilisateurManager;
    private readonly IAbonnementRepository<Abonnement, int>  _abonnementManager; 
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UtilisateurController(IUtilisateurRepository utilisateurManager, IAbonnementRepository<Abonnement, int> abonnementManager,ICurrentUserService currentUserService, IMapper mapper)
    {
        _abonnementManager =  abonnementManager;
        _utilisateurManager = utilisateurManager;
        _mapper = mapper;
        _currentUserService = currentUserService;
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
        utilisateurDTO.followeddByCurrentUser = await _currentUserService.IsFollowedByCurrentUser(id);
        utilisateurDTO.BlockedByCurrentUser = await _currentUserService.IsBlockedByCurrentUser(id);
        return Ok(utilisateurDTO);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutUtilisateur(int id, [FromBody] UtilisateurPutDTO utilisateurDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        Utilisateur utilisateurToUpdate = await _utilisateurManager.GetByIdAsync(id);
        if (utilisateurToUpdate == null)
            return NotFound();
        _mapper.Map(utilisateurDTO, utilisateurToUpdate);
        await _utilisateurManager.UpdateAsync(utilisateurToUpdate, utilisateurToUpdate);
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
}