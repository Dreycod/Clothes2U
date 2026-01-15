using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using API.Services;
using API.Services.Interfaces;
using API.Services.VerificationSrvceV2;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared.DTO.Notification; 

namespace API.Tests.Controllers.integration;

[TestClass]
[TestSubject(typeof(NotificationController))]
[TestCategory("integration")]
public class NotificationControllerTest
{
    private Clothes2UDbContext _context;
    private ICurrentUserService _currentUserService;
    private NotificationController _controller;
    private IMapper _mapper;
    
    private const int TEST_USER_ID1 = 1;
    private const int TEST_USER_ID2 = 2;
    
    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        _context = new Clothes2UDbContext(builder.Options);

        CleanupDatabase();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<NotificationMappingProfile>();
            cfg.AddProfile<AnnonceMappingProfile>();
        });
        _mapper = config.CreateMapper();

        var Manager = new NotificationManager(_context);
        NotificationAvertissementManager notificationAvertissementManager =
            new NotificationAvertissementManager(_context);
        FavorisManager favorisManager = new FavorisManager(_context);
        UtilisateurManager utilisateurManager = new UtilisateurManager(_context);
        NotificationCommercialManager notificationCommercialManager =
            new NotificationCommercialManager(_context);
        NotificationPropositionManager notificationPropositionManager =
            new NotificationPropositionManager(_context);
        NotificationMessageManager notificationMessageManager =
            new NotificationMessageManager(_context);
        NotificationNouvelleAnnonceManager notificationNouvelleAnnonceManager =
            new NotificationNouvelleAnnonceManager(_context);
        NotificationModificationAnnonceManager notificationModificationAnnonceManager =
            new NotificationModificationAnnonceManager(_context);
        NotificationAchatManager notificationAchatManager =
            new NotificationAchatManager(_context);
        
        AbonnementManager abonnementManager = new AbonnementManager(_context);
        

        var mockCurrentUserService = new Mock<ICurrentUserService>();
        mockCurrentUserService
            .Setup(s => s.GetUserIdOrThrow())
            .ReturnsAsync(TEST_USER_ID1);
        
        var mockNotificationMailService = new Mock<INotificationMailService>();
        var notificationHubService = new Mock<INotificationHubService>();
        

        NotificationService notificationService = new NotificationService(
            _mapper,
            Manager,
            favorisManager,
            utilisateurManager,
            notificationAvertissementManager,
            notificationCommercialManager,
            notificationPropositionManager,
            notificationMessageManager,
            notificationNouvelleAnnonceManager,
            notificationModificationAnnonceManager,
            notificationAchatManager,
            mockNotificationMailService.Object,
            abonnementManager,
            mockCurrentUserService.Object,
            notificationHubService.Object
        );

        _controller = new NotificationController(
            Manager,
            _mapper,
            mockCurrentUserService.Object,
            notificationService
        );
        InitializeDefaultObjects();
    }
    
    private void CleanupDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    // Types de notification
    private NotificationType _notificationTypeAvertissement;
    private NotificationType _notificationTypeNouvelleAnnonce;
    private NotificationType _notificationTypeMessage;
    
    // Utilisateurs
    private Utilisateur _defaultUtilisateur1;
    private Utilisateur _defaultUtilisateur2;
    private RoleUtilisateur _defaultRole;
    private StatutUtilisateur _defaultStatut;
    
    // Notifications parentes
    private Notification _notificationAvertissementParent;
    private Notification _notificationNouvelleAnnonceParent;
    private Notification _notificationMessageParent;
    
    // Notifications spécifiques
    private NotificationAvertissement _notificationAvertissement;
    private NotificationNouvelleAnnonce _notificationNouvelleAnnonce;
    private NotificationMessage _notificationMessage;
    
    // Entités liées pour les annonces
    private Annonce _defaultAnnonce;
    private Categorie _defaultCategorie;
    private SousCategorie _defaultSousCategorie;
    private Taille _defaultTaille;
    private EtatArticle _defaultEtat;
    private Marque _defaultMarque;
    private Genre _defaultGenre;
    private StatutAnnonce _defaultStatutAnnonce;
    
    private void InitializeDefaultObjects()
    {
        // Types de notification
        _notificationTypeAvertissement = new NotificationType
        {
            NotificationTypeId = 1,
            LibelleType = "Avertissement"
        };
        
        _notificationTypeNouvelleAnnonce = new NotificationType
        {
            NotificationTypeId = 2,
            LibelleType = "Nouvelle Annonce"
        };
        
        _notificationTypeMessage = new NotificationType
        {
            NotificationTypeId = 3,
            LibelleType = "Message"
        };
        
        // Rôles et statuts
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

        // Utilisateurs
        _defaultUtilisateur1 = new Utilisateur
        {
            UtilisateurId = TEST_USER_ID1,
            Login = "User1",
            Email = "user1@example.com",
            Password = "pwd123",
            Description = "Utilisateur de test 1",
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
        
        _defaultUtilisateur2 = new Utilisateur
        {
            UtilisateurId = TEST_USER_ID2,
            Login = "User2",
            Email = "user2@example.com",
            Password = "pwd456",
            Description = "Utilisateur de test 2",
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
        
        // Entités pour les annonces
        _defaultCategorie = new Categorie
        {
            CategorieId = 1,
            LibelleCategorie = "Vêtements"
        };

        _defaultSousCategorie = new SousCategorie
        {
            SousCategorieId = 1,
            LibelleSousCategorie = "T-shirts"
        };

        _defaultTaille = new Taille
        {
            TailleId = 1,
            Libelletaille = "M"
        };

        _defaultEtat = new EtatArticle
        {
            EtatArticleId = 1,
            NomEtat = "Neuf"
        };

        _defaultMarque = new Marque
        {
            MarqueId = 1,
            NomMarque = "Nike"
        };

        _defaultGenre = new Genre
        {
            GenreId = 1,
            NomGenre = "Homme"
        };

        _defaultStatutAnnonce = new StatutAnnonce
        {
            StatutAnnonceId = 1,
            StatutLibelle = "En Ligne"
        };
        
        _defaultAnnonce = new Annonce
        {
            AnnonceId = 1,
            Title = "Annonce Test",
            Description = "Description test",
            Prix = 50,
            UtilisateurId = TEST_USER_ID2,
            Utilisateur = _defaultUtilisateur2,
            CategorieId = _defaultCategorie.CategorieId,
            Categorie = _defaultCategorie,
            SousCategorieId = _defaultSousCategorie.SousCategorieId,
            SousCategorie = _defaultSousCategorie,
            TailleId = _defaultTaille.TailleId,
            Taille = _defaultTaille,
            EtatId = _defaultEtat.EtatArticleId,
            Etat = _defaultEtat,
            MarqueId = _defaultMarque.MarqueId,
            Marque = _defaultMarque,
            GenreId = _defaultGenre.GenreId,
            GenreAnnonce = _defaultGenre,
            StatutAnnonceId = _defaultStatutAnnonce.StatutAnnonceId,
            Statut = _defaultStatutAnnonce,
            DateAnnonce = DateTime.Now,
            Negociable = true
        };
        
        // Notifications parentes
        _notificationAvertissementParent = new Notification
        {
            NotificationId = 1,
            DateCreation = DateTime.UtcNow,
            EstLu = false,
            NotificationTypeId = _notificationTypeAvertissement.NotificationTypeId,
            NotificationType = _notificationTypeAvertissement,
            UtilisateurId = TEST_USER_ID1,
            Utilisateur = _defaultUtilisateur1
        };
        
        _notificationNouvelleAnnonceParent = new Notification
        {
            NotificationId = 2,
            DateCreation = DateTime.UtcNow,
            EstLu = false,
            NotificationTypeId = _notificationTypeNouvelleAnnonce.NotificationTypeId,
            NotificationType = _notificationTypeNouvelleAnnonce,
            UtilisateurId = TEST_USER_ID1,
            Utilisateur = _defaultUtilisateur1
        };
        
        _notificationMessageParent = new Notification
        {
            NotificationId = 3,
            DateCreation = DateTime.UtcNow,
            EstLu = false,
            NotificationTypeId = _notificationTypeMessage.NotificationTypeId,
            NotificationType = _notificationTypeMessage,
            UtilisateurId = TEST_USER_ID1,
            Utilisateur = _defaultUtilisateur1
        };
        
        // Notifications spécifiques
        _notificationAvertissement = new NotificationAvertissement
        {
            NotificationAvertissementId = 1,
            MessageAvertissement = "Ceci est un avertissement",
            NotificationId = _notificationAvertissementParent.NotificationId,
            LaNotification = _notificationAvertissementParent
        };
        
        _notificationNouvelleAnnonce = new NotificationNouvelleAnnonce
        {
            NotificationNouvelleAnnonceId = 1,
            AnnonceId = _defaultAnnonce.AnnonceId,
            Annonce = _defaultAnnonce,
            NotificationId = _notificationNouvelleAnnonceParent.NotificationId,
            LaNotification = _notificationNouvelleAnnonceParent
        };
        
        _notificationMessage = new NotificationMessage
        {
            NotificationMessageId = 1,
            MessageId = 1,
            NotificationId = _notificationMessageParent.NotificationId,
            LaNotification = _notificationMessageParent
        };
    }

    private async Task SeedBasicData()
    {
        _context.RolesUtilisateurs.Add(_defaultRole);
        _context.StatutUtilisateurs.Add(_defaultStatut);
        _context.NotificationTypes.AddRange(
            _notificationTypeAvertissement,
            _notificationTypeNouvelleAnnonce,
            _notificationTypeMessage
        );
        
        _context.Utilisateurs.AddRange(_defaultUtilisateur1, _defaultUtilisateur2);
        
        await _context.SaveChangesAsync();
    }
    
    private async Task SeedNotificationAvertissement()
    {
        await SeedBasicData();
        
        _context.Notifications.Add(_notificationAvertissementParent);
        await _context.SaveChangesAsync();
        
        _context.NotificationAvertissements.Add(_notificationAvertissement);
        await _context.SaveChangesAsync();
    }
    
    private async Task SeedNotificationNouvelleAnnonce()
    {
        await SeedBasicData();
        _context.Categories.Add(_defaultCategorie);
        _context.SousCategories.Add(_defaultSousCategorie);
        _context.Tailles.Add(_defaultTaille);
        _context.EtatArticles.Add(_defaultEtat);
        _context.Marques.Add(_defaultMarque);
        _context.Genres.Add(_defaultGenre);
        _context.StatutAnnonces.Add(_defaultStatutAnnonce);
        await _context.SaveChangesAsync();
        
        _context.Annonces.Add(_defaultAnnonce);
        await _context.SaveChangesAsync();
        
        _context.Notifications.Add(_notificationNouvelleAnnonceParent);
        await _context.SaveChangesAsync();
        
        _context.NotificationNouvelleAnnonces.Add(_notificationNouvelleAnnonce);
        await _context.SaveChangesAsync();
    }
    [TestMethod]
    public async Task ShouldGetUserNotifications()
    {
        // Given
        await SeedNotificationAvertissement();
        var notification2 = new Notification
        {
            NotificationId = 2,
            DateCreation = DateTime.UtcNow.AddHours(-1),
            EstLu = false,
            NotificationTypeId = _notificationTypeMessage.NotificationTypeId,
            NotificationType = _notificationTypeMessage,
            UtilisateurId = TEST_USER_ID1,
            Utilisateur = _defaultUtilisateur1
        };
        
        _context.Notifications.Add(notification2);
        await _context.SaveChangesAsync();
        var notification3 = new Notification
        {
            NotificationId = 3,
            DateCreation = DateTime.UtcNow,
            EstLu = false,
            NotificationTypeId = _notificationTypeAvertissement.NotificationTypeId,
            NotificationType = _notificationTypeAvertissement,
            UtilisateurId = TEST_USER_ID2,
            Utilisateur = _defaultUtilisateur2
        };
        
        _context.Notifications.Add(notification3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetUserNotifications();

        // Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var notifications = okResult.Value as IEnumerable<NotificationDTO>;
        Assert.IsNotNull(notifications);
        Assert.AreEqual(2, notifications.Count());
        var notificationsInDb = await _context.Notifications
            .AsNoTracking()
            .Where(n => n.UtilisateurId == TEST_USER_ID1)
            .ToListAsync();
        Assert.AreEqual(2, notificationsInDb.Count);
        Assert.IsTrue(notificationsInDb.All(n => n.EstLu));
    }

    [TestMethod]
    public async Task ShouldGetUserNotificationsReturnEmptyWhenNoNotifications()
    {
        // Given
        await SeedBasicData();

        // Act
        var result = await _controller.GetUserNotifications();

        // Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var notifications = okResult.Value as IEnumerable<NotificationDTO>;
        Assert.IsNotNull(notifications);
        Assert.AreEqual(0, notifications.Count());
    }

    [TestMethod]
    public async Task ShouldDeleteNotification()
    {
        //Given
        await SeedNotificationAvertissement();
        var notification2 = new Notification
        {
            NotificationId = 2,
            DateCreation = DateTime.UtcNow.AddHours(-1),
            EstLu = false,
            NotificationTypeId = _notificationTypeMessage.NotificationTypeId,
            NotificationType = _notificationTypeMessage,
            UtilisateurId = TEST_USER_ID1,
            Utilisateur = _defaultUtilisateur1
        };
        
        _context.Notifications.Add(notification2);
        await _context.SaveChangesAsync();
        
        //Act
        var action = await _controller.DeleteNotification(notification2.NotificationId);
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        var elementInDb = _context.Notifications.Find(notification2.NotificationId);
        Assert.IsNull(elementInDb);
    }
    [TestMethod]
    public async Task ShouldDeleteNotificationReturnNotFound()
    {
        //Given
        int nonExistentId = 999;
        
        
        //Act
        var action = await _controller.DeleteNotification(nonExistentId);
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    [TestMethod]
    public async Task ShouldGetNotificationById()
    {
        //Given
        await SeedNotificationAvertissement(); 
    
        //Act
        ActionResult<NotificationDTO> action = await _controller.GetById(_notificationAvertissementParent.NotificationId);
    
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(NotificationAvertissementDTO));
        var returnElement = okResult.Value as NotificationAvertissementDTO;
        Assert.IsNotNull(returnElement);
        Assert.AreEqual(_notificationAvertissementParent.NotificationId, returnElement.NotificationId);
        Assert.AreEqual("Ceci est un avertissement", returnElement.MessageAvertissement);
    }
    [TestMethod]
    public async Task ShouldGetNotificationReturnNotFound()
    {
        //Given
        int nonExistentId = 999;
        
        
        //Act
        var action = await _controller.GetById(nonExistentId);
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
    }
}