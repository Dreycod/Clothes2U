using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;



[Route("api/[controller]")]
[ApiController]
public class MotInterditController : ControllerBase
{
    private readonly IDataRepository<MotInterdit, int> _motInterditRepository;
    private readonly ICurrentUserService _currentUserService;

    public MotInterditController(IDataRepository<MotInterdit, int> motInterditRepository,
        ICurrentUserService currentUserService)
    {
        _motInterditRepository = motInterditRepository;
        _currentUserService = currentUserService;
    }

    
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<MotInterdit>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<MotInterdit>>> GetMotInterdit()
    {
        int? currentUserId = await _currentUserService.GetUserId();
        if (currentUserId == null)
        {
            return Unauthorized("Le user est null");
        }
        Utilisateur user = await _currentUserService.GetUser();
        if (user.Role.RoleUtilisateurLibelle != "Admin" && user.Role.RoleUtilisateurLibelle != "Modérateur")
        {
            return Unauthorized("vous n'avez pas le bon role");
        }

        return Ok(await _motInterditRepository.GetAllAsync());
    }
}