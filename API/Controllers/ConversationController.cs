using Shared.DTO.Conversation;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using API.Services;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.DTO.Message;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class ConversationController : ControllerBase
{
    private readonly IMessageRepository _messageManager;
    private readonly INotificationRepository _notificationManager;
    private readonly IConversationRepository<Conversation, int> _conversationManager;
    private readonly IDataRepository<StatutConversation, int> _statutConversationManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IConversationService _conversationService;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    
    public ConversationController(
        IConversationRepository<Conversation, int> manager,
        IMessageRepository messageManager,
        IDataRepository<StatutConversation, int> statutConversationManager,
        INotificationRepository notificationManager,
        ICurrentUserService currentUserService,
        IConversationService conversationService, 
        INotificationService notificationService,
        IMapper mapper)
    {
        _notificationManager =  notificationManager;
        _conversationManager = manager;
        _currentUserService = currentUserService;
        _messageManager = messageManager;
        _statutConversationManager = statutConversationManager;
        _conversationService = conversationService;
        _mapper = mapper;
        _notificationService =  notificationService;
    }
    
    // <summary>
    /// Récupère une conversation spécifique par son identifiant.
    /// Supprime également les notifications de messages pour cette conversation pour l'utilisateur connecté.
    /// </summary>
    /// <param name="id">L'identifiant de la conversation.</param>
    /// <returns>Les détails de la conversation.</returns>
    /// <response code="200">Retourne la conversation trouvée.</response>
    /// <response code="404">La conversation n'existe pas.</response>
    /// <response code="401">L'utilisateur n'est pas authentifié.</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpGet("conversation/{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ConversationDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ConversationDTO>> GetById(int id)
    {
        var conversation = await _conversationManager.GetByIdAsync(id);
        if (conversation == null) return NotFound();
        int userId = await _currentUserService.GetUserIdOrThrow();
        var conversationDTO = _mapper.Map<ConversationDTO>(conversation, opts =>
        {
            opts.Items["CurrentUserId"] = userId;
        });
        await _notificationService.DeleteMessagesNotificationByConversationId(id, userId);
        return Ok(conversationDTO);
    }
    /// <summary>
    /// Récupère la liste de tous les statuts de conversation disponibles.
    /// </summary>
    /// <returns>La liste des statuts de conversation.</returns>
    /// <response code="200">Retourne la liste des statuts.</response>
    /// <response code="404">Aucun statut n'a été trouvé.</response>
    /// <response code="401">L'utilisateur n'est pas authentifié.</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpGet("statutConversation")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<StatutConversationDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<StatutConversationDTO>> GetStatutConversation()
    {
        var statutConversation = await _statutConversationManager.GetAllAsync();
        if (statutConversation == null) return NotFound();
        return Ok(statutConversation);
    }
    /// <summary>
    /// Récupère toutes les conversations d'un utilisateur spécifique, triées par date du dernier message.
    /// </summary>
    /// <param name="id">L'identifiant de l'utilisateur.</param>
    /// <returns>La liste des conversations de l'utilisateur, triées par date décroissante.</returns>
    /// <response code="200">Retourne la liste des conversations.</response>
    /// <response code="401">L'utilisateur n'est pas authentifié.</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpGet("utilisateur/{id}")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<ConversationDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// <summary>
    /// Récupère ou crée une conversation pour une annonce spécifique.
    /// Si une conversation existe déjà entre l'utilisateur connecté et le propriétaire de l'annonce, elle est retournée.
    /// Sinon, une nouvelle conversation est créée.
    /// </summary>
    /// <param name="annonceId">L'identifiant de l'annonce concernée.</param>
    /// <returns>La conversation existante ou nouvellement créée.</returns>
    /// <response code="200">Retourne la conversation (existante ou créée).</response>
    /// <response code="401">L'utilisateur n'est pas authentifié.</response>
    /// <response code="404">L'annonce n'existe pas.</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpPost("annonce/{annonceId}")]
    [Authorize]
    [ProducesResponseType(typeof(ConversationDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Récupère un message spécifique par son identifiant (incluant les détails de signalement).
    /// Accessible uniquement aux administrateurs et modérateurs.
    /// </summary>
    /// <param name="id">L'identifiant du message.</param>
    /// <returns>Le message avec ses informations de signalement.</returns>
    /// <response code="200">Retourne le message trouvé.</response>
    /// <response code="404">Le message n'existe pas.</response>
    /// <response code="401">L'utilisateur n'est pas authentifié.</response>
    /// <response code="403">L'utilisateur n'a pas les droits nécessaires (Admin ou Modérateur requis).</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpGet("messageById/{id}")]
    [Authorize(Roles = "Admin,Moderateur")]
    [ProducesResponseType(typeof(MessageSignalementDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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