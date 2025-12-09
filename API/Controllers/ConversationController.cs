using API.DTO.Conversation;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class ConversationController : ControllerBase
{
    private readonly IDataRepository<Message, int> _messageManager;
    private readonly IConversationRepository<Conversation, int> _conversationManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    
    public ConversationController(IConversationRepository<Conversation, int> manager,IDataRepository<Message, int> messageManager,ICurrentUserService currentUserService, IMapper mapper)
    {
        _conversationManager = manager;
        _currentUserService = currentUserService;
        _messageManager = messageManager;
        _mapper = mapper;
    }
    
    
    [HttpGet("conversation/{id}")]
    [Authorize]
    public async Task<ActionResult<ConversationDetailDTO>> GetById(int id)
    {
        var conversation = await _conversationManager.GetByIdAsync(id);
        if (conversation == null) return NotFound();

        var currentUserId = _currentUserService.GetUserId();

        var conversationDTO = _mapper.Map<ConversationDetailDTO>(conversation, opts =>
        {
            opts.Items["CurrentUserId"] = currentUserId;
        });

        return Ok(conversationDTO);
    }
    
    [HttpGet("utilisateur/{id}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ConversationDTO>>> GetByUtilisateurId(int id)
    {
        var conversations = await _conversationManager.GetAllAsyncByUser(id);
        IEnumerable<ConversationDTO> conversationsDTO = _mapper.Map<IEnumerable<ConversationDTO>>(conversations, opt =>
            {
                opt.Items["CurrentUserId"] = _currentUserService.GetUserId();
            })
            ;
        
        return Ok(conversationsDTO.OrderByDescending(c => c.LastMessageDate));
    }
    
}