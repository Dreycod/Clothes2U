using API.DTO;
using API.DTO.Favoris;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
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
    private readonly IMapper _mapper;
    
    public FavorisController(IFavorisRepository manager,IAnnonceRepository<Annonce, int, FilterDTO> annonceManager, IMapper mapper)
    {
        _favorisManager = manager;
        _annonceManager = annonceManager;
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
        Annonce annonce = await _annonceManager.GetByIdAsync(annonceId);
        if (annonce == null)
        {
            return NotFound("L'annonce n'existe pas");
        }
        if (User?.Identity?.IsAuthenticated != true)
        {
            return Unauthorized("Vous devez être connecté pour ajouter un favori");
        }
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("ID utilisateur invalide");
        }
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
        FavorisDTO favorisDto = _mapper.Map<FavorisDTO>(favoris);
    
        return CreatedAtAction(nameof(GetById), new { id = favoris.FavorisId }, favorisDto);
    }
    [HttpDelete("id/{annonceId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduit(int annonceId)
    {
       
        if (User?.Identity?.IsAuthenticated != true)
        {
            return Unauthorized("Vous devez être connecté pour ajouter un favori");
        }
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("ID utilisateur invalide");
        }
        
        Favoris? favorisToDelete = await _favorisManager.GetFavorisByAnnonceAndUserId(userId, annonceId);
        if (favorisToDelete == null)
        {
            return NotFound();
        }
        await _favorisManager.DeleteAsync(favorisToDelete);
        return NoContent();
    }
}