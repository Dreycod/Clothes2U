using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class PhotoController : ControllerBase
{
    private readonly IDataRepository<Photo, int> _photoManager;
    private readonly IMapper _mapper;
    
    public PhotoController(IDataRepository<Photo, int> manager, IMapper mapper)
    {
        _photoManager = manager;
        _mapper = mapper;
    }
    
    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(Photo),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Photo>> GetById(int id)
    {
        var photo = await _photoManager.GetByIdAsync(id);
        if (photo == null)
        {
            return NotFound();
        }
        return photo;
    }
}