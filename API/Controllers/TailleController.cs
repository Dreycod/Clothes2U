using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Categorie;
using Shared.DTO.Taille;
using Shared.DTO.Mesures;

namespace API.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class TailleController : ControllerBase
{
    private readonly ITailleRepository _tailleManager;
    private readonly ICaracteristiquesRepository<Mesure> _mesureRepository;
    private readonly IMapper _mapper;

    public TailleController(ITailleRepository manager, IMapper mapper, ICaracteristiquesRepository<Mesure> mesureRepository)
    {
        _tailleManager = manager;
        _mapper = mapper;
        _mesureRepository = mesureRepository;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TailleDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TailleDTO>>> GetAllTaille()
    {
        IEnumerable<Taille> tailles =  await _tailleManager.GetAllWithDetailsAsync();
        IEnumerable<TailleDTO> taillesDTO = _mapper.Map<IEnumerable<TailleDTO>>(tailles);
        return Ok(taillesDTO);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Taille), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Taille>> AddTaille([FromBody] TailleDTO Taille)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Taille _Taille = _mapper.Map<Taille>(Taille);

        await _tailleManager.AddAsync(_Taille);
        return CreatedAtAction(nameof(GetById), new { id = _Taille.TailleId }, _Taille);
    }
    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTaille(int id)
    {
        Taille? TailleToDelete = await _tailleManager.GetByIdAsync(id);
        if (TailleToDelete == null)
        {
            return NotFound();
        }
        await _tailleManager.DeleteAsync(TailleToDelete);
        return NoContent();
    }
    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutTaille(int id, [FromBody] TailleDTO Taille)
    {
        if (id != Taille.TailleId)
        {
            return BadRequest();
        }
        ActionResult<Taille?> TailleToUpdate = await _tailleManager.GetByIdAsync(id);

        if (TailleToUpdate.Value == null)
        {
            return NotFound();
        }
        Taille updatedTaille = _mapper.Map<Taille>(Taille);

        await _tailleManager.UpdateAsync(updatedTaille);
        return NoContent();
    }

    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(Taille), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Taille>> GetById(int id)
    {
        var Taille = await _tailleManager.GetByIdAsync(id);
        if (Taille == null)
            return NotFound();
        return Ok(Taille);
    }

    [HttpPut("id/{id}/mesures")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutTailleMesures(int id, [FromBody] List<MesureDTO> MesuresDTO)
    {
        if (id != MesuresDTO.First().TailleId)
        {
            return BadRequest();
        }

        List<Mesure> Mesures = _mapper.Map<List<Mesure>>(MesuresDTO);                                                                                                      

        IEnumerable<Mesure> result = await _tailleManager.PutTailleMesuresAsync(id,Mesures);

        if (result == null)
        {
            return NotFound();
        }
        List<MesureDTO> _mesuresDTO = _mapper.Map<List<MesureDTO>>(result);

        return NoContent();
    }
}