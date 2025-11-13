using API.DTO.Categorie;
using API.DTO.Taille;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class TailleController : ControllerBase
{
    private readonly IDataRepository<Taille, int> _tailleManager;
    private readonly IMapper _mapper;

    public TailleController(IDataRepository<Taille, int> manager, IMapper mapper)
    {
        _tailleManager = manager;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Taille>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TailleDTO>>> GetAllTaille()
    {
        IEnumerable<Taille> tailles =  await _tailleManager.GetAllAsync();
        IEnumerable<CategorieDTO> taillesDTO = _mapper.Map<IEnumerable<CategorieDTO>>(tailles);
        return Ok(taillesDTO);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Taille>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TailleDTO>>> GetAllTailleByCategorieId()
    {
        IEnumerable<Taille> tailles =  await _tailleManager.GetAllAsync();
        IEnumerable<CategorieDTO> taillesDTO = _mapper.Map<IEnumerable<CategorieDTO>>(tailles);
        return Ok(taillesDTO);
    }
}