using Shared.DTO.Categorie;
using Shared.DTO.Taille;
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
    private readonly ITailleRepository _tailleManager;
    private readonly IMapper _mapper;

    public TailleController(ITailleRepository manager, IMapper mapper)
    {
        _tailleManager = manager;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TailleDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TailleDTO>>> GetAllTaille()
    {
        IEnumerable<Taille> tailles =  await _tailleManager.GetAllAsync();
        IEnumerable<TailleDTO> taillesDTO = _mapper.Map<IEnumerable<TailleDTO>>(tailles);
        return Ok(taillesDTO);
    }
    
    [HttpGet("byCategoryId/{id}")]
    [ProducesResponseType(typeof(IEnumerable<Taille>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TailleDTO>>> GetAllTailleByCategorieId(int id)
    {
        IEnumerable<Taille> tailles =  await _tailleManager.GetAllAsyncByIdentifier(id);
        IEnumerable<TailleDTO> taillesDTO = _mapper.Map<IEnumerable<TailleDTO>>(tailles);
        return Ok(taillesDTO);
    }
}