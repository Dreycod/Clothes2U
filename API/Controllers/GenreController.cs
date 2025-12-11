using API.DTO;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenreController : ControllerBase
{
    private readonly ICaracteristiquesRepository<Genre> _genreManager;
    private readonly IMapper _mapper;
    
    public GenreController(ICaracteristiquesRepository<Genre> manager, IMapper mapper)
    {
        _genreManager = manager;
        _mapper = mapper;
    }
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GenreDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<GenreDTO>>> GetAllGenres()
    {
        IEnumerable<Genre> genres = await _genreManager.GetAllAsync();
        IEnumerable<GenreDTO> genresDTO = _mapper.Map<IEnumerable<GenreDTO>>(genres);
        return Ok(genresDTO);
    }
    
}