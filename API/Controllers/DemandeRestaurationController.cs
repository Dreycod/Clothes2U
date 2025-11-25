using API.DTO.DemandeRestauration;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemandeRestaurationController : ControllerBase
    {
        private readonly IDemandeRestaurationRepository<DemandeRestauration, int> _demandeManager;
        private readonly IMapper _mapper;

        public DemandeRestaurationController(IDemandeRestaurationRepository<DemandeRestauration, int> manager, IMapper mapper)
        {
            _demandeManager = manager;
            _mapper = mapper;
        }

        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DemandeRestaurationDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DemandeRestaurationDTO>>> GetAll()
        {
            IEnumerable<DemandeRestauration> demandes = await _demandeManager.GetAllAsync();
            IEnumerable<DemandeRestaurationDTO> demandesDTO = _mapper.Map<IEnumerable<DemandeRestaurationDTO>>(demandes);
            return Ok(demandesDTO);
        }

        
        [HttpGet("id/{id}")]
        [ProducesResponseType(typeof(DemandeRestaurationDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DemandeRestaurationDetailDTO>> GetById(int id)
        {
            var demande = await _demandeManager.GetByIdAsync(id);
            if (demande == null)
                return NotFound();

            DemandeRestaurationDetailDTO dto = _mapper.Map<DemandeRestaurationDetailDTO>(demande);
            return Ok(dto);
        }

        
        [HttpGet("ByUtilisateurId/{utilisateurId}")]
        [ProducesResponseType(typeof(IEnumerable<DemandeRestaurationDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DemandeRestaurationDTO>>> GetByUtilisateurId(int utilisateurId)
        {
            IEnumerable<DemandeRestauration> demandes = await _demandeManager.GetByUtilisateurId(utilisateurId);
            IEnumerable<DemandeRestaurationDTO> demandesDTO = _mapper.Map<IEnumerable<DemandeRestaurationDTO>>(demandes);
            return Ok(demandesDTO);
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(DemandeRestaurationDetailDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DemandeRestaurationDetailDTO>> Create(DemandeRestaurationCreateDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var demande = _mapper.Map<DemandeRestauration>(request);
            await _demandeManager.AddAsync(demande);

            var result = _mapper.Map<DemandeRestaurationDetailDTO>(demande);

            return CreatedAtAction(nameof(GetById), new { id = demande.DemandeRestaurationId }, result);
        }

        
        [HttpDelete("id/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            DemandeRestauration? demande = await _demandeManager.GetByIdAsync(id);

            if (demande == null)
                return NotFound();

            await _demandeManager.DeleteAsync(demande);
            return NoContent();
        }
    }
}
