using Shared.DTO.Conversation;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Message;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class ConversationController : ControllerBase
{
    private readonly IDataRepository<Message, int> _messageManager;
    private readonly INotificationRepository _notificationManager;
    private readonly IConversationRepository<Conversation, int> _conversationManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IConversationService _conversationService;
    private readonly IMapper _mapper;
    
    public ConversationController(
        IConversationRepository<Conversation, int> manager,
        IDataRepository<Message, int> messageManager,
        INotificationRepository notificationManager,
        ICurrentUserService currentUserService,
        IConversationService conversationService, 
        IMapper mapper)
    {
        _notificationManager =  notificationManager;
        _conversationManager = manager;
        _currentUserService = currentUserService;
        _messageManager = messageManager;
        _conversationService = conversationService;
        _mapper = mapper;
    }
    
    
    [HttpGet("conversation/{id}")]
    [Authorize]
    public async Task<ActionResult<ConversationDTO>> GetById(int id)
    {
        var conversation = await _conversationManager.GetByIdAsync(id);
        if (conversation == null) return NotFound();

        var currentUserId = await _currentUserService.GetUserId();

        var conversationDTO = _mapper.Map<ConversationDTO>(conversation, opts =>
        {
            opts.Items["CurrentUserId"] = currentUserId;
        });
        
        await _notificationManager.DeleteMessageNotificationByConversationId(id, (int)currentUserId);
        return Ok(conversationDTO);
    }
    
    [HttpGet("utilisateur/{id}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ConversationDTO>>> GetByUtilisateurId(int id)
    {
        var conversations = await _conversationManager.GetAllAsyncByUser(id);
        var currentUserId = await _currentUserService.GetUserId();

        IEnumerable<ConversationDTO> conversationsDTO = _mapper.Map<IEnumerable<ConversationDTO>>(conversations, opt =>
        {
            opt.Items["CurrentUserId"] = currentUserId;
        });
        
        return Ok(conversationsDTO.OrderByDescending(c => c.LastMessageDate));
    }
    
    [HttpPost("annonce/{annonceId}")]
    [Authorize]
    public async Task<ActionResult<ConversationDTO>> GetOrCreate(int annonceId)
    {
        var currentUserId = await _currentUserService.GetUserId();
        var conversation = await _conversationService.GetOrCreateConversation(annonceId, (int)currentUserId);
        

        var dto = _mapper.Map<ConversationDTO>(conversation, opt =>
        {
            opt.Items["CurrentUserId"] = currentUserId;
        });
        return Ok(dto);
    }

    [HttpGet("messageById/{id}")]
    [Authorize(Roles = "Admin,Moderateur")]
    public async Task<ActionResult<MessageSignalementDTO>> GetMessageById(int id)
    {
        Message message =  await _messageManager.GetByIdAsync(id);
        if (message == null)
        {
            return NotFound();
        }
        MessageSignalementDTO messageDTO =  _mapper.Map<MessageSignalementDTO>(message);
        return messageDTO;
    }
    
}