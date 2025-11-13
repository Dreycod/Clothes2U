using API.DTO.Categorie;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
 
namespace API.Controllers;



[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class CategorieController : ControllerBase
{
    private readonly IDataRepository<Categorie, int> _categorieManager;
    private readonly IMapper _mapper;

    public CategorieController(IDataRepository<Categorie, int> manager, IMapper mapper)
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
    
}