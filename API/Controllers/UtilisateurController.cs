using API.DTO.Utilisateur;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UtilisateurController :  ControllerBase
{
    private readonly IUtilisateurRepository _utilisateurManager;
    private readonly IMapper _mapper;

    public UtilisateurController(IUtilisateurRepository utilisateurManager, IMapper mapper)
    {
        _utilisateurManager = utilisateurManager;
        _mapper = mapper;
    }
   

    [HttpGet("utilisateur/{id}")]
    public async Task<ActionResult<UtilisateurViewDTO>> GetUtilisateur(int id)
    {
        Utilisateur? utilisateur = await _utilisateurManager.GetByIdAsync(id);
        UtilisateurViewDTO utiliateurDTO = _mapper.Map<UtilisateurViewDTO>(utilisateur);
        return Ok(utiliateurDTO);
    }

    [HttpPut("utilisateur/{id}")]
    public async Task<IActionResult> PutUtilisateur(int id, UtilisateurPutDTO utilisateurDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Utilisateur utilisateurToUpdate = await _utilisateurManager.GetByIdAsync(id);
        if (utilisateurToUpdate == null)
        {
            return NotFound();
        }
        Utilisateur utilisateur = _mapper.Map<Utilisateur>(utilisateurDTO);
        await _utilisateurManager.UpdateAsync(utilisateurToUpdate, utilisateur);
        return NoContent();
    }
    

    [HttpDelete("id/{id}")]
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