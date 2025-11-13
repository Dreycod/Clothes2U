using API.DTO.StatutAnnonce;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class StatutAnnonceController : ControllerBase
{
    private readonly IDataRepository<StatutAnnonce, int> _statutAnnonceManager;
    private readonly IMapper _mapper;

    public StatutAnnonceController(IDataRepository<StatutAnnonce, int> manager, IMapper mapper)
    {
        _statutAnnonceManager = manager;
        _mapper = mapper;
    }
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StatutAnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<StatutAnnonceDTO>>> GetAllCategorieWithNavigation()
    {
        IEnumerable<StatutAnnonce> statutAnnonces =  await _statutAnnonceManager.GetAllAsync();
        IEnumerable<StatutAnnonceDTO> statutAnnoncesDTO = _mapper.Map<IEnumerable<StatutAnnonceDTO>>(statutAnnonces);
        return Ok(statutAnnoncesDTO);
    }
}