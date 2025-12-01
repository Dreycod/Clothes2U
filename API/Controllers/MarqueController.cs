using API.Models.EntityFramework;
using API.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class MarqueController : ControllerBase
{
    private readonly IDataRepository<Marque, int> _marqueManager;

    public MarqueController(IDataRepository<Marque, int> manager)
    {
        _marqueManager= manager;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Marque>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Marque>> GetAllMarques()
    {
        IEnumerable<Marque> marques = await _marqueManager.GetAllAsync();
        return Ok(marques);
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
    public async Task<ActionResult<Marque>> AddAnnonce(Marque marque)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _marqueManager.AddAsync(marque);
        return CreatedAtAction( nameof(GetById), new { id = marque.MarqueId }, marque);
    }
    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAnnonce(int id) 
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
    public async Task<IActionResult> PutMarque(int id, [FromBody] Marque brand)
    {
        if (id != brand.MarqueId)
        {
            return BadRequest();
        }
        ActionResult<Marque?> brandToUpdate = await _marqueManager.GetByIdAsync(id);

        if (brandToUpdate.Value == null)
        {
            return NotFound();
        }
        await _marqueManager.UpdateAsync(brandToUpdate.Value, brand);
        return NoContent();
    }
}