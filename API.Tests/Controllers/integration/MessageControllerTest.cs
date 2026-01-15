using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.Hubs;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared.DTO;
using Shared.DTO.Message;
using Shared.DTO.Notification;

namespace API.Tests.Controllers.integration;

[TestClass]
[TestCategory("integration")]
public class MessageControllerTest
{
    private Clothes2UDbContext _context;
    private MessageController _controller;
    private IMapper _mapper;
    private Mock<IHubContext<ChatHub>> _mockHubContext;
    private Mock<INotificationService> _mockNotificationService;
    private Mock<IMessageService> _mockMessageService;
    
    private const int TEST_USER_1_ID = 1;
    private const int TEST_USER_2_ID = 2;
    private const int TEST_CONVERSATION_ID = 1;
    private const int TEST_ANNONCE_ID = 1;
    
    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        _context = new Clothes2UDbContext(builder.Options);

        CleanupDatabase();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AnnonceMappingProfile>();
            cfg.AddProfile<MessageMapperProfile>();
        });
        _mapper = config.CreateMapper();

        // Setup mocks
        _mockHubContext = new Mock<IHubContext<ChatHub>>();
        _mockNotificationService = new Mock<INotificationService>();
        _mockMessageService = new Mock<IMessageService>();
        
        // Setup SignalR mock
        var mockClients = new Mock<IHubClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        
        _mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
        mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

        // Setup notification service mock
        // _mockNotificationService
        //     .Setup(s => s.CreateNotification(It.IsAny<NotificationMessageCreateDTO>()))
        //     .ReturnsAsync(new NotificationDTO());
        //
        _mockMessageService
            .Setup(s => s.SendMessageCount(It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Initialize managers
        var messageManager = new MessageManager(_context);
        var messageTexteManager = new MessageTexteManager(_context);
        var conversationManager = new ConversationManager(_context);
        var messageDemandeManager = new MessageDemandeManager(_context);
        var messageValidationManager = new MessageEstPayeeManager(_context);
        var messageEnvoieColisManager = new MessageEnvoieColisManager(_context);
        var messageEstRecuManager = new MessageEstRecuManager(_context);
        var messageContientImageManager = new MessageContientImageManager(_context);
        var photoService = new PhotoManager(_context);
        var annonceService = new AnnonceManager(_context);
        var orderService = new OrderManager(_context);

        _controller = new MessageController(
            messageManager,
            messageTexteManager,
            conversationManager,
            messageDemandeManager,
            messageValidationManager,
            messageContientImageManager,
            messageEnvoieColisManager,
            messageEstRecuManager,
            annonceService,
            orderService,
            photoService,
            _mockNotificationService.Object,
            _mapper,
            _mockMessageService.Object,
            _mockHubContext.Object
        );

        InitializeDefaultObjects();
    }

    private void CleanupDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    private Utilisateur _user1;
    private Utilisateur _user2;
    private Conversation _conversation;
    private Annonce _annonce;
    private RoleUtilisateur _defaultRole;
    private StatutUtilisateur _defaultStatut;
    private StatutAnnonce _defaultStatutAnnonce;
    private StatutConversation _defaultStatutConversation;

    private void InitializeDefaultObjects()
    {
        _defaultRole = new RoleUtilisateur
        {
            RoleUtilisateurId = 1,
            RoleUtilisateurLibelle = "Utilisateur"
        };

        _defaultStatut = new StatutUtilisateur
        {
            StatutUtilisateurId = 1,
            StatutLibelle = "Actif"
        };

        _defaultStatutAnnonce = new StatutAnnonce
        {
            StatutAnnonceId = 1,
            StatutLibelle = "Active"
        };

        _defaultStatutConversation = new StatutConversation
        {
            StatutConversationId = 1,
            StatutConversationLibelle = "Active"
        };

        _user1 = new Utilisateur
        {
            UtilisateurId = TEST_USER_1_ID,
            Login = "User1",
            Email = "user1@example.com",
            Password = "pwd123",
            Description = "User 1",
            Dateinscription = DateTime.Now,
            ValidEmail = true,
            ValidTelephone = false,
            PreferenceCookies = false,
            PreferenceTheme = false,
            PreferenceNotifMail = false,
            RoleId = _defaultRole.RoleUtilisateurId,
            StatutId = _defaultStatut.StatutUtilisateurId,
            Role = _defaultRole,
            Statut = _defaultStatut
        };

        _user2 = new Utilisateur
        {
            UtilisateurId = TEST_USER_2_ID,
            Login = "User2",
            Email = "user2@example.com",
            Password = "pwd456",
            Description = "User 2",
            Dateinscription = DateTime.Now,
            ValidEmail = true,
            ValidTelephone = false,
            PreferenceCookies = false,
            PreferenceTheme = false,
            PreferenceNotifMail = false,
            RoleId = _defaultRole.RoleUtilisateurId,
            StatutId = _defaultStatut.StatutUtilisateurId,
            Role = _defaultRole,
            Statut = _defaultStatut
        };

        _annonce = new Annonce
        {
            AnnonceId = TEST_ANNONCE_ID,
            Title = "Test Annonce",
            Prix = 50,
            UtilisateurId = TEST_USER_1_ID,
            StatutAnnonceId = _defaultStatutAnnonce.StatutAnnonceId,
            Utilisateur = _user1,
            Statut = _defaultStatutAnnonce
        };

        _conversation = new Conversation
        {
            ConversationId = TEST_CONVERSATION_ID,
            AnnonceId = TEST_ANNONCE_ID,
            //Acheteur = TEST_USER_2_ID,
            StatutConversationId = _defaultStatutConversation.StatutConversationId,
            LAnnonce = _annonce,
            //Acheteur = _user2,
            StatutConversation = _defaultStatutConversation
        };
    }

    private async Task SeedBasicData()
    {
        _context.RolesUtilisateurs.Add(_defaultRole);
        _context.StatutUtilisateurs.Add(_defaultStatut);
        _context.StatutAnnonces.Add(_defaultStatutAnnonce);
        _context.StatutConversations.Add(_defaultStatutConversation);
        _context.Utilisateurs.AddRange(_user1, _user2);
        _context.Annonces.Add(_annonce);
        _context.Conversations.Add(_conversation);
        await _context.SaveChangesAsync();
    }

    #region PostMessageTexte Tests

    [TestMethod]
    public async Task PostMessageTexte_ShouldCreateMessage_WhenValidData()
    {
        // Given
        await SeedBasicData();
        
        var dto = new MessageTextePostDTO
        {
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID,
            Content = "Test message",
            Photos = null
        };

        // Act
        var result = await _controller.PostMessageTexte(dto);

        // Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        
        var messageInDb = await _context.Messages
            .Include(m => m.MessageTexte)
            .FirstOrDefaultAsync(m => m.ConversationId == TEST_CONVERSATION_ID);
        
        Assert.IsNotNull(messageInDb);
        Assert.AreEqual("Test message", messageInDb.MessageTexte.Content);
        Assert.AreEqual(TEST_USER_1_ID, messageInDb.UtilisateurId);
    }

    [TestMethod]
    public async Task PostMessageTexte_ShouldReturnBadRequest_WhenConversationNotFound()
    {
        // Given
        await SeedBasicData();
        
        var dto = new MessageTextePostDTO
        {
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = 999,
            Content = "Test message"
        };

        // Act
        var result = await _controller.PostMessageTexte(dto);

        // Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task PostMessageTexte_ShouldCreateNotification_WhenMessageSent()
    {
        // Given
        await SeedBasicData();
        
        var dto = new MessageTextePostDTO
        {
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID,
            Content = "Test notification"
        };

        // Act
        await _controller.PostMessageTexte(dto);

        // Then
        _mockNotificationService.Verify(
            s => s.CreateNotification(It.Is<NotificationMessageCreateDTO>(
                n => n.UtilisateurId == TEST_USER_2_ID && 
                     n.TypeId == 1)),
            Times.Once);
        
        _mockMessageService.Verify(
            s => s.SendMessageCount(TEST_USER_2_ID),
            Times.Once);
    }

    #endregion

    #region PostMessageDemande Tests

    [TestMethod]
    public async Task PostMessageDemande_ShouldCreateDemande_WhenValidData()
    {
        // Given
        await SeedBasicData();
        
        // var demande = new Demande
        // {
        //     DemandeId = 1,
        //     UtilisateurId = TEST_USER_2_ID,
        //     AnnonceId = TEST_ANNONCE_ID
        // };
        //_context.Demandes.Add(demande);
        await _context.SaveChangesAsync();
        
        var dto = new MessageDemandePostDTO
        {
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID,
            //DemandeId = demande.DemandeId,
            PrixPropose = 45
        };

        // Act
        var result = await _controller.PostMessageDemande(dto);

        // Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        
        var messageInDb = await _context.Messages
            .Include(m => m.MessageDemande)
            .FirstOrDefaultAsync(m => m.MessageDemande != null);
        
        Assert.IsNotNull(messageInDb);
        Assert.AreEqual(45, messageInDb.MessageDemande.PrixPropose);
        Assert.IsFalse(messageInDb.MessageDemande.EstAcceptee);
        Assert.IsFalse(messageInDb.MessageDemande.EstRepondue);
    }

    [TestMethod]
    public async Task PostMessageDemande_ShouldCreateNotification_WhenDemandeSent()
    {
        // Given
        await SeedBasicData();
        
        // var demande = new Demande
        // {
        //     DemandeId = 1,
        //     UtilisateurId = TEST_USER_2_ID,
        //     AnnonceId = TEST_ANNONCE_ID
        // };
        //_context.Demandes.Add(demande);
        await _context.SaveChangesAsync();
        
        var dto = new MessageDemandePostDTO
        {
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID,
            //DemandeId = demande.DemandeId,
            PrixPropose = 45
        };

        // Act
        await _controller.PostMessageDemande(dto);

        // Then
        _mockNotificationService.Verify(
            s => s.CreateNotification(It.Is<NotificationPropositionCreateDTO>(
                n => n.UtilisateurId == TEST_USER_2_ID && 
                     n.TypeId == 6)),
            Times.Once);
    }

    #endregion

    #region PostMessagePayee Tests

    [TestMethod]
    public async Task PostMessagePayee_ShouldCreatePayment_WhenValidData()
    {
        // Given
        await SeedBasicData();
        
        var commande = new Commande
        {
            CommandeId = 1,
            ConversationId = TEST_CONVERSATION_ID,
            StatutCommandeId = 1
        };
        _context.Commandes.Add(commande);
        await _context.SaveChangesAsync();
        
        var dto = new MessageEstPayeePostDTO
        {
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID
        };

        // Act
        var result = await _controller.PostMessagePayee(dto);

        // Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        
        var messageInDb = await _context.Messages
            .Include(m => m.MessageEstPayee)
            .FirstOrDefaultAsync(m => m.MessageEstPayee != null);
        
        Assert.IsNotNull(messageInDb);
        Assert.AreEqual(commande.CommandeId, messageInDb.MessageEstPayee.CommandeId);
    }

    [TestMethod]
    public async Task PostMessagePayee_ShouldReturnBadRequest_WhenOrderNotFound()
    {
        // Given
        await SeedBasicData();
        
        var dto = new MessageEstPayeePostDTO
        {
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID
        };

        // Act
        var result = await _controller.PostMessagePayee(dto);

        // Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task PostMessagePayee_ShouldReturnBadRequest_WhenPaymentAlreadyExists()
    {
        // Given
        await SeedBasicData();
        
        var message = new Message
        {
            MessageId = 1,
            MessageDate = DateTime.UtcNow,
            MessageLu = false,
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID
        };
        
        var commande = new Commande
        {
            CommandeId = 1,
            ConversationId = TEST_CONVERSATION_ID,
            StatutCommandeId = 1
        };
        
        var messagePayee = new MessageEstPayee
        {
            MessageEstPayeeId = 1,
            MessageId = message.MessageId,
            CommandeId = commande.CommandeId,
            Message = message,
            Commande = commande
        };
        
        _context.Messages.Add(message);
        _context.Commandes.Add(commande);
        _context.MessageEstPayees.Add(messagePayee);
        await _context.SaveChangesAsync();
        
        var dto = new MessageEstPayeePostDTO
        {
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID
        };

        // Act
        var result = await _controller.PostMessagePayee(dto);

        // Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    #endregion

    #region MarkAsRead Tests

    [TestMethod]
    public async Task MarkAsRead_ShouldUpdateMessage_WhenMessageExists()
    {
        // Given
        await SeedBasicData();
        
        var message = new Message
        {
            MessageId = 1,
            MessageDate = DateTime.UtcNow,
            MessageLu = false,
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID
        };
        
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.MarkAsRead(message.MessageId);

        // Then
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        
        var updatedMessage = await _context.Messages.FindAsync(message.MessageId);
        Assert.IsTrue(updatedMessage.MessageLu);
    }

    [TestMethod]
    public async Task MarkAsRead_ShouldReturnNotFound_WhenMessageNotExists()
    {
        // Given
        await SeedBasicData();

        // Act
        var result = await _controller.MarkAsRead(999);

        // Then
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    #endregion

    #region AnswerPriceProposal Tests

    [TestMethod]
    public async Task AnswerPriceProposal_ShouldAcceptProposal_WhenAnswerIsTrue()
    {
        // Given
        await SeedBasicData();
        
        // var demande = new Demande
        // {
        //     DemandeId = 1,
        //     UtilisateurId = TEST_USER_2_ID,
        //     AnnonceId = TEST_ANNONCE_ID
        // };
        
        var message = new Message
        {
            MessageId = 1,
            MessageDate = DateTime.UtcNow,
            MessageLu = false,
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID
        };
        
        var messageDemande = new MessageDemande
        {
            MessageDemandeId = 1,
            MessageId = message.MessageId,
            //DemandeId = demande.DemandeId,
            PrixPropose = 45,
            EstAcceptee = false,
            EstRepondue = false,
            Message = message
        };
        
       // _context.Demandes.Add(demande);
        _context.Messages.Add(message);
        _context.MessageDemandes.Add(messageDemande);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.AnswerPriceProposal(message.MessageId, true);

        // Then
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        
        var updatedDemande = await _context.MessageDemandes.FindAsync(messageDemande.MessageDemandeId);
        Assert.IsTrue(updatedDemande.EstAcceptee);
        Assert.IsTrue(updatedDemande.EstRepondue);
        
        var updatedConversation = await _context.Conversations.FindAsync(TEST_CONVERSATION_ID);
        Assert.AreEqual(45, updatedConversation.Prix);
    }

    [TestMethod]
    public async Task AnswerPriceProposal_ShouldRejectProposal_WhenAnswerIsFalse()
    {
        // Given
        await SeedBasicData();
        
        
        var message = new Message
        {
            MessageId = 1,
            MessageDate = DateTime.UtcNow,
            MessageLu = false,
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID
        };
        
        var messageDemande = new MessageDemande
        {
            MessageDemandeId = 1,
            MessageId = message.MessageId,
            //DemandeId = demande.DemandeId,
            PrixPropose = 45,
            EstAcceptee = false,
            EstRepondue = false,
            Message = message
        };
        
        //_context.MessageDemandes.Add(demande);
        _context.Messages.Add(message);
        _context.MessageDemandes.Add(messageDemande);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.AnswerPriceProposal(message.MessageId, false);

        // Then
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        
        var updatedDemande = await _context.MessageDemandes.FindAsync(messageDemande.MessageDemandeId);
        Assert.IsFalse(updatedDemande.EstAcceptee);
        Assert.IsTrue(updatedDemande.EstRepondue);
    }

    #endregion

    #region AnnulePayement Tests

    [TestMethod]
    public async Task AnnulePayement_ShouldCancelPayment_WhenNotSent()
    {
        // Given
        await SeedBasicData();
        
        var message = new Message
        {
            MessageId = 1,
            MessageDate = DateTime.UtcNow,
            MessageLu = false,
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID,
            Conversation = _conversation
        };
        
        var commande = new Commande
        {
            CommandeId = 1,
            ConversationId = TEST_CONVERSATION_ID,
            StatutCommandeId = 1,
            Conversation = _conversation
        };
        
        var messagePayee = new MessageEstPayee
        {
            MessageEstPayeeId = 1,
            MessageId = message.MessageId,
            CommandeId = commande.CommandeId,
            EstEnvoye = false,
            EstAnnule = false,
            Message = message,
            Commande = commande
        };
        
        _context.Messages.Add(message);
        _context.Commandes.Add(commande);
        _context.MessageEstPayees.Add(messagePayee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.AnnulePayement(messagePayee.MessageEstPayeeId);

        // Then
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        
        var updatedPayment = await _context.MessageEstPayees.FindAsync(messagePayee.MessageEstPayeeId);
        Assert.IsTrue(updatedPayment.EstAnnule);
    }

    [TestMethod]
    public async Task AnnulePayement_ShouldReturnBadRequest_WhenAlreadySent()
    {
        // Given
        await SeedBasicData();
        
        var message = new Message
        {
            MessageId = 1,
            MessageDate = DateTime.UtcNow,
            MessageLu = false,
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID
        };
        
        var commande = new Commande
        {
            CommandeId = 1,
            ConversationId = TEST_CONVERSATION_ID,
            StatutCommandeId = 1
        };
        
        var messagePayee = new MessageEstPayee
        {
            MessageEstPayeeId = 1,
            MessageId = message.MessageId,
            CommandeId = commande.CommandeId,
            EstEnvoye = true,
            EstAnnule = false,
            Message = message
        };
        
        _context.Messages.Add(message);
        _context.Commandes.Add(commande);
        _context.MessageEstPayees.Add(messagePayee);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.AnnulePayement(messagePayee.MessageEstPayeeId);

        // Then
        Assert.IsInstanceOfType(result, typeof(BadRequestResult));
    }

    [TestMethod]
    public async Task AnnulePayement_ShouldReturnNotFound_WhenPaymentNotExists()
    {
        // Given
        await SeedBasicData();

        // Act
        var result = await _controller.AnnulePayement(999);

        // Then
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    #endregion

    #region DeleteTexteMessage Tests

    [TestMethod]
    public async Task DeleteTexteMessage_ShouldDeleteMessage_WhenExists()
    {
        // Given
        await SeedBasicData();
        
        var message = new Message
        {
            MessageId = 1,
            MessageDate = DateTime.UtcNow,
            MessageLu = false,
            UtilisateurId = TEST_USER_1_ID,
            ConversationId = TEST_CONVERSATION_ID
        };
        
        var messageTexte = new MessageTexte
        {
            MessageTexteId = 1,
            MessageId = message.MessageId,
            Content = "Test message",
            Message = message
        };
        
        message.MessageTexte = messageTexte;
        
        _context.Messages.Add(message);
        _context.MessageTextes.Add(messageTexte);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteTexteMessage(message.MessageId);

        // Then
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        
        var deletedMessage = await _context.Messages.FindAsync(message.MessageId);
        Assert.IsNull(deletedMessage);
    }

    [TestMethod]
    public async Task DeleteTexteMessage_ShouldReturnNotFound_WhenMessageNotExists()
    {
        // Given
        await SeedBasicData();

        // Act
        var result = await _controller.DeleteTexteMessage(999);

        // Then
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    #endregion

    [TestCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
    }
}