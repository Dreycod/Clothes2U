using API.DTO.Categorie;
using API.DTO.Couleur;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class CouleurController : ControllerBase
{
    private readonly IDataRepository<Couleur, int> _couleurManager;
    private readonly IMapper _mapper;

    public CouleurController(IDataRepository<Couleur, int> couleurManager, IMapper mapper)
    {
        _couleurManager = couleurManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Couleur>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CouleurDTO>>> GetAllCouleurs()
    {
        IEnumerable<Couleur> couleurs =  await _couleurManager.GetAllAsync();
        IEnumerable<CouleurDTO> couleursDTO = _mapper.Map<IEnumerable<CouleurDTO>>(couleurs);
        return Ok(couleursDTO);
    }
}