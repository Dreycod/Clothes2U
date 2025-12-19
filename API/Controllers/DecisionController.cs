using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Decision;

namespace API.Controllers;




[ApiController]
[Route("api/[controller]")]
public class DecisionController : ControllerBase
{
    private readonly IDecisionRepository _decisionManager;
    private readonly IMapper _mapper;

    public DecisionController(IDecisionRepository decisionManager, IMapper mapper)
    {
        _decisionManager = decisionManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ActionResult<DecisionDTO>>> GetAllDecisionsByModerateurId(int id)
    {
        IEnumerable<Decision> decisions = await _decisionManager.GetAllDecisionsByModerateurId(id);
        IEnumerable<DecisionDTO> decisionsDTO = _mapper.Map<IEnumerable<DecisionDTO>>(decisions);
        return Ok(decisionsDTO);
    }

    [HttpPost]
    public async Task<ActionResult<DecisionDTO>> CreateDecision(DecisionCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        throw new NotImplementedException();
    }
}