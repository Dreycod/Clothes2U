using API.DTO.SousCategorie;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

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
}