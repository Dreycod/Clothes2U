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
}