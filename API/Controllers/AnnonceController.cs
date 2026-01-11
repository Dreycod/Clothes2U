using Shared.DTO;
using Shared.DTO.Annonce;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Services.VerificationSrvceV2;
using Shared.DTO.Couleur;
using Shared.DTO.Notification;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnonceController : ControllerBase
{
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceManager;
    private readonly ICaracteristiquesRepository<Est_De_Couleur> _estDeCouleurRepository;
    private readonly IAnnonceExtensionService _annonceExtensionService;
    private readonly INotificationService _notificationService;
    private readonly ISuggestionService _suggestionService;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public AnnonceController(
        IAnnonceRepository<Annonce, int, FilterDTO> manager,
        ICaracteristiquesRepository<Est_De_Couleur> estDeCouleurRepo,
        IAnnonceExtensionService annonceExtensionService,
        IMapper mapper,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ISuggestionService suggestionService
        )
    {
        _annonceManager = manager;
        _estDeCouleurRepository = estDeCouleurRepo;
        _mapper = mapper;
        _annonceExtensionService = annonceExtensionService;
        _notificationService = notificationService;
        _currentUserService = currentUserService;
        _suggestionService = suggestionService;
    }

    [HttpGet("ByUtilisateurId/{utilisateurId}")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetAllByUtilisateurId(int utilisateurId)
    {
        IEnumerable<Annonce> annonces = await _annonceManager.GetByUtilisateurId(utilisateurId);
        IEnumerable<AnnonceDTO> annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        annoncesDTO = await _annonceExtensionService.LikeAnnonces(annoncesDTO);
        return Ok(annoncesDTO);
    }
    [AllowAnonymous]
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
        annonceDTO = await _annonceExtensionService.LikeAnnonceDetail(annonceDTO);
        await _notificationService.DeleteAnnonceNotificationForUser(id);
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

        int userId = await _currentUserService.GetUserIdOrThrow();
        IEnumerable<Annonce> annonces = await _annonceManager.GetByUtilisateurFavoris((int)userId);
    
        IEnumerable<Annonce> annoncesPaginees = annonces
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    
        IEnumerable<AnnonceDTO> annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annoncesPaginees);
        annoncesDTO = await _annonceExtensionService.LikeAnnonces(annoncesDTO);

        return Ok(annoncesDTO);
    }

    [Authorize]
    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutAnnonce(int id, [FromBody] PutAnnonceDTO annonceDTO)
    {
        if (id != annonceDTO.AnnonceId)
        {
            return BadRequest();
        }
        int userId = await _currentUserService.GetUserIdOrThrow();
        Annonce annonceToUpdate = await _annonceManager.GetByIdAsync(id);
        if (annonceToUpdate == null)
        {
            return NotFound();
        }
        Annonce annonce = _mapper.Map<Annonce>(annonceDTO);
        await _annonceManager.UpdateAsync(annonce);
        await _notificationService.CreateModificationAnnonceNotification(annonce.AnnonceId);
        return NoContent();
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(AnnonceDetailDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AnnonceDetailDTO>> AddAnnonce(CreateAnnonceDTO createAnnonceDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int userId = await _currentUserService.GetUserIdOrThrow();
        var annonce = _mapper.Map<Annonce>(createAnnonceDto);

        await _annonceManager.AddAsync(annonce);

        if (createAnnonceDto.Couleurs?.Any() == true)
        {
            foreach (var couleurId in createAnnonceDto.Couleurs)
            {
                var estDeCouleur = new Est_De_Couleur
                {
                    AnnonceId = annonce.AnnonceId,
                    CouleurId = couleurId
                };

                await _estDeCouleurRepository.AddAsync(estDeCouleur);
            }
        }
        var annonceComplete = await _annonceManager.GetByIdAsync(annonce.AnnonceId);
        AnnonceDetailDTO resultDto = _mapper.Map<AnnonceDetailDTO>(annonceComplete);

        // Notification
        await _notificationService.CreateNouvelleAnnonceNotification(annonce.AnnonceId);
    
        return CreatedAtAction(nameof(GetById), new { id = annonce.AnnonceId }, resultDto);
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
        annoncesDTO = await _annonceExtensionService.LikeAnnonces(annoncesDTO);
    
        return Ok(annoncesDTO);
    }
    [AllowAnonymous]
    [HttpGet("similarAnnonces")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetSimilarAnnonces(
        [FromQuery] int annonceId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest("Page et pageSize doivent être supérieurs à 0");
        }
        int? userId = await _currentUserService.GetUserId();
        var annonces = await _annonceManager.GetSimilarAsync(annonceId, page, pageSize, userId);
        var annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        annoncesDTO = await _annonceExtensionService.LikeAnnonces(annoncesDTO);
    
        return Ok(annoncesDTO);
    }

    [Authorize]
    [HttpGet("Recommandations")]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetRecommandations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30
    )
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest("Page et pageSize doivent être supérieurs à 0");
        }
        IEnumerable<AnnonceDTO> annonces = await _suggestionService.GetRecommandations(page, pageSize);
        var annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        return  Ok(annoncesDTO);
    }

    [HttpPut("Vendu/{annonceId}")]
    public async Task<IActionResult> PutAnnonce(int annonceId)
    {
        var annonceToSell = _annonceManager.GetByIdAsync(annonceId);
        annonceToSell.Result.StatutAnnonceId = 4;
        await _annonceManager.UpdateAsync(annonceToSell.Result);
        return NoContent();
    }
    
}