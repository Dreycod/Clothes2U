using Shared.DTO;
using Shared.DTO.Favoris;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Favoris = API.Models.EntityFramework.Favoris;

namespace API.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class FavorisController :  ControllerBase
{
    private readonly IFavorisRepository _favorisManager;
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceManager;
    private readonly ISuggestionService _suggestionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    
    public FavorisController(IFavorisRepository manager,IAnnonceRepository<Annonce, int, FilterDTO> annonceManager, IMapper mapper,ISuggestionService suggestionService, ICurrentUserService currentUserService)
    {
        _favorisManager = manager;
        _annonceManager = annonceManager;
        _currentUserService = currentUserService;
        _suggestionService =  suggestionService;
        _mapper = mapper;
    }
    /// <summary>
    /// Ajoute une annonce aux favoris de l'utilisateur connecté.
    /// </summary>
    /// <param name="annonceId">L'identifiant de l'annonce à ajouter aux favoris.</param>
    /// <returns>Le favori créé.</returns>
    /// <remarks>
    /// Cette action déclenche également le recalcul des suggestions personnalisées pour l'utilisateur.
    /// </remarks>
    /// <response code="201">Annonce ajoutée aux favoris avec succès.</response>
    /// <response code="400">Requête invalide.</response>
    /// <response code="401">Non autorisé - authentification requise.</response>
    /// <response code="404">L'annonce spécifiée n'existe pas.</response>
    /// <response code="409">Conflit - l'annonce est déjà dans les favoris.</response>
    /// <response code="500">Erreur serveur interne.</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(FavorisDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FavorisDTO>> AddFavoris([FromBody] int annonceId)
    {
        Annonce annonce =  await _annonceManager.GetByIdAsync(annonceId);
        if (annonce == null)
        {
            return NotFound("L'annonce n'existe pas");
        }

        int userId = await _currentUserService.GetUserIdOrThrow();
        bool alreadyLiked = await _favorisManager.CheckIfLiked(userId, annonceId);
        if (alreadyLiked)
        {
            return Conflict("Vous avez déjà ajouté cette annonce à vos favoris");
        }
        Favoris favoris = new Favoris
        {
            UtilisateurId = userId,
            AnnonceId = annonceId
        };
        await _favorisManager.AddAsync(favoris);
        _suggestionService.CalculSuggestion(userId);
        FavorisDTO favorisDto = _mapper.Map<FavorisDTO>(favoris);
    
        return StatusCode(StatusCodes.Status201Created, favorisDto);

    }
    /// <summary>
    /// Retire une annonce des favoris de l'utilisateur connecté.
    /// </summary>
    /// <param name="annonceId">L'identifiant de l'annonce à retirer des favoris.</param>
    /// <returns>Aucun contenu en cas de succès.</returns>
    /// <remarks>
    /// Cette action déclenche également le recalcul des suggestions personnalisées pour l'utilisateur.
    /// </remarks>
    /// <response code="204">Annonce retirée des favoris avec succès.</response>
    /// <response code="401">Non autorisé - authentification requise.</response>
    /// <response code="404">Favori introuvable - l'annonce n'est pas dans les favoris de l'utilisateur.</response>
    /// <response code="500">Erreur serveur interne.</response>
    [HttpDelete("id/{annonceId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteFavoris(int annonceId)
    {
       
        int userId = await _currentUserService.GetUserIdOrThrow();
        Favoris? favorisToDelete = await _favorisManager.GetFavorisByAnnonceAndUserId((int)userId, annonceId);
        if (favorisToDelete == null)
        {
            return NotFound();
        }
        await _favorisManager.DeleteAsync(favorisToDelete);
        _suggestionService.CalculSuggestion(userId);
        return NoContent();
    }
}