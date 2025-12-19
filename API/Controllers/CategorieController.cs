using Shared.DTO;
using Shared.DTO.Categorie;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
 
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

    [HttpGet("details")]
    [ProducesResponseType(typeof(IEnumerable<CategorieDetailDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategorieDetailDTO>> GetAllCategoriesWithDetails()
    {
        IEnumerable<Categorie> categories = await _categorieManager.GetAllWithDetailsAsync();
        IEnumerable<CategorieDetailDTO> categoriesdetailDTO = _mapper.Map<IEnumerable<CategorieDetailDTO>>(categories);
        return Ok(categoriesdetailDTO);
    }

}