using API.DTO.Message;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly IDataRepository<Message, int> _messageManager;
    private readonly IDataRepository<MessageTexte, int> _messageTexteManager;
    private readonly IDataRepository<MessageDemande, int> _messageDemandeManager;
    private readonly IDataRepository<MessageValidation, int> _messageValidationManager;
    private readonly IMapper _mapper;

    public MessageController(
        IDataRepository<Message, int> messageManager,
        IDataRepository<MessageTexte, int> messageTexteManager,
        IDataRepository<MessageDemande, int> messageDemandeManager,
        IDataRepository<MessageValidation, int> messageValidationManager,
        IMapper mapper)
    {
        _messageManager = messageManager;
        _messageTexteManager = messageTexteManager;
        _messageDemandeManager = messageDemandeManager;
        _messageValidationManager = messageValidationManager;
        _mapper = mapper;
    }

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
            ContenuMessage = dto.ContenuMessage
        };
        
        await _messageTexteManager.AddAsync(messageTexte);

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
    
}