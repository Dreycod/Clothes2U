using System.Collections.ObjectModel;
using Shared.DTO.Message;
using API.Hubs;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using API.Models.Repository.Managers;
using API.Services;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shared.DTO;
using Shared.DTO.Notification;

namespace API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly IMessageRepository _messageManager;
    private readonly IDataRepository<MessageTexte, int> _messageTexteManager;
    private readonly IMessageDemandeRepository _messageDemandeManager;
    private readonly IDataRepository<MessageEstPayee, int> _messageValidationManager;
    private readonly IDataRepository<MessageEnvoieColis, int> _messageEnvoieColisManager;
    private readonly IConversationRepository<Conversation, int> _conversationManager;
    private readonly IDataRepository<MessageEstRecu, int> _messageEstRecuManager;
    private readonly IDataRepository<MessageContientImage, int> _messageContientImageManager;
    private readonly IPhotoRepository _photoService;
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceService;
    private readonly IOrderRepository _orderService;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IMessageService _messageService;

    public MessageController(
        IMessageRepository messageManager,
        IDataRepository<MessageTexte, int> messageTexteManager,
        IConversationRepository<Conversation, int> conversationManager,
        IMessageDemandeRepository messageDemandeManager,
        IDataRepository<MessageEstPayee, int> messageValidationManager,
        IDataRepository<MessageContientImage, int> messageContientImageManager,
        IDataRepository<MessageEnvoieColis, int> messageEnvoieColisManager,
        IDataRepository<MessageEstRecu, int> messageEstRecuManager,
        IAnnonceRepository<Annonce, int, FilterDTO> annonceService,
        IOrderRepository orderService,
        IPhotoRepository photoService,
        INotificationService notificationMessageManager,
        IMapper mapper,
        IMessageService messageService,
        IHubContext<ChatHub> hubContext)
    {
        _messageManager = messageManager;
        _messageTexteManager = messageTexteManager;
        _conversationManager = conversationManager;
        _messageDemandeManager = messageDemandeManager;
        _messageValidationManager = messageValidationManager;
        _messageEnvoieColisManager = messageEnvoieColisManager;
        _messageEstRecuManager = messageEstRecuManager;
        _notificationService = notificationMessageManager;
        _messageContientImageManager = messageContientImageManager;
        _annonceService = annonceService;
        _orderService = orderService;
        _photoService = photoService;
        _mapper = mapper;
        _hubContext = hubContext;
        _messageService = messageService;
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
             MessageStatut = true,
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
         
         var listMessageContientPhotos = new Collection<MessageContientImage>();
         var photoIds = new List<int>();
         if (dto.Photos != null)
         {
             foreach (var photoDto in dto.Photos)
             {
                 var photo = await _photoService.AddPhotoAsync(photoDto);
                 photoIds.Add(photo.PhotoId);
                 if (photo != null)
                 {
                     var mesconima = new MessageContientImage
                     {
                         MessageTexteId = messageTexte.MessageTexteId,
                         PhotoId = photo.PhotoId
                     };
                     Console.WriteLine($"cjbhekhjwfvbhrewbfvkjfewbrvkhjberkjhfvgkehwgrvkberhwkvgnreiwnhgjkerwhuogvub :{messageTexte.MessageTexteId} + {photo.PhotoId}");
                     await _messageContientImageManager.AddAsync(mesconima);
                     listMessageContientPhotos.Add(mesconima);
                     Console.WriteLine("cjbhekhjwfvbhrewbfvkjfewbrvkhjberkjhfvgkehwgrvkberhwkvgnreiwnhgjkerwhuogvub1212352634823");
                 }
             }
 
         }
         Console.WriteLine($"Photos liées : {listMessageContientPhotos.Count}");
         messageTexte.Photos = listMessageContientPhotos;
         
         var conversation = await _conversationManager.GetByIdAsync(dto.ConversationId);
 
         if (conversation != null)
         {
             int? targetUserId = await _conversationManager.GetOtherUser(dto.UtilisateurId, conversation);
             if (targetUserId != null)
             {

                 NotificationMessageCreateDTO notification = new NotificationMessageCreateDTO()
                 {
                    TypeId = 1,
                    UtilisateurId = (int)targetUserId,
                    MessageId = message.MessageId,
                    MessagePreview = string.IsNullOrWhiteSpace(dto.Content)
                        ? "📷 Photo"
                        : dto.Content[..Math.Min(50, dto.Content.Length)]
                 };
                 await _notificationService.CreateNotification(notification);
                 await _messageService.SendMessageCount((int)targetUserId);
                 // 🔥 BROADCASTER VIA SIGNALR
                 //Console.WriteLine($"[MessageController] 📡 Broadcasting to group: conversation_{message.ConversationId}");
                 
                 try
                 {
                     await _hubContext.Clients.Group($"conversation_{dto.ConversationId}")
                         .SendAsync("ReceiveMessage", 
                             dto.ConversationId, 
                             dto.UtilisateurId, 
                             dto.Content,
                             photoIds,
                             message.MessageDate,
                             message.MessageId);
        
                     Console.WriteLine($"[MessageController] ✅ Message notification sent");
                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine($"[MessageController] ❌ Error sending message notification: {ex.Message}");
                 }
                 
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
        Console.WriteLine("--------------------------------------------------------------------------");
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

        var messageDemande = new MessageDemande()
        {
            MessageId = message.MessageId,
            DemandeId = dto.DemandeId,
            PrixPropose = dto.PrixPropose,
            EstAcceptee = false,
            EstRepondue = false
        };
        
        await _messageDemandeManager.AddAsync(messageDemande);
        
        var conversation = await _conversationManager.GetByIdAsync(dto.ConversationId);

        if (conversation != null)
        {
            int? targetUserId = await _conversationManager.GetOtherUser(dto.UtilisateurId, conversation);
            if (targetUserId != null)
            {
                NotificationPropositionCreateDTO notification = new NotificationPropositionCreateDTO()
                {
                    UtilisateurId = (int)targetUserId,
                    TypeId = 6,
                    PropositionId = messageDemande.MessageDemandeId
                };
                await _notificationService.CreateNotification(notification);

                // 🔥 BROADCASTER VIA SIGNALR
                //Console.WriteLine($"[MessageController] 📡 Broadcasting to group: conversation_{message.ConversationId}");

                await _hubContext.Clients
                    .Group($"conversation_{message.ConversationId}")
                    .SendAsync("ReceivePriceProposal",
                        message.ConversationId,
                        message.MessageId,
                        message.UtilisateurId,
                        messageDemande.PrixPropose,
                        message.MessageDate);
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

    [HttpPost("payee")]
    [ProducesResponseType(typeof(MessageEstPayeePostDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MessageEstPayeePostDTO>> PostMessagePayee(MessageEstPayeePostDTO dto)
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

        
        var messageValidation = new MessageEstPayee()
        {
            MessageId = message.MessageId,
        };
        
        await _messageValidationManager.AddAsync(messageValidation);
        
        try
        {
            await _hubContext.Clients.Group($"conversation_{dto.ConversationId}")
                .SendAsync("ReceivePayment", 
                    dto.ConversationId, 
                    message.MessageId, 
                    dto.UtilisateurId, 
                    message.MessageDate);
        
            Console.WriteLine($"[MessageController] ✅ Payment notification sent for conversation {dto.ConversationId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MessageController] ❌ Error sending payment notification: {ex.Message}");
        }

        return CreatedAtAction(nameof(GetById), new { id = message.MessageId }, dto);
    }
    
    [HttpPost("envoieColis")]
    [ProducesResponseType(typeof(MessageEstPayeePostDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MessageEstPayeePostDTO>> PostMessageEnvoieColis(MessageEnvoisColisPostDTO dto)
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

        var photo = await _photoService.AddPhotoAsync(dto.Photo);

        var messageEnvoieColis = new MessageEnvoieColis
        {
            MessageId = message.MessageId,
            PhotoId = photo.PhotoId,
            MessageEstPayeeId = dto.MessageEstPayeeId
        };
        
        await _messageEnvoieColisManager.AddAsync(messageEnvoieColis);
        
        var messagePayee = await _messageValidationManager.GetByIdAsync(dto.MessageEstPayeeId);
        
        messagePayee.EstEnvoye = true;
        
        await _messageValidationManager.UpdateAsync(messagePayee);
        
        var order = await _orderService.GetByIdAsync(messagePayee.Message.Conversation.Commandes.LastOrDefault().ConversationId);
        
        await _orderService.UpdateOrderStatusAsync(order.ConversationId, 2);
        
        try
        {
            await _hubContext.Clients.Group($"conversation_{dto.ConversationId}")
                .SendAsync("ReceiveColisEnvoye", 
                    dto.ConversationId, 
                    message.MessageId, 
                    dto.UtilisateurId, 
                    photo.PhotoId,
                    message.MessageDate,
                    dto.MessageEstPayeeId);
        
            Console.WriteLine($"[MessageController] ✅ Colis envoyé notification sent for conversation {dto.ConversationId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MessageController] ❌ Error sending colis envoyé notification: {ex.Message}");
        }

        return CreatedAtAction(nameof(GetById), new { id = message.MessageId }, dto);
    }

    [HttpPost("recuColis")]
    [ProducesResponseType(typeof(MessageEstRecuPostDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MessageEstRecuPostDTO>> PostMessageRecuColis(MessageEstRecuPostDTO dto)
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

        var messageRecu = new MessageEstRecu
        {
            MessageId = message.MessageId,
            EstConforme = dto.EstConforme,
            MessageEstEnvoieId = dto.MessageEstEnvoieId
        };

        if (!dto.EstConforme)
        {
            if (dto.Photo == null) return BadRequest("Photo manquante");
            
            var photo = await _photoService.AddPhotoAsync(dto.Photo);
            
            if (dto.Description == null) return BadRequest("Description manquante");
            
            messageRecu.Description = dto.Description;
            messageRecu.PhotoId = photo.PhotoId;
        }
        await _messageEstRecuManager.AddAsync(messageRecu);
        
        var order = await _orderService.GetByIdAsync(messageRecu.Message.Conversation.Commandes.LastOrDefault().ConversationId);
        
        await _orderService.UpdateOrderStatusAsync(order.ConversationId, 3);
        
        try
        {
            await _hubContext.Clients.Group($"conversation_{dto.ConversationId}")
                .SendAsync("ReceiveColisRecu", 
                    dto.ConversationId, 
                    message.MessageId, 
                    dto.UtilisateurId, 
                    dto.EstConforme,
                    messageRecu.PhotoId != null ? messageRecu.PhotoId : 0,
                    dto.Description,
                    message.MessageDate,
                    dto.MessageEstEnvoieId);
        
            Console.WriteLine($"[MessageController] ✅ Colis reçu notification sent for conversation {dto.ConversationId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MessageController] ❌ Error sending colis reçu notification: {ex.Message}");
        }
        
        
        return CreatedAtAction(nameof(GetById), new { id = message.MessageId }, dto);
        
    }
    

    [HttpPut("annulePayement/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AnnulePayement(int id)
    {
        var messagePayee = await _messageValidationManager.GetByIdAsync(id);
        
        if (messagePayee == null) return NotFound();

        if (messagePayee.EstEnvoye) return BadRequest();
        
        if (messagePayee.EstAnnule) return BadRequest();
        
        
        
        messagePayee.EstAnnule = true;
        
        await _messageValidationManager.UpdateAsync(messagePayee);
        
        var annonce = await _annonceService.GetByIdAsync(messagePayee.Message.ConversationId);
        annonce.StatutAnnonceId = 1;
        await _annonceService.UpdateAsync(annonce);
        
        var order = await _orderService.GetByIdAsync(messagePayee.Message.Conversation.Commandes.LastOrDefault().ConversationId);
        await _orderService.UpdateOrderStatusAsync(order.ConversationId, 4);
        
        try
        {
            await _hubContext.Clients.Group($"conversation_{messagePayee.Message.ConversationId}")
                .SendAsync("ReceivePaymentCancelled", 
                    messagePayee.Message.ConversationId, 
                    messagePayee.MessageId,
                    messagePayee.Message.UtilisateurId);
        
            Console.WriteLine($"[MessageController] ✅ Payment cancelled notification sent for conversation {messagePayee.Message.ConversationId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MessageController] ❌ Error sending payment cancelled notification: {ex.Message}");
        }
       
        return NoContent();
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
        
        message.MessageLu = true; 
        
        await _messageManager.UpdateAsync(message);
        
        // Notifier SignalR
        // Dans MessageController après avoir sauvegardé le message
        try
        {
            await _hubContext.Clients.Group($"conversation_{message.ConversationId}")
                .SendAsync("MessagesRead", message.ConversationId, message.UtilisateurId);
        
            Console.WriteLine($"[MessageController] ✅ MessagesRead notification sent");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MessageController] ❌ Error sending MessagesRead notification: {ex.Message}");
        }
        return NoContent();
    }

    [HttpPut("Answer/{messageId}/{answer}")]
    public async Task<IActionResult> AnswerPriceProposal(int messageId, bool answer)
    {
        var messageDemande = await _messageDemandeManager.GetByMessageIdAsync(messageId);
        if (messageDemande == null) return NotFound();
        
        messageDemande.EstAcceptee = answer;
        messageDemande.EstRepondue = true;
        
        await _messageDemandeManager.UpdateAsync(messageDemande);
        
        var conversation = await _conversationManager.GetByIdAsync(messageDemande.Message.ConversationId);

        conversation.Prix = messageDemande.PrixPropose;
        
        await _conversationManager.UpdateAsync(conversation);
        
        Console.WriteLine(messageDemande.Message.ConversationId);
        
        await _hubContext.Clients.
            Group($"conversation_{messageDemande.Message.ConversationId}")
            .SendAsync("ProposalResponseReceived", 
                messageDemande.Message.ConversationId,
                messageId, 
                answer);
        return NoContent();
    }
    
}