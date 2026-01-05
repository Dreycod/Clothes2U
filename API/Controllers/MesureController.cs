using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Mesures;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MesureController : ControllerBase
{
    private readonly ICaracteristiquesRepository<Mesure> _mesureRepository;
    private readonly IMapper _mapper;

    public MesureController(ICaracteristiquesRepository<Mesure> mesureRepository, IMapper mapper)
    {
        _mesureRepository = mesureRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// GET: api/Mesure
    /// Récupère toutes les mesures (relation taille-catégorie)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MesureDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MesureDTO>>> GetAllMesures()
    {
        try
        {
            IEnumerable<Mesure> mesures = await _mesureRepository.GetAllAsync();
            IEnumerable<MesureDTO> mesuresDTO = _mapper.Map<IEnumerable<MesureDTO>>(mesures);
            return Ok(mesuresDTO);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur GET /api/Mesure: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la récupération des mesures");
        }
    }

    /// <summary>
    /// GET: api/Mesure/byCategorie/{categorieId}
    /// Récupère les IDs de tailles disponibles pour une catégorie donnée
    /// </summary>
    [HttpGet("byCategorie/{categorieId}")]
    [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<int>>> GetTaillesByCategorie(int categorieId)
    {
        try
        {
            IEnumerable<Mesure> mesures = await _mesureRepository.GetAllAsync();

            var tailleIds = mesures
                .Where(m => m.CategorieId == categorieId)
                .Select(m => m.TailleId)
                .Distinct()
                .ToList();

            if (!tailleIds.Any())
            {
                return NotFound($"Aucune taille trouvée pour la catégorie {categorieId}");
            }

            return Ok(tailleIds);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur GET /api/Mesure/byCategorie/{categorieId}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la récupération des tailles");
        }
    }

    /// <summary>
    /// GET: api/Mesure/id/{id}
    /// Récupère une mesure par son ID
    /// </summary>
    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(MesureDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MesureDTO>> GetById(int id)
    {
        try
        {
            var mesure = await _mesureRepository.GetByIdAsync(id);

            if (mesure == null)
                return NotFound($"Mesure avec l'ID {id} introuvable");

            var mesureDTO = _mapper.Map<MesureDTO>(mesure);
            return Ok(mesureDTO);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur GET /api/Mesure/id/{id}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// POST: api/Mesure
    /// Crée une nouvelle mesure
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MesureDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MesureDTO>> AddMesure([FromBody] MesureDTO mesureDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mesure mesure = _mapper.Map<Mesure>(mesureDto);
            await _mesureRepository.AddAsync(mesure);

            var createdMesureDto = _mapper.Map<MesureDTO>(mesure);
            return CreatedAtAction(nameof(GetById), new { id = mesure.MesureId }, createdMesureDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur POST /api/Mesure: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// PUT: api/Mesure/id/{id}
    /// Met à jour une mesure existante
    /// </summary>
    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutMesure(int id, [FromBody] MesureDTO mesureDto)
    {
        try
        {
            if (id != mesureDto.MesureId)
            {
                return BadRequest("L'ID dans l'URL ne correspond pas à l'ID de la mesure");
            }

            var mesureToUpdate = await _mesureRepository.GetByIdAsync(id);

            if (mesureToUpdate == null)
            {
                return NotFound($"Mesure avec l'ID {id} introuvable");
            }

            Mesure updatedMesure = _mapper.Map<Mesure>(mesureDto);
            await _mesureRepository.UpdateAsync(updatedMesure);

            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur PUT /api/Mesure/id/{id}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// DELETE: api/Mesure/id/{id}
    /// Supprime une mesure
    /// </summary>
    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMesure(int id)
    {
        try
        {
            Mesure? mesureToDelete = await _mesureRepository.GetByIdAsync(id);

            if (mesureToDelete == null)
            {
                return NotFound($"Mesure avec l'ID {id} introuvable");
            }

            await _mesureRepository.DeleteAsync(mesureToDelete);
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur DELETE /api/Mesure/id/{id}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}