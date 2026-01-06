using Shared.DTO.MotInterdit;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;



[Route("api/[controller]")]
[ApiController]
public class MotInterditController : ControllerBase
{
    private readonly IMotInterditRepository _motInterditRepository;
    private readonly ICurrentUserService _currentUserService;
    private  readonly IMapper _mapper;

    public MotInterditController(IMotInterditRepository motInterditRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _motInterditRepository = motInterditRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    
    [HttpGet]
    [Authorize(Roles = "Admin, Moderateur")]
    [ProducesResponseType(typeof(IEnumerable<MotInterditDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MotInterditDTO>>> GetMotInterdit()
    {
        IEnumerable<MotInterdit> mots = await _motInterditRepository.GetAllAsync();
        IEnumerable<MotInterditDTO> motsDTO = _mapper.Map<List<MotInterditDTO>>(mots);
        return Ok(motsDTO);
    }
    [HttpGet("id/{id}")]
    [Authorize]
    [ProducesResponseType(typeof(MotInterditDTO),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MotInterditDTO>> GetById(int id)
    {
        var mot = await _motInterditRepository.GetByIdAsync(id);
        if (mot == null)
            return NotFound();
        
        MotInterditDTO motDTO = _mapper.Map<MotInterditDTO>(mot);
        return Ok(motDTO);
    }

    [HttpPost]
    [Authorize(Roles="Admin, Moderateur")]
    [ProducesResponseType(typeof(MotInterditDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MotInterditDTO>> AddMotInterdit([FromBody] MotInterditDTO motInterditDTO)
    {
        if (string.IsNullOrWhiteSpace(motInterditDTO.LibelleMot))
        {
            return BadRequest(new { 
                LibelleMot = "Le mot ne peut pas être vide." 
            });
        }
        var motToAdd = _mapper.Map<MotInterdit>(motInterditDTO);
        try
        {
            await _motInterditRepository.AddAsync(motToAdd);
            return CreatedAtAction(nameof(GetById), new { id = motToAdd.MotinterditId }, motToAdd);
        }
        catch (DbUpdateException dbEx)
        {
            if (dbEx.InnerException != null)
            {
                var innerMessage = dbEx.InnerException.Message.ToLower();
                if (innerMessage.Contains("unique") || 
                    innerMessage.Contains("duplicate") || 
                    innerMessage.Contains("duplicata") ||
                    innerMessage.Contains("constraint"))
                {
                    return BadRequest(new { 
                        LibelleMot = "Ce mot est déjà utilisé." 
                    });
                }
            }
            return StatusCode(500, new { 
                LibelleMot = "Erreur lors de l'ajout du mot." 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                LibelleMot = "Erreur inattendue lors de l'ajout du mot." 
            });
        }
    }

    [HttpDelete("id/{id}")]
    [Authorize(Roles="Admin, Moderateur")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMotInterdit(int id) 
    {
        MotInterdit? motInterditToDelete = await _motInterditRepository.GetByIdAsync(id);
        if (motInterditToDelete == null)
        {
            return NotFound();
        }
        await _motInterditRepository.DeleteAsync(motInterditToDelete);
        return NoContent();
    }
}