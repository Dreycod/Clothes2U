using API.DTO.Annonce;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnonceController : ControllerBase
{
    private readonly IAnnonceRepository<Annonce, int> _annonceManager;
    private readonly IFavorisRepository  _favorisRepository;
    private readonly IMapper _mapper;

    public AnnonceController(IAnnonceRepository<Annonce, int> manager,IFavorisRepository favorisManager,  IMapper mapper)
    {
        _annonceManager = manager;
        _favorisRepository = favorisManager;
        _mapper = mapper;
    }
    
    [AllowAnonymous]
    [HttpGet("GetActiveAnnonces")]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetActiveAnnonces()
    {
        IEnumerable<Annonce> annonces = await _annonceManager.GetActiveAnnonces();
        var annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        
        if (User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                foreach (AnnonceDTO annonce in annoncesDTO)
                {
                    if (await _favorisRepository.CheckIfLiked(int.Parse(userIdClaim), annonce.AnnonceId))
                    {
                        annonce.IsLikedByCurrentUser = true;
                    }
                }
            }
        }
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

    [HttpGet("ByUtilisateurId/{utilisateurId}")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetAllByUtilisateurId(int utilisateurId)
    {
        IEnumerable<Annonce> annonces = await _annonceManager.GetByUtilisateurId(utilisateurId);
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
        return Ok(annonceDTO);
    }
    
    [HttpGet("ByFavorisUtilisateur/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetByFavorisUtilisateur(int id)
    {
        IEnumerable<Annonce> annonces = await _annonceManager.GetByUtilisateurFavoris(id);
        IEnumerable<AnnonceDTO> annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        return Ok(annoncesDTO);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(AnnonceDetailDTO), StatusCodes.Status201Created)]
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

    [HttpPost("Search")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> Search([FromBody] AnnonceSearchRequestDTO request)
    {
        var annonces = await _annonceManager.SearchAsync(request);
        var annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        return Ok(annoncesDTO);
    }

    [HttpGet("MostRecent")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetMostRecent()
    {
        var annonces = await _annonceManager.GetMostRecentAsync();
        var annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        return Ok(annoncesDTO);
    }

    [HttpGet("PlusLike")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetMostLiked()
    {
        var annonces = await _annonceManager.GetPlusLikeAsync();
        var annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        return Ok(annoncesDTO);
    }

    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAnnonce(int id) 
    {
        Annonce? annonceToDelete = await _annonceManager.GetByIdAsync(id);
        if (annonceToDelete == null)
        {
            return NotFound();
        }
        await _annonceManager.DeleteAsync(annonceToDelete);
        return NoContent();
    }
    

}