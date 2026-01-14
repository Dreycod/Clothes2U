using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using API.Models.Repository.Managers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Categorie;
using Shared.DTO.Couleur;
using Shared.DTO.Couleur;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class CouleurController : ControllerBase
{
    private readonly  ICaracteristiquesRepository<Couleur> _couleurManager;
    private readonly IMapper _mapper;

    public CouleurController( ICaracteristiquesRepository<Couleur> couleurManager, IMapper mapper)
    {
        _couleurManager = couleurManager;
        _mapper = mapper;
    }
    /// <summary>
    /// Récupère la liste complète de toutes les couleurs avec leurs détails.
    /// </summary>
    /// <returns>La liste de toutes les couleurs disponibles.</returns>
    /// <response code="200">Retourne la liste complète des couleurs.</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CouleurDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CouleurDTO>>> GetAllCouleurs()
    {
        IEnumerable<Couleur> couleurs =  await _couleurManager.GetAllWithDetailsAsync();
        IEnumerable<CouleurDTO> couleursDTO = _mapper.Map<IEnumerable<CouleurDTO>>(couleurs);
        return Ok(couleursDTO);
    }
    /// <summary>
    /// Crée une nouvelle couleur.
    /// Nécessite le rôle Admin ou Commercial.
    /// </summary>
    /// <param name="Couleur">Les données de la couleur à créer.</param>
    /// <returns>La couleur créée.</returns>
    /// <response code="201">La couleur a été créée avec succès.</response>
    /// <response code="400">Les données fournies sont invalides.</response>
    /// <response code="401">L'utilisateur n'est pas authentifié.</response>
    /// <response code="403">L'utilisateur n'a pas les droits nécessaires (Admin ou Commercial requis).</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Commercial")]
    [ProducesResponseType(typeof(Couleur), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Couleur>> AddCouleur([FromBody] CouleurDTO Couleur)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Couleur _Couleur = _mapper.Map<Couleur>(Couleur);

        await _couleurManager.AddAsync(_Couleur);
        return CreatedAtAction(nameof(GetById), new { id = _Couleur.CouleurId }, _Couleur);
    }
    /// <summary>
    /// Supprime une couleur existante.
    /// Nécessite le rôle Admin ou Commercial.
    /// </summary>
    /// <param name="id">L'identifiant de la couleur à supprimer.</param>
    /// <returns>Aucun contenu en cas de succès.</returns>
    /// <response code="204">La couleur a été supprimée avec succès.</response>
    /// <response code="404">La couleur n'existe pas.</response>
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
    public async Task<IActionResult> DeleteCouleur(int id)
    {
        Couleur? CouleurToDelete = await _couleurManager.GetByIdAsync(id);
        if (CouleurToDelete == null)
        {
            return NotFound();
        }
        await _couleurManager.DeleteAsync(CouleurToDelete);
        return NoContent();
    }
    /// <summary>
    /// Met à jour une couleur existante.
    /// Nécessite le rôle Admin ou Modérateur.
    /// </summary>
    /// <param name="id">L'identifiant de la couleur à modifier.</param>
    /// <param name="couleur">Les nouvelles données de la couleur.</param>
    /// <returns>Aucun contenu en cas de succès.</returns>
    /// <response code="204">La couleur a été mise à jour avec succès.</response>
    /// <response code="400">L'identifiant de la couleur ne correspond pas.</response>
    /// <response code="404">La couleur n'existe pas.</response>
    /// <response code="401">L'utilisateur n'est pas authentifié.</response>
    /// <response code="403">L'utilisateur n'a pas les droits nécessaires (Admin ou Modérateur requis).</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpPut("id/{id}")]
    [Authorize(Roles = "Admin,Moderateur")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutCouleur(int id, [FromBody] CouleurDTO couleur)
    {
        if (id != couleur.CouleurId)
        {
            return BadRequest();
        }
        ActionResult<Couleur?> couleurToUpdate = await _couleurManager.GetByIdAsync(id);

        if (couleurToUpdate.Value == null)
        {
            return NotFound();
        }
        Couleur updatedcouleur = _mapper.Map<Couleur>(couleur);

        await _couleurManager.UpdateAsync(updatedcouleur);
        return NoContent();
    }

    /// <summary>
    /// Récupère une couleur spécifique par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant de la couleur.</param>
    /// <returns>La couleur correspondante.</returns>
    /// <response code="200">Retourne la couleur trouvée.</response>
    /// <response code="404">La couleur n'existe pas.</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(Couleur), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Couleur>> GetById(int id)
    {
        var Couleur = await _couleurManager.GetByIdAsync(id);
        if (Couleur == null)
            return NotFound();
        return Ok(Couleur);
    }
}