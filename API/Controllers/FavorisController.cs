using API.Models.EntityFramework;
using API.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class FavorisController :  ControllerBase
{
    private readonly IDataRepository<Favoris, int> _favorisManager;
    
    public FavorisController(IDataRepository<Favoris, int> favorisManager)
    {
        _favorisManager = favorisManager;
    }
    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(Favoris),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Favoris>> GetById(int id)
    {
        Favoris favoris = await _favorisManager.GetByIdAsync(id);
        if (favoris == null)
            return NotFound();
        return favoris;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(Favoris), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Favoris>> AddFavoris(Favoris favoris)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _favorisManager.AddAsync(favoris);
        return CreatedAtAction( nameof(GetById), new { id = favoris.FavorisId }, favoris);
    }
    
}