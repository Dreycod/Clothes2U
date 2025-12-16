using API.DTO;
using API.DTO.Favoris;
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
    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(Favoris),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Favoris>> GetById(int id)
    {
        Favoris favoris = await _favorisManager.GetByIdAsync(id);
        if (favoris == null)
            return NotFound();
        return favoris;
    } 
    
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(FavorisDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FavorisDTO>> AddFavoris([FromBody] int annonceId)
    {
        Annonce annonce =  await _annonceManager.GetByIdAsync(annonceId);
        if (annonce == null)
        {
            return NotFound("L'annonce n'existe pas");
        }
        int? userId = await _currentUserService.GetUserId();
        if (userId == null)
        {
            return Unauthorized("Vous devez être connecté pour ajouter un favori");
        }
        bool alreadyLiked = await _favorisManager.CheckIfLiked((int)userId, annonceId);
        if (alreadyLiked)
        {
            return Conflict("Vous avez déjà ajouté cette annonce à vos favoris");
        }
        Favoris favoris = new Favoris
        {
            UtilisateurId = (int)userId,
            AnnonceId = annonceId
        };
        await _favorisManager.AddAsync(favoris);
        _suggestionService.CalculSuggestion((int)userId);
        FavorisDTO favorisDto = _mapper.Map<FavorisDTO>(favoris);
    
        return CreatedAtAction(nameof(GetById), new { id = favoris.FavorisId }, favorisDto);
    }
    [HttpDelete("id/{annonceId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteFavoris(int annonceId)
    {
       
       int? userId = await _currentUserService.GetUserId();
       if (userId == null)
       {
           return Unauthorized();
       }
        
        Favoris? favorisToDelete = await _favorisManager.GetFavorisByAnnonceAndUserId((int)userId, annonceId);
        if (favorisToDelete == null)
        {
            return NotFound();
        }
        await _favorisManager.DeleteAsync(favorisToDelete);
        return NoContent();
    }
}