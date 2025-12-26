using Shared.DTO.Message;
using API.Hubs;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using API.Services.Notifications;
using API.Services.Notifications.Events;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly IDataRepository<Message, int> _messageManager;
    private readonly IDataRepository<MessageTexte, int> _messageTexteManager;
    private readonly IDataRepository<MessageDemande, int> _messageDemandeManager;
    private readonly IDataRepository<MessageValidation, int> _messageValidationManager;
    private readonly IConversationRepository<Conversation, int> _conversationManager;
    private readonly IDataRepository<MessageContientImage, int> _messageContientImageManager;
    private readonly IPhotoService _photoService;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly IHubContext<ChatHub> _hubContext;

    public MessageController(
        IDataRepository<Message, int> messageManager,
        IDataRepository<MessageTexte, int> messageTexteManager,
        IConversationRepository<Conversation, int> conversationManager,
        IDataRepository<MessageDemande, int> messageDemandeManager,
        IDataRepository<MessageValidation, int> messageValidationManager,
        IDataRepository<MessageContientImage, int> messageContientImageManager,
        IPhotoService photoService,
        INotificationService notificationMessageManager,
        IMapper mapper,
        IHubContext<ChatHub> hubContext)
    {
        _messageManager = messageManager;
        _messageTexteManager = messageTexteManager;
        _conversationManager = conversationManager;
        _messageDemandeManager = messageDemandeManager;
        _messageValidationManager = messageValidationManager;
        _notificationService = notificationMessageManager;
        _messageContientImageManager = messageContientImageManager;
        _photoService = photoService;
        _mapper = mapper;
        _hubContext = hubContext;
    }
    private int? GetConnectedUserId()
    {
        if (User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int id))
            {
                return id;
            }
        }
        return null;
    }
    [Authorize]
    [HttpPost("texte")]
    [ProducesResponseType(typeof(MessageTextePostDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MessageTextePostDTO>> PostMessageTexte(MessageTextePostDTO dto)
     {
         if (!ModelState.IsValid)
             return BadRequest(ModelState);
 
         var message = new Message
         {
             MessageDate = DateTime.UtcNow,
             MessageLu = false,
             UtilisateurId = dto.UtilisateurId,
             ConversationId = dto.ConversationId
         };
         
         await _messageManager.AddAsync(message);
        
         var messageTexte = new MessageTexte
         {
             MessageId = message.MessageId,
             Content = dto.Content,
         };
         
         await _messageTexteManager.AddAsync(messageTexte);
         
         var listMessageContientPhotos = new List<MessageContientImage>();
         if (dto.Photos != null)
         {
             foreach (var photoDto in dto.Photos)
             {
                 var photo = await _photoService.UploadMessagePhotoAsync(photoDto);
                 if (photo != null)
                 {
                     await _messageContientImageManager.AddAsync(
                         new MessageContientImage
                         {
                             MessageTexteId = messageTexte.MessageTexteId, 
                             PhotoId = photo.PhotoId
                         });
                 }
             }
 
         }
         
         messageTexte.Photos = listMessageContientPhotos;
         
         var conversation = await _conversationManager.GetByIdAsync(dto.ConversationId);
 
         
         
         if (conversation != null)
         {
             int? targetUserId = await _conversationManager.GetOtherUser(dto.UtilisateurId, conversation);
             if (targetUserId != null)
             {
                 var notificationEvent = new NewMessageEvent
                 {
                     TargetUserId = (int)targetUserId,
                     MessageId = message.MessageId,
                     SenderId = dto.UtilisateurId,
                     MessagePreview = string.IsNullOrWhiteSpace(dto.Content)
                         ? "📷 Photo"
                         : dto.Content[..Math.Min(50, dto.Content.Length)]
                 };
                 await _notificationService.NotifyAsync(notificationEvent);
                 
                 // 🔥 BROADCASTER VIA SIGNALR
                 //Console.WriteLine($"[MessageController] 📡 Broadcasting to group: conversation_{message.ConversationId}");
                 
                 await _hubContext.Clients
                     .Group($"conversation_{message.ConversationId}")
                     .SendAsync("ReceiveMessage", 
                         message.ConversationId, 
                         message.UtilisateurId, 
                         dto.Content, 
                         messageTexte.Photos,
                         message.MessageDate);
                 
                 Console.WriteLine($"[MessageController] ✅ Message broadcasted successfully");
             }
             else
             {
                 Console.WriteLine($"[MessageController] ❌ Target user not found");
                 return BadRequest("Utilisateur non autorisé pour cette conversation");
             }
         }
         else
         {
             Console.WriteLine($"[MessageController] ❌ Conversation {dto.ConversationId} not found");
             return BadRequest("Conversation introuvable");
         }
         
         return CreatedAtAction(nameof(GetById), new { id = message.MessageId }, dto);
     }

    [HttpPost("demande")]
    [ProducesResponseType(typeof(MessageDemandePostDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MessageDemandePostDTO>> PostMessageDemande(MessageDemandePostDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var message = new Message
        {
            MessageDate = DateTime.UtcNow,
            MessageLu = false,
            UtilisateurId = dto.UtilisateurId,
            ConversationId = dto.ConversationId
        };
        
        await _messageManager.AddAsync(message);

        var messageDemande = new MessageDemande
        {
            MessageId = message.MessageId,
            DemandeId = dto.DemandeId
        };
        
        await _messageDemandeManager.AddAsync(messageDemande);

        return CreatedAtAction(nameof(GetById), new { id = message.MessageId }, dto);
    }

    [HttpPost("validation")]
    [ProducesResponseType(typeof(MessageValidationPostDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MessageValidationPostDTO>> PostMessageValidation(MessageValidationPostDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var message = new Message
        {
            MessageDate = DateTime.UtcNow,
            MessageLu = false,
            UtilisateurId = dto.UtilisateurId,
            ConversationId = dto.ConversationId
        };
        
        await _messageManager.AddAsync(message);

        var messageValidation = new MessageValidation
        {
            MessageId = message.MessageId
        };
        
        await _messageValidationManager.AddAsync(messageValidation);

        return CreatedAtAction(nameof(GetById), new { id = message.MessageId }, dto);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetById(int id)
    {
        var message = await _messageManager.GetByIdAsync(id);
        if (message == null)
            return NotFound();
            
        return Ok(message);
    }


    [HttpDelete("Delete/TexteMessage/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTexteMessage(int id)
    {
        Message? message = await _messageManager.GetByIdAsync(id);
        if (message == null)
        {
            return NotFound();
        }
        
        MessageTexte? messageTexte = await _messageTexteManager.GetByIdAsync(message.MessageTexte.MessageTexteId);
        if (messageTexte == null)
        {
            return NotFound();
        }
        
        await _messageTexteManager.DeleteAsync(messageTexte);
        await _messageManager.DeleteAsync(message);
        return NoContent();
    }

    [HttpPut("markAsRead/{messageId}")]
    public async Task<IActionResult> MarkAsRead(int messageId)
    {
        var message = await _messageManager.GetByIdAsync(messageId);
        if (message == null) return NotFound();
        
        var newMessage = _mapper.Map<Message>(message);
        newMessage.MessageLu = true; 
        
        await _messageManager.UpdateAsync(newMessage);
        
        // Notifier SignalR
        // Dans MessageController après avoir sauvegardé le message
        await _hubContext.Clients.Group($"conversation_{message.ConversationId}")
            .SendAsync("ReceiveMessage", message.ConversationId, message.UtilisateurId, message.MessageTexte.Content, message.MessageDate);

        return NoContent();
    }
    
}