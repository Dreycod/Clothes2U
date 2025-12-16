using API.DTO.Decision_suspension;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DecisionSuspensionController : ControllerBase
    {
        private readonly IDecisionSuspensionRepository<Decision_suspension, int> _suspensionManager;
        private readonly IMapper _mapper;

        public DecisionSuspensionController(IDecisionSuspensionRepository<Decision_suspension, int> manager, IMapper mapper)
        {
            _suspensionManager = manager;
            _mapper = mapper;
        }

        //// Toutes les suspensions actives
        //[HttpGet("Active")]
        //[ProducesResponseType(typeof(IEnumerable<DecisionSuspensionDTO>), StatusCodes.Status200OK)]
        //public async Task<ActionResult<IEnumerable<DecisionSuspensionDTO>>> GetActiveSuspensions()
        //{
        //    var suspensions = await _suspensionManager.GetActiveSuspensionsAsync();
        //    var dto = _mapper.Map<IEnumerable<DecisionSuspensionDTO>>(suspensions);
        //    return Ok(dto);
        //}

        // Toutes les suspensions d'un utilisateur
        [HttpGet("ByUtilisateur/{utilisateurId}")]
        [ProducesResponseType(typeof(IEnumerable<DecisionSuspensionDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DecisionSuspensionDTO>>> GetByUtilisateurId(int utilisateurId)
        {
            var suspensions = await _suspensionManager.GetAllAsyncByUser(utilisateurId);
            var dto = _mapper.Map<IEnumerable<DecisionSuspensionDTO>>(suspensions);
            return Ok(dto);
        }

        // GET Suspension By Id
        [HttpGet("id/{id}")]
        [ProducesResponseType(typeof(DecisionSuspensionDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DecisionSuspensionDetailDTO>> GetById(int id)
        {
            var suspension = await _suspensionManager.GetByIdAsync(id);
            if (suspension == null)
                return NotFound();

            var dto = _mapper.Map<DecisionSuspensionDetailDTO>(suspension);
            return Ok(dto);
        }

        // POST Create
        [HttpPost]
        [ProducesResponseType(typeof(DecisionSuspensionDetailDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DecisionSuspensionDetailDTO>> Create(DecisionSuspensionCreateDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = _mapper.Map<Decision_suspension>(request);

            await _suspensionManager.AddAsync(entity);

            var dto = _mapper.Map<DecisionSuspensionDetailDTO>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.Decision_suspensionId }, dto);
        }

        // POST Search
        [HttpPost("Search")]
        [ProducesResponseType(typeof(IEnumerable<DecisionSuspensionDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DecisionSuspensionDTO>>> Search([FromBody] DecisionSuspensionSearchRequestDTO request)
        {
            var suspensions = await _suspensionManager.SearchAsync(request);
            var dto = _mapper.Map<IEnumerable<DecisionSuspensionDTO>>(suspensions);
            return Ok(dto);
        }
    }
}
