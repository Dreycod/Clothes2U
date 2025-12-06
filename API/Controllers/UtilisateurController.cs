using API.DTO.Utilisateur;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
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
}