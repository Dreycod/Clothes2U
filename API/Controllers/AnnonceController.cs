using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using API.Models.Repository.Managers;
using API.Services;
using API.Services.VerificationSrvceV2;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.DTO.Annonce;
using Shared.DTO.Couleur;
using Shared.DTO.Notification;
using Shared.DTO.Photo;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnonceController : ControllerBase
{
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceManager;
    private readonly IEstDeCouleurRepository<Est_De_Couleur, int> _estDeCouleurRepository;
    private readonly IllustreAnnonceRepository<Illustre_Annonce, int> _illustreAnnonceManager;
    private readonly IAnnonceExtensionService _annonceExtensionService;
    private readonly INotificationService _notificationService;
    private readonly ISuggestionService _suggestionService;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMotInterditService _motInterditService;
    private readonly IPhotoService _photoService;
    private readonly ITagRepository<Tag, int> _tagManager;


    public AnnonceController(
        IAnnonceRepository<Annonce, int, FilterDTO> manager,
        IEstDeCouleurRepository<Est_De_Couleur, int> estDeCouleurRepo,
        IAnnonceExtensionService annonceExtensionService,
        IMapper mapper,
        IllustreAnnonceRepository<Illustre_Annonce, int> illustreAnnonceManager,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        IPhotoService photoService,
        ISuggestionService suggestionService,
        IMotInterditService motInterditService,
        ITagRepository<Tag, int> tagManager
        )
    {
        _annonceManager = manager;
        _estDeCouleurRepository = estDeCouleurRepo;
        _mapper = mapper;
        _annonceExtensionService = annonceExtensionService;
        _notificationService = notificationService;
        _currentUserService = currentUserService;
        _suggestionService = suggestionService;
        _motInterditService = motInterditService;
        _illustreAnnonceManager = illustreAnnonceManager;
        _photoService = photoService;
        _tagManager = tagManager;
    }
    [HttpGet("ByUtilisateurId/{utilisateurId}")]
    [ProducesResponseType(typeof(IEnumerable<AnnonceDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<AnnonceDTO>>> GetAllByUtilisateurId(int utilisateurId)
    {
        IEnumerable<AnnonceDTO> annoncesDTO = await _annonceExtensionService.GetAnnoncesByUserId(utilisateurId);
        return Ok(annoncesDTO);
    }

    [AllowAnonymous]
    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(AnnonceDetailDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AnnonceDetailDTO>> GetById(int id)
    {
        var annonce = await _annonceManager.GetByIdAsync(id);
        if (annonce == null)
            return NotFound();

        AnnonceDetailDTO annonceDTO = _mapper.Map<AnnonceDetailDTO>(annonce);
        annonceDTO = await _annonceExtensionService.LikeAnnonceDetail(annonceDTO);
        annonceDTO = await _annonceExtensionService.CheckOwnerAnnonceDetail(annonceDTO);
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
        await _photoService.DeletePhotosAnnonceAsync(annonce.AnnonceId); // éviter dupliqués
        await _tagManager.DeleteTagsAnnonceAsync(annonce.AnnonceId); // éviter dupliqués
        await _estDeCouleurRepository.DeleteCouleurAnnonce(annonce.AnnonceId); // éviter dupliqués
        await _annonceManager.UpdateAsync(annonce);

        var UpdatedAnnonce = _annonceManager.GetByIdAsync(id);
        
        if (UpdatedAnnonce == null)
        {
            return NotFound();
        }

        AnnonceDTO updatedAnnonceDTO = _mapper.Map<AnnonceDTO>(UpdatedAnnonce.Result);

        await _notificationService.CreateModificationAnnonceNotification(annonce.AnnonceId);

        return Ok(updatedAnnonceDTO);
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
        var isForbidden = await _motInterditService.ContientMotInterdit(createAnnonceDto.Titre) ||
                                 await _motInterditService.ContientMotInterdit(createAnnonceDto.Description);
        if (isForbidden)
        {
            return BadRequest("Mot Interdit");
        }
        int userId = await _currentUserService.GetUserIdOrThrow();
        var annonce = _mapper.Map<Annonce>(createAnnonceDto);

        await _annonceManager.AddAsync(annonce);

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
        annoncesDTO = await _annonceExtensionService.CheckOwnerAnnonce(annoncesDTO);

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
        annoncesDTO = await _annonceExtensionService.CheckOwnerAnnonce(annoncesDTO);

        return Ok(annoncesDTO);
    }

    [AllowAnonymous]
    [HttpGet("GetAnnoncesByPhotoIDs")]
    [ProducesResponseType(typeof(AnnonceDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AnnonceDTO>> GetAnnoncesByPhotoIDs(
    [FromQuery] IEnumerable<int> PhotoIDs)
    {
        // Valider PhotoIDs
        if (PhotoIDs.Any(p => p <= 0))
        {
            return BadRequest("Tous les PhotoID doivent être supérieurs à 0");
        }

        // Fetch tous Illustre_Annonce
        var illustreAnnonces = await _illustreAnnonceManager.GetByPhotoIds(PhotoIDs);

        if (illustreAnnonces == null || !illustreAnnonces.Any())
        {
            return BadRequest("Aucune Illustre_Annonce trouvée pour les PhotoIDs fournis.");
        }

        var annonceIds = illustreAnnonces.Select(i => i.AnnonceId).Distinct();
        var annonces = await _annonceManager.GetByIdsAsync(annonceIds); // assuming you have batch fetch
        if (annonces == null || !annonces.Any())
        {
            return BadRequest("Aucune Annonce trouvée pour les Illustre_Annonce fournies.");
        }

        // Map AnnonceId -> Annonce
        var annoncesById = annonces.ToDictionary(a => a.AnnonceId);

        // Map -> DTOs
        var annonceDTOs = illustreAnnonces
            .Where(i => annoncesById.ContainsKey(i.AnnonceId))
            .Select(i =>
            {
                var annonce = annoncesById[i.AnnonceId];
                return _mapper.Map<AnnonceDTO>(annonce);
            })
            .ToList();

        return Ok(annonceDTOs);
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
        return Ok(annoncesDTO);
    }

    [HttpPut("Vendu/{annonceId}")]
    public async Task<IActionResult> PutAnnonce(int annonceId)
    {
        var annonceToSell = _annonceManager.GetByIdAsync(annonceId);
        annonceToSell.Result.StatutAnnonceId = 4;
        await _annonceManager.UpdateAsync(annonceToSell.Result);
        return NoContent();
    }

    [HttpGet("ByUtilisateurIdPagination/{id}")]
    public async Task<ActionResult<AnnonceDetailDTO>> GetAnnoncesPaginationByUserId(int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest("Page et pageSize doivent être supérieurs à 0");
        }
        IEnumerable<AnnonceDTO> annonces = await _annonceExtensionService.GetAnnoncesByUserId(id);

        IEnumerable<AnnonceDTO> annoncesDTO = _mapper.Map<IEnumerable<AnnonceDTO>>(annonces);
        annoncesDTO = await _annonceExtensionService.LikeAnnonces(annoncesDTO);
        annoncesDTO = await _annonceExtensionService.CheckOwnerAnnonce(annoncesDTO);
        return Ok(annoncesDTO);
    }

    [HttpPatch("PauseAnnonce/{id}")]
    public async Task<IActionResult> PauserAnnonce(int id)
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        if (userId == null)
            return Unauthorized("Utilisateur non connecté.");

        Annonce annonceToPause = await _annonceManager.GetByIdAsync(id);
        if (annonceToPause == null)
            return NotFound("Annonce non trouvée.");

        if (annonceToPause.UtilisateurId != userId)
            return Forbid("Vous n'êtes pas le propriétaire de cette annonce.");

        annonceToPause.StatutAnnonceId = 5;
        await _annonceManager.UpdateAsync(annonceToPause);
        return NoContent();
    }

    [HttpPatch("ReprendreAnnonce/{id}")]
    public async Task<IActionResult> ReprendreAnnonce(int id)
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        if (userId == null)
            return Unauthorized("Utilisateur non connecté.");

        Annonce annonceToPause = await _annonceManager.GetByIdAsync(id);
        if (annonceToPause == null)
            return NotFound("Annonce non trouvée.");

        if (annonceToPause.UtilisateurId != userId)
            return Forbid("Vous n'êtes pas le propriétaire de cette annonce.");

        annonceToPause.StatutAnnonceId = 1;
        await _annonceManager.UpdateAsync(annonceToPause);
        return NoContent();
    }
}