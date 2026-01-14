using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.DTO.Categorie;
 
namespace API.Controllers;



[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class CategorieController : ControllerBase
{
    private readonly ICaracteristiquesRepository<Categorie> _categorieManager;
    private readonly IMapper _mapper;

    public CategorieController(ICaracteristiquesRepository<Categorie> manager, IMapper mapper)
    {
        _categorieManager = manager;
        _mapper = mapper;
    }
    /// <summary>
    /// Récupère la liste complète de toutes les catégories avec leurs propriétés de navigation.
    /// </summary>
    /// <returns>La liste de toutes les catégories disponibles.</returns>
    /// <response code="200">Retourne la liste complète des catégories.</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Categorie>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CategorieDTO>>> GetAllCategorieWithNavigation()
    {
        IEnumerable<Categorie> categories =  await _categorieManager.GetAllAsync();
        IEnumerable<CategorieDTO> categoriesDTO = _mapper.Map<IEnumerable<CategorieDTO>>(categories);
        return Ok(categoriesDTO);
    }
    /// <summary>
    /// Crée une nouvelle catégorie.
    /// Nécessite le rôle Admin ou Commercial.
    /// </summary>
    /// <param name="Categorie">Les données de la catégorie à créer.</param>
    /// <returns>La catégorie créée.</returns>
    /// <response code="201">La catégorie a été créée avec succès.</response>
    /// <response code="400">Les données fournies sont invalides.</response>
    /// <response code="401">L'utilisateur n'est pas authentifié.</response>
    /// <response code="403">L'utilisateur n'a pas les droits nécessaires (Admin ou Commercial requis).</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Commercial")]
    [ProducesResponseType(typeof(Categorie), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Categorie>> AddCategorie([FromBody] CategorieDTO Categorie)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Categorie _Categorie = _mapper.Map<Categorie>(Categorie);

        await _categorieManager.AddAsync(_Categorie);
        return CreatedAtAction(nameof(GetById), new { id = _Categorie.CategorieId }, _Categorie);
    }
    /// <summary>
    /// Supprime une catégorie existante.
    /// Nécessite le rôle Admin ou Commercial.
    /// </summary>
    /// <param name="id">L'identifiant de la catégorie à supprimer.</param>
    /// <returns>Aucun contenu en cas de succès.</returns>
    /// <response code="204">La catégorie a été supprimée avec succès.</response>
    /// <response code="404">La catégorie n'existe pas.</response>
    /// <response code="401">L'utilisateur n'est pas authentifié.</response>
    /// <response code="403">L'utilisateur n'a pas les droits nécessaires (Admin ou Commercial requis).</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpDelete("id/{id}")]
    [Authorize(Roles = "Admin,Commercial")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCategorie(int id)
    {
        Categorie? CategorieToDelete = await _categorieManager.GetByIdAsync(id);
        if (CategorieToDelete == null)
        {
            return NotFound();
        }
        await _categorieManager.DeleteAsync(CategorieToDelete);
        return NoContent();
    }
    
    /// <summary>
    /// Met à jour une catégorie existante.
    /// Nécessite le rôle Admin ou Commercial.
    /// </summary>
    /// <param name="id">L'identifiant de la catégorie à modifier.</param>
    /// <param name="Categorie">Les nouvelles données de la catégorie.</param>
    /// <returns>Aucun contenu en cas de succès.</returns>
    /// <response code="204">La catégorie a été mise à jour avec succès.</response>
    /// <response code="400">L'identifiant de la catégorie ne correspond pas.</response>
    /// <response code="404">La catégorie n'existe pas.</response>
    /// <response code="401">L'utilisateur n'est pas authentifié.</response>
    /// <response code="403">L'utilisateur n'a pas les droits nécessaires (Admin ou Commercial requis).</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpPut("id/{id}")]
    [Authorize(Roles = "Admin,Commercial")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutCategorie(int id, [FromBody] CategorieDTO Categorie)
    {
        if (id != Categorie.IdCategorie)
        {
            return BadRequest();
        }
        ActionResult<Categorie?> CategorieToUpdate = await _categorieManager.GetByIdAsync(id);

        if (CategorieToUpdate.Value == null)
        {
            return NotFound();
        }
        Categorie updatedCategorie = _mapper.Map<Categorie>(Categorie);

        await _categorieManager.UpdateAsync(updatedCategorie);
        return NoContent();
    }
    /// <summary>
    /// Récupère une catégorie spécifique par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant de la catégorie.</param>
    /// <returns>La catégorie correspondante.</returns>
    /// <response code="200">Retourne la catégorie trouvée.</response>
    /// <response code="404">La catégorie n'existe pas.</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(Categorie), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Categorie>> GetById(int id)
    {
        var Categorie = await _categorieManager.GetByIdAsync(id);
        if (Categorie == null)
            return NotFound();
        return Ok(Categorie);
    }
}