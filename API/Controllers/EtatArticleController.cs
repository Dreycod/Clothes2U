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
    private readonly IDataRepository<EtatArticle, int> _etatManager;
    private readonly IMapper _mapper;

    public EtatArticleController(IDataRepository<EtatArticle, int> manager, IMapper mapper)
    {
        _etatManager = manager;
        _mapper = mapper;
    }
    /// <summary>
    /// Récupère la liste de tous les états d'articles disponibles.
    /// </summary>
    /// <returns>Une collection d'états d'articles.</returns>
    /// <response code="200">Retourne la liste des états d'articles.</response>
    /// <response code="500">Erreur serveur interne.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EtatArticleDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<EtatArticleDTO>>> GetAllEtats()
    {
        IEnumerable<EtatArticle> EtatArticles = await _etatManager.GetAllAsync();
        IEnumerable<EtatArticleDTO> EtatArticlesDTO = _mapper.Map<IEnumerable<EtatArticleDTO>>(EtatArticles);
        return Ok(EtatArticlesDTO);
    }
}


