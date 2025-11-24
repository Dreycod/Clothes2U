using API.DTO.Conversation;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class ConversationController : ControllerBase
{
    private readonly IConversationRepository<Conversation, int> _conversationManager;
    private readonly IMapper _mapper;
    
    public ConversationController(IConversationRepository<Conversation, int> manager, IMapper mapper)
    {
        _conversationManager = manager;
        _mapper = mapper;
    }
    
    [HttpGet("conversation/{id}")]
    public async Task<ActionResult<ConversationDetailDTO>> GetById(int id)
    {
        var conversation = await _conversationManager.GetByIdAsync(id);
        if (conversation == null) return NotFound();
        ConversationDetailDTO conversationDTO = _mapper.Map<ConversationDetailDTO>(conversation);
        return Ok(conversationDTO);
    }
    
    [HttpGet("utilisateur/{id}")]
    public async Task<ActionResult<IEnumerable<ConversationDTO>>> GetByUtilisateurId(int id)
    {
        var conversations = await _conversationManager.GetAllAsyncByUser(id);
        IEnumerable<ConversationDTO> conversationsDTO = _mapper.Map<IEnumerable<ConversationDTO>>(conversations);
        return Ok(conversationsDTO);
    }
    
}