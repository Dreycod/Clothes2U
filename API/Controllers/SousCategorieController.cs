using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.SousCategorie;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class SousCategorieController : ControllerBase
{
    private readonly IDataRepository<SousCategorie, int> _sousCategorieManager;
    private readonly IMapper _mapper;

    public SousCategorieController(IDataRepository<SousCategorie, int> manager, IMapper mapper)
    {
        _sousCategorieManager = manager;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SousCategorie>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SousCategorieDTO>>> GetAll()
    {
        IEnumerable<SousCategorie> categories = await _sousCategorieManager.GetAllAsync();
        IEnumerable<SousCategorieDTO> categoriesDTO = _mapper.Map<IEnumerable<SousCategorieDTO>>(categories);
        return Ok(categoriesDTO);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SousCategorie), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SousCategorie>> AddSousCategorie([FromBody] SousCategorieDTO SousCategorie)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        SousCategorie _SousCategorie = _mapper.Map<SousCategorie>(SousCategorie);

        await _sousCategorieManager.AddAsync(_SousCategorie);
        return CreatedAtAction(nameof(GetById), new { id = _SousCategorie.SousCategorieId }, _SousCategorie);
    }
    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSousCategorie(int id)
    {
        SousCategorie? SousCategorieToDelete = await _sousCategorieManager.GetByIdAsync(id);
        if (SousCategorieToDelete == null)
        {
            return NotFound();
        }
        await _sousCategorieManager.DeleteAsync(SousCategorieToDelete);
        return NoContent();
    }
    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutSousCategorie(int id, [FromBody] SousCategorieDTO SousCategorie)
    {
        if (id != SousCategorie.SousCategorieId)
        {
            return BadRequest();
        }
        ActionResult<SousCategorie?> SousCategorieToUpdate = await _sousCategorieManager.GetByIdAsync(id);

        if (SousCategorieToUpdate.Value == null)
        {
            return NotFound();
        }
        SousCategorie updatedSousCategorie = _mapper.Map<SousCategorie>(SousCategorie);

        await _sousCategorieManager.UpdateAsync(updatedSousCategorie);
        return NoContent();
    }

    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(SousCategorie), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SousCategorie>> GetById(int id)
    {
        var SousCategorie = await _sousCategorieManager.GetByIdAsync(id);
        if (SousCategorie == null)
            return NotFound();
        return Ok(SousCategorie);
    }

}