using Shared.DTO;
using Shared.DTO.Annonce;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services;
using API.Services.Notifications;
using API.Services.Notifications.Events;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Services.VerificationSrvceV2;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnonceController : ControllerBase
{
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceManager;
    private readonly IFavorisRepository  _favorisRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationMailService _notificationMailService;

    public AnnonceController(IAnnonceRepository<Annonce, int, FilterDTO> manager,IFavorisRepository favorisManager,  IMapper mapper, INotificationService notificationService, ICurrentUserService currentUserService, INotificationMailService notificationMailService)
    {
        _annonceManager = manager;
        _favorisRepository = favorisManager;
        _mapper = mapper;
        _notificationService = notificationService;
        _currentUserService = currentUserService;
        _notificationMailService = notificationMailService;
    }

    private async Task<IEnumerable<AnnonceDTO>> LikeAnnonce(IEnumerable<AnnonceDTO> annoncesDTO)
    {
        int? userId = await _currentUserService.GetUserId();
        if (!userId.HasValue)
            return annoncesDTO;

        int uid = userId.Value;

        foreach (var annonce in annoncesDTO)
        {
            if (await _favorisRepository.CheckIfLiked(uid, annonce.AnnonceId))
            {
                annonce.IsLikedByCurrentUser = true;
            }
        }

        return annoncesDTO;
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
        int? userId = await _currentUserService.GetUserId();
        if (userId != null)
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
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetByFavorisUtilisateur(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest("Page et pageSize doivent être supérieurs à 0");
        }

        int? userId = await _currentUserService.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        IEnumerable<Annonce> annonces = await _annonceManager.GetByUtilisateurFavoris((int)userId);
    
        IEnumerable<Annonce> annoncesPaginees = annonces
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    
        IEnumerable<AnnonceDTO> annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annoncesPaginees);
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
        int? userId = await _currentUserService.GetUserId();
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
        await _annonceManager.UpdateAsync(annonce);
        var notificationEvent = new ModificationAnnonceEvent()
        {
            AnnonceId = annonce.AnnonceId,
            CreatorId = annonce.UtilisateurId,
            
        };
        await _notificationService.NotifyAsync(notificationEvent);
        await _notificationMailService.NotifyAnnonceUpdatedAsync(annonce);
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
        int? userId = await _currentUserService.GetUserId();
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
        await _notificationMailService.NotifyNewAnnonceAsync(annonce);
        return CreatedAtAction( nameof(GetById), new { id = annonce.AnnonceId }, resultDto);
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
    [AllowAnonymous]
    [HttpGet("productByFilter")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetAllAnnonceByFilter(
        [FromQuery] FilterDTO filterDto,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest("Page et pageSize doivent être supérieurs à 0");
        }
        int? userId = await _currentUserService.GetUserId();
        var annonces = await _annonceManager.FilterAsync(filterDto, page, pageSize, userId);
        var annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        annoncesDTO = await LikeAnnonce(annoncesDTO);
    
        return Ok(annoncesDTO);
    }
}