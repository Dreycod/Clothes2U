using API.Models.EntityFramework;
using API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
 
namespace API.Controllers;



[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class CategorieController : ControllerBase
{
    private readonly IDataRepository<Categorie, int> _categorieManager;

    public CategorieController(IDataRepository<Categorie, int> manager)
    {
        _categorieManager = manager;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Categorie>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Categorie>>> GetAll()
    {
        return  Ok(await _categorieManager.GetAllAsync());
    }
    
}