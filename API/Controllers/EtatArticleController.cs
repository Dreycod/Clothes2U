using Shared.DTO;
using Shared.DTO.EtatArticle;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class EtatArticleController: ControllerBase
{
    private readonly IEtatArticleRepository _etatManager;
    private readonly IMapper _mapper;

    public EtatArticleController(IEtatArticleRepository manager, IMapper mapper)
    {
        _etatManager = manager;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EtatArticleDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<EtatArticleDTO>>> GetAllEtats()
    {
        IEnumerable<EtatArticle> EtatArticles = await _etatManager.GetAllAsync();
        IEnumerable<EtatArticleDTO> EtatArticlesDTO = _mapper.Map<IEnumerable<EtatArticleDTO>>(EtatArticles);
        return Ok(EtatArticlesDTO);
    }
    [HttpGet("byCategoryId/{id}")]
    [ProducesResponseType(typeof(IEnumerable<EtatArticle>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<EtatArticleDTO>>> GetAllEtatArticleByCategorieId(int id)
    {
        IEnumerable<EtatArticle> EtatArticles = await _etatManager.GetAllAsyncByIdentifier(id);
        IEnumerable<EtatArticleDTO> EtatArticlesDTO = _mapper.Map<IEnumerable<EtatArticleDTO>>(EtatArticles);
        return Ok(EtatArticlesDTO);
    }
}


