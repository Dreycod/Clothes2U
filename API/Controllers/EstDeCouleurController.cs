using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Couleur;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EstDeCouleurController : ControllerBase
{
    private readonly ICaracteristiquesRepository<Est_De_Couleur> _estDeCouleurRepository;
    private readonly IMapper _mapper;

    public EstDeCouleurController(ICaracteristiquesRepository<Est_De_Couleur> estDeCouleurRepository, IMapper mapper)
    {
        _estDeCouleurRepository = estDeCouleurRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// GET: api/EstDeCouleur
    /// Récupère toutes les relations couleur-annonce
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EstDeCouleurDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<EstDeCouleurDTO>>> GetAllEstDeCouleurs()
    {
        try
        {
            IEnumerable<Est_De_Couleur> estDeCouleurs = await _estDeCouleurRepository.GetAllAsync();
            IEnumerable<EstDeCouleurDTO> estDeCouleursDTO = _mapper.Map<IEnumerable<EstDeCouleurDTO>>(estDeCouleurs);
            return Ok(estDeCouleursDTO);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur GET /api/EstDeCouleur: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la récupération des relations couleur-annonce");
        }
    }

    /// <summary>
    /// GET: api/EstDeCouleur/byAnnonce/{annonceId}
    /// Récupère les IDs de couleurs pour une annonce donnée
    /// </summary>
    [HttpGet("byAnnonce/{annonceId}")]
    [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<int>>> GetCouleursByAnnonce(int annonceId)
    {
        try
        {
            IEnumerable<Est_De_Couleur> estDeCouleurs = await _estDeCouleurRepository.GetAllAsync();

            var couleurIds = estDeCouleurs
                .Where(edc => edc.AnnonceId == annonceId)
                .Select(edc => edc.CouleurId)
                .Distinct()
                .ToList();

            if (!couleurIds.Any())
            {
                return NotFound($"Aucune couleur trouvée pour l'annonce {annonceId}");
            }

            return Ok(couleurIds);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur GET /api/EstDeCouleur/byAnnonce/{annonceId}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la récupération des couleurs");
        }
    }

    /// <summary>
    /// GET: api/EstDeCouleur/byCouleur/{couleurId}
    /// Récupère les IDs d'annonces pour une couleur donnée
    /// </summary>
    [HttpGet("byCouleur/{couleurId}")]
    [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<int>>> GetAnnoncesByCouleur(int couleurId)
    {
        try
        {
            IEnumerable<Est_De_Couleur> estDeCouleurs = await _estDeCouleurRepository.GetAllAsync();

            var annonceIds = estDeCouleurs
                .Where(edc => edc.CouleurId == couleurId)
                .Select(edc => edc.AnnonceId)
                .Distinct()
                .ToList();

            if (!annonceIds.Any())
            {
                return NotFound($"Aucune annonce trouvée pour la couleur {couleurId}");
            }

            return Ok(annonceIds);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur GET /api/EstDeCouleur/byCouleur/{couleurId}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la récupération des annonces");
        }
    }

    /// <summary>
    /// GET: api/EstDeCouleur/id/{id}
    /// Récupère une relation couleur-annonce par son ID
    /// </summary>
    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(EstDeCouleurDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EstDeCouleurDTO>> GetById(int id)
    {
        try
        {
            var estDeCouleur = await _estDeCouleurRepository.GetByIdAsync(id);

            if (estDeCouleur == null)
                return NotFound($"Relation couleur-annonce avec l'ID {id} introuvable");

            var estDeCouleurDTO = _mapper.Map<EstDeCouleurDTO>(estDeCouleur);
            return Ok(estDeCouleurDTO);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur GET /api/EstDeCouleur/id/{id}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// POST: api/EstDeCouleur
    /// Crée une nouvelle relation couleur-annonce
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EstDeCouleurDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EstDeCouleurDTO>> AddEstDeCouleur([FromBody] CreateEstDeCouleurDTO createEstDeCouleurDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Est_De_Couleur estDeCouleur = _mapper.Map<Est_De_Couleur>(createEstDeCouleurDto);
            await _estDeCouleurRepository.AddAsync(estDeCouleur);

            var EstDeCouleurDto = _mapper.Map<EstDeCouleurDTO>(estDeCouleur);
            return CreatedAtAction(nameof(GetById), new { id = estDeCouleur.EstDeCouleurId }, EstDeCouleurDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur POST /api/EstDeCouleur: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// PUT: api/EstDeCouleur/id/{id}
    /// Met à jour une relation couleur-annonce existante
    /// </summary>
    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutEstDeCouleur(int id, [FromBody] EstDeCouleurDTO estDeCouleurDto)
    {
        try
        {
            if (id != estDeCouleurDto.EstDeCouleurId)
            {
                return BadRequest("L'ID dans l'URL ne correspond pas à l'ID de la relation");
            }

            var estDeCouleurToUpdate = await _estDeCouleurRepository.GetByIdAsync(id);

            if (estDeCouleurToUpdate == null)
            {
                return NotFound($"Relation couleur-annonce avec l'ID {id} introuvable");
            }

            Est_De_Couleur updatedEstDeCouleur = _mapper.Map<Est_De_Couleur>(estDeCouleurDto);
            await _estDeCouleurRepository.UpdateAsync(updatedEstDeCouleur);

            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur PUT /api/EstDeCouleur/id/{id}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// DELETE: api/EstDeCouleur/id/{id}
    /// Supprime une relation couleur-annonce
    /// </summary>
    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteEstDeCouleur(int id)
    {
        try
        {
            Est_De_Couleur? estDeCouleurToDelete = await _estDeCouleurRepository.GetByIdAsync(id);

            if (estDeCouleurToDelete == null)
            {
                return NotFound($"Relation couleur-annonce avec l'ID {id} introuvable");
            }

            await _estDeCouleurRepository.DeleteAsync(estDeCouleurToDelete);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur DELETE /api/EstDeCouleur/id/{id}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// DELETE: api/EstDeCouleur/byAnnonce/{annonceId}
    /// Supprime toutes les couleurs d'une annonce
    /// </summary>
    [HttpDelete("byAnnonce/{annonceId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCouleursByAnnonce(int annonceId)
    {
        try
        {
            IEnumerable<Est_De_Couleur> estDeCouleurs = await _estDeCouleurRepository.GetAllAsync();
            var toDelete = estDeCouleurs.Where(edc => edc.AnnonceId == annonceId).ToList();

            if (!toDelete.Any())
            {
                return NotFound($"Aucune couleur trouvée pour l'annonce {annonceId}");
            }

            foreach (var edc in toDelete)
            {
                await _estDeCouleurRepository.DeleteAsync(edc);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur DELETE /api/EstDeCouleur/byAnnonce/{annonceId}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}