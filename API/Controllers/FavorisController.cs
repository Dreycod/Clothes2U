using API.DTO.Favoris;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class FavorisController :  ControllerBase
{
    private readonly IFavorisRepository _favorisManager;
    private readonly IMapper _mapper;
    
    public FavorisController(IFavorisRepository manager, IMapper mapper)
    {
        _favorisManager = manager;
        _mapper = mapper;
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
    [ProducesResponseType(typeof(FavorisDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FavorisDTO>> AddFavoris(FavorisDTO favorisDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Favoris favoris =  _mapper.Map<Favoris>(favorisDto);
        await _favorisManager.AddAsync(favoris);
        return CreatedAtAction( nameof(GetById), new { id = favoris.FavorisId }, favoris);
    }
    [HttpDelete("id/{annonceId}/{utilisateurId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduit(int annonceId, int utilisateurId)
    {
        Favoris? favorisToDelete = await _favorisManager.GetFavorisByAnnonceAndUserId(utilisateurId, annonceId);
        if (favorisToDelete == null)
        {
            return NotFound();
        }
        await _favorisManager.DeleteAsync(favorisToDelete);
        return NoContent();
    }
    
}