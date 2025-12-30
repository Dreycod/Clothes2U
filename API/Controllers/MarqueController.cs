using Shared.DTO.Marque;
using Shared.DTO.Taille;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class MarqueController : ControllerBase
{
    private readonly ICaracteristiquesRepository<Marque> _marqueManager;
    private readonly IMapper _mapper;

    public MarqueController(ICaracteristiquesRepository<Marque> manager, IMapper mapper)
    {
        _marqueManager= manager;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MarqueDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MarqueDTO>> GetAllMarques()
    {
        IEnumerable<Marque> marques = await _marqueManager.GetAllWithDetailsAsync();
        IEnumerable<MarqueDTO> marquesDTO = _mapper.Map<IEnumerable<MarqueDTO>>(marques);
        return Ok(marquesDTO);
    }

    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(Marque),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Marque>> GetById(int id)
    {
        var marque = await _marqueManager.GetByIdAsync(id);
        if (marque == null)
            return NotFound();
        return Ok(marque);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(Marque), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Marque>> AddMarque([FromBody] MarqueDTO marque)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Marque _marque = _mapper.Map<Marque>(marque);

        await _marqueManager.AddAsync(_marque);
        return CreatedAtAction( nameof(GetById), new { id = _marque.MarqueId }, _marque);
    }
    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMarque(int id) 
    {
        Marque? marqueToDelete = await _marqueManager.GetByIdAsync(id);
        if (marqueToDelete == null)
        {
            return NotFound();
        }
        await _marqueManager.DeleteAsync(marqueToDelete);
        return NoContent();
    }
    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutMarque(int id, [FromBody] MarqueDTO brand)
    {
        if (id != brand.MarqueID)
        {
            return BadRequest();
        }
        ActionResult<Marque?> brandToUpdate = await _marqueManager.GetByIdAsync(id);

        if (brandToUpdate.Value == null)
        {
            return NotFound();
        }
        Marque updatedBrand = _mapper.Map<Marque>(brand);

        await _marqueManager.UpdateAsync(updatedBrand);
        return NoContent();
    }
}