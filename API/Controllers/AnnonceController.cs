using API.DTO.Annonce;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class AnnonceController : ControllerBase
{
    private readonly IAnnonceRepository<Annonce, int> _annonceManager;
    private readonly IMapper _mapper;

    public AnnonceController(IAnnonceRepository<Annonce, int> manager, IMapper mapper)
    {
        _annonceManager = manager;
        _mapper = mapper;
    }
    
    [HttpGet("GetActiveAnnonces")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetActiveAnnonces()
    {
        IEnumerable<Annonce> annonces = await _annonceManager.GetActiveAnnonces();
        IEnumerable<AnnonceDTO> annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        return Ok(annoncesDTO);
    }

    
    [HttpGet("ByCategorieId/{categorieId}")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetAllByCategorieId(int  categorieId)
    {
        IEnumerable<Annonce> annonces =  await _annonceManager.GetByCategorieId(categorieId);
        IEnumerable<AnnonceDTO> annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        return Ok(annoncesDTO);
    }
    [HttpGet("BySousCategorieId/{categorieId}")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetAllBySousCategorieId(int  sousCategorieId)
    {
        IEnumerable<Annonce> annonces =  await _annonceManager.GetBySousCategorieId(sousCategorieId);
        IEnumerable<AnnonceDTO> annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        return Ok(annoncesDTO);
    }
    
    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(AnnonceDetailDTO),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AnnonceDetailDTO>> GetById(int id)
    {
        var annonce = await _annonceManager.GetByIdAsync(id);
        if (annonce == null)
            return NotFound();
        
        AnnonceDetailDTO annonceDTO = _mapper.Map<AnnonceDetailDTO>(annonce);
        return annonceDTO;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(Annonce), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AnnonceDetailDTO>> AddAnnonce(AnnonceDetailDTO annonceDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var annonce =  _mapper.Map<Annonce>(annonceDto);
        await _annonceManager.AddAsync(annonce);
        AnnonceDetailDTO resultDto = _mapper.Map<AnnonceDetailDTO>(annonce);
        return CreatedAtAction( nameof(GetById), new { id = annonce.AnnonceId }, resultDto);
    }
}