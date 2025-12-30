using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.DTO.Categorie;
 
namespace API.Controllers;



[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class CategorieController : ControllerBase
{
    private readonly ICaracteristiquesRepository<Categorie> _categorieManager;
    private readonly IMapper _mapper;

    public CategorieController(ICaracteristiquesRepository<Categorie> manager, IMapper mapper)
    {
        _categorieManager = manager;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Categorie>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CategorieDTO>>> GetAllCategorieWithNavigation()
    {
        IEnumerable<Categorie> categories =  await _categorieManager.GetAllAsync();
        IEnumerable<CategorieDTO> categoriesDTO = _mapper.Map<IEnumerable<CategorieDTO>>(categories);
        return Ok(categoriesDTO);
    }
    [HttpGet("detail")]
    [ProducesResponseType(typeof(IEnumerable<Categorie>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CategorieDTO>>> GetAllCategorieDetails()
    {
        IEnumerable<Categorie> categories = await _categorieManager.GetAllWithDetailsAsync();
        IEnumerable<CategorieDTO> categoriesDTO = _mapper.Map<IEnumerable<CategorieDTO>>(categories);
        return Ok(categoriesDTO);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Categorie), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Categorie>> AddCategorie([FromBody] CategorieDTO Categorie)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Categorie _Categorie = _mapper.Map<Categorie>(Categorie);

        await _categorieManager.AddAsync(_Categorie);
        return CreatedAtAction(nameof(GetById), new { id = _Categorie.CategorieId }, _Categorie);
    }
    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCategorie(int id)
    {
        Categorie? CategorieToDelete = await _categorieManager.GetByIdAsync(id);
        if (CategorieToDelete == null)
        {
            return NotFound();
        }
        await _categorieManager.DeleteAsync(CategorieToDelete);
        return NoContent();
    }
    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutCategorie(int id, [FromBody] CategorieDTO Categorie)
    {
        if (id != Categorie.IdCategorie)
        {
            return BadRequest();
        }
        ActionResult<Categorie?> CategorieToUpdate = await _categorieManager.GetByIdAsync(id);

        if (CategorieToUpdate.Value == null)
        {
            return NotFound();
        }
        Categorie updatedCategorie = _mapper.Map<Categorie>(Categorie);

        await _categorieManager.UpdateAsync(updatedCategorie);
        return NoContent();
    }

    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(Categorie), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Categorie>> GetById(int id)
    {
        var Categorie = await _categorieManager.GetByIdAsync(id);
        if (Categorie == null)
            return NotFound();
        return Ok(Categorie);
    }
}