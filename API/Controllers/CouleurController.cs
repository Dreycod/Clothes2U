using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Categorie;
using Shared.DTO.Couleur;
using Shared.DTO.Couleur;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class CouleurController : ControllerBase
{
    private readonly ICaracteristiquesRepository<Couleur> _couleurManager;
    private readonly IMapper _mapper;

    public CouleurController(ICaracteristiquesRepository<Couleur> couleurManager, IMapper mapper)
    {
        _couleurManager = couleurManager;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Couleur>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CouleurDTO>>> GetAllCouleurs()
    {
        IEnumerable<Couleur> couleurs =  await _couleurManager.GetAllWithDetailsAsync();
        IEnumerable<CouleurDTO> couleursDTO = _mapper.Map<IEnumerable<CouleurDTO>>(couleurs);
        return Ok(couleursDTO);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Commercial")]
    [ProducesResponseType(typeof(Couleur), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Couleur>> AddCouleur([FromBody] CouleurDTO Couleur)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Couleur _Couleur = _mapper.Map<Couleur>(Couleur);

        await _couleurManager.AddAsync(_Couleur);
        return CreatedAtAction(nameof(GetById), new { id = _Couleur.CouleurId }, _Couleur);
    }
    [HttpDelete("id/{id}")]
    [Authorize(Roles = "Admin,Commercial")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCouleur(int id)
    {
        Couleur? CouleurToDelete = await _couleurManager.GetByIdAsync(id);
        if (CouleurToDelete == null)
        {
            return NotFound();
        }
        await _couleurManager.DeleteAsync(CouleurToDelete);
        return NoContent();
    }
    [HttpPut("id/{id}")]
    [Authorize(Roles = "Admin,Moderateur")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutCouleur(int id, [FromBody] CouleurDTO couleur)
    {
        if (id != couleur.CouleurId)
        {
            return BadRequest();
        }
        ActionResult<Couleur?> couleurToUpdate = await _couleurManager.GetByIdAsync(id);

        if (couleurToUpdate.Value == null)
        {
            return NotFound();
        }
        Couleur updatedcouleur = _mapper.Map<Couleur>(couleur);

        await _couleurManager.UpdateAsync(updatedcouleur);
        return NoContent();
    }

    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(Couleur), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Couleur>> GetById(int id)
    {
        var Couleur = await _couleurManager.GetByIdAsync(id);
        if (Couleur == null)
            return NotFound();
        return Ok(Couleur);
    }
}