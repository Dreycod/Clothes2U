using API.DTO;
using API.DTO.Annonce;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services.Notifications;
using API.Services.Notifications.Events;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnonceController : ControllerBase
{
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceManager;
    private readonly IFavorisRepository  _favorisRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public AnnonceController(IAnnonceRepository<Annonce, int, FilterDTO> manager,IFavorisRepository favorisManager,  IMapper mapper, INotificationService notificationService)
    {
        _annonceManager = manager;
        _favorisRepository = favorisManager;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    private async Task<IEnumerable<AnnonceDTO>> LikeAnnonce(IEnumerable<AnnonceDTO> annoncesDTO)
    {
        int? userId = GetConnectedUserId();
        if (userId == null)
        {
            foreach (AnnonceDTO annonce in annoncesDTO)
            {
                if (await _favorisRepository.CheckIfLiked((int)userId, annonce.AnnonceId))
                {
                    annonce.IsLikedByCurrentUser = true;
                }
            }
        }
        return annoncesDTO;
    }

    private int? GetConnectedUserId()
    {
        if (User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int id))
            {
                return id;
            }
        }
        return null;
    }
    
    
    [AllowAnonymous]
    [HttpGet("GetActiveAnnonces")]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetActiveAnnonces()
    {
        IEnumerable<Annonce> annonces = await _annonceManager.GetActiveAnnonces();
        var annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        annoncesDTO = await LikeAnnonce(annoncesDTO);
        return Ok(annoncesDTO);
    }

    [HttpGet("ByUtilisateurId/{utilisateurId}")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetAllByUtilisateurId(int utilisateurId)
    {
        IEnumerable<Annonce> annonces = await _annonceManager.GetByUtilisateurId(utilisateurId);
        IEnumerable<AnnonceDTO> annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        annoncesDTO = await LikeAnnonce(annoncesDTO);
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
        int? userId = GetConnectedUserId();
        if (userId == null)
        {
            if (await _favorisRepository.CheckIfLiked((int)userId, id))
            {
                annonceDTO.IsLikedByCurrentUser = true;
            }
        }
        return Ok(annonceDTO);
    }
    [Authorize]
    [HttpGet("ByFavorisUtilisateur")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetByFavorisUtilisateur()
    {
        int ? userId = GetConnectedUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        IEnumerable<Annonce> annonces = await _annonceManager.GetByUtilisateurFavoris((int)userId);
        IEnumerable<AnnonceDTO> annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        annoncesDTO = await LikeAnnonce(annoncesDTO);
        return Ok(annoncesDTO);
    }

    [Authorize]
    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutAnnonce(int id, [FromBody] AnnonceDetailDTO annonceDTO)
    {
        if (id != annonceDTO.AnnonceId)
        {
            return BadRequest();
        }
        int? userId = GetConnectedUserId();
        if (userId == null || userId != annonceDTO.UtilisateurId)
        {
            return Unauthorized();
        }
        Annonce annonceToUpdate = await _annonceManager.GetByIdAsync(id);
        if (annonceToUpdate == null)
        {
            return NotFound();
        }
        Annonce annonce = _mapper.Map<Annonce>(annonceDTO);
        await _annonceManager.UpdateAsync(annonceToUpdate, annonce);
        var notificationEvent = new ModificationAnnonceEvent()
        {
            AnnonceId = annonce.AnnonceId,
            CreatorId = annonce.UtilisateurId,
            
        };
        await _notificationService.NotifyAsync(notificationEvent);
        return NoContent();
    }
    
    [Authorize]
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
        int? userId = GetConnectedUserId();
        if (userId == null || userId != annonceDto.UtilisateurId)
        {
            return Unauthorized();
        }

        var annonce =  _mapper.Map<Annonce>(annonceDto);
        await _annonceManager.AddAsync(annonce);
        AnnonceDetailDTO resultDto = _mapper.Map<AnnonceDetailDTO>(annonce);
        var notificationEvent = new NewAnnonceEvent
        {
            AnnonceId = annonce.AnnonceId,
            CreatorId = annonce.UtilisateurId,
            
        };
        await _notificationService.NotifyAsync(notificationEvent);
        return CreatedAtAction( nameof(GetById), new { id = annonce.AnnonceId }, resultDto);
    }

    [HttpGet("MostRecent")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetMostRecent()
    {
        var annonces = await _annonceManager.GetMostRecentAsync();
        var annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        annoncesDTO = await LikeAnnonce(annoncesDTO);
        return Ok(annoncesDTO);
    }

    [HttpGet("PlusLike")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetMostLiked()
    {
        var annonces = await _annonceManager.GetPlusLikeAsync();
        var annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        annoncesDTO = await LikeAnnonce(annoncesDTO);
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
    
    [HttpGet("productByFilter")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetAllAnnonceByFilter(
        [FromQuery] FilterDTO filterDto)
    {
        var annonces = (await _annonceManager.FilterAsync(filterDto));
        var annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        annoncesDTO = await LikeAnnonce(annoncesDTO);
        return new ActionResult<IEnumerable<AnnonceDTO>>(annoncesDTO);
    }
}