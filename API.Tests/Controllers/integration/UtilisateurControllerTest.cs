using System;
using System.Collections.Generic;
using System.Linq;
using API.Controllers;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using API.Services;
using API.Services.VerificationSrvceV2;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared;
using Shared.DTO;
using Shared.DTO.Utilisateur;

namespace API.Tests.Controllers.integration;

[TestClass]
[TestSubject(typeof(UtilisateurController))]
[TestCategory("integration")]
public class UtilisateurControllerTest
{
    private Clothes2UDbContext _context;
    private UtilisateurController _controller;
    private IMapper _mapper;
    private ICurrentUserService _currentUserService;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    
    private const int TEST_USER_ID1 = 1;
    private const int TEST_USER_ID2 = 2;
    private Utilisateur _defaultUtilisateur1;
    
    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        _context = new Clothes2UDbContext(builder.Options);

        CleanupDatabase();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UtilisateurMappingProfile>();
            cfg.AddProfile<AnnonceMappingProfile>();
        });
        _mapper = config.CreateMapper();

        var manager = new UtilisateurManager(_context);
        MessageManager messageManager = new MessageManager(_context);
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        NotificationManager notificationManager = new NotificationManager(_context);
        UserDeletionService userDeletionService = new UserDeletionService(_context);
        AnnonceManager annonceManager = new AnnonceManager(_context);
        FavorisManager favorisManager = new FavorisManager(_context);
        IAnnonceExtensionService annonceExtensionService = new AnnonceExtensionService(mockCurrentUserService.Object, favorisManager, _mapper,annonceManager);
        mockCurrentUserService
            .Setup(s => s.GetUserIdOrThrow())
            .ReturnsAsync(TEST_USER_ID1);
        mockCurrentUserService
            .Setup(s => s.GetUserId())
            .ReturnsAsync(TEST_USER_ID1);
        mockCurrentUserService
            .Setup(s => s.IsFollowedByCurrentUser(It.IsAny<int>()))
            .ReturnsAsync(false);
        mockCurrentUserService
            .Setup(s => s.IsBlockedByCurrentUser(It.IsAny<int>()))
            .ReturnsAsync(false);

        var mockSuggestionService = new Mock<ISuggestionService>();
        var mockAnnonceExtensionService = new Mock<IAnnonceExtensionService>();
        _controller = new UtilisateurController(
            manager,
            mockCurrentUserService.Object,
            _mapper,
            notificationManager,
            messageManager,
            userDeletionService,
            annonceManager,
            annonceExtensionService
        );

        // Initialiser les objets par défaut
        InitializeDefaultObjects();
    }
    private void CleanupDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }
    private void InitializeDefaultObjects()
    {
        var role = new RoleUtilisateur
        {
            RoleUtilisateurId = 1,
            RoleUtilisateurLibelle = "Utilisateur"
        };

        var statut = new StatutUtilisateur
        {
            StatutUtilisateurId = 1,
            StatutLibelle = "Actif"
        };
        _defaultUtilisateur1 = new Utilisateur
        {
            UtilisateurId = TEST_USER_ID1,
            Login = "TestUser1",
            Email = "test@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("pwd"),
            Description = "desc",
            Dateinscription = DateTime.Now,
            ValidEmail = false,
            ValidTelephone = false,
            PreferenceCookies = false,
            PreferenceTheme = false,
            PreferenceNotifMail = false,
            RoleId = role.RoleUtilisateurId,
            StatutId = statut.StatutUtilisateurId,
            Role = role,
            Statut = statut,
            Adresses = new List<Adresse>
            {
                new Adresse
                {
                    AdresseId = 1,
                    AdresseRue = "123 Rue de la Paix",
                    AdresseVille = "Paris",
                    AdresseCodePostal = "750001",
                    AdressePays = "France",
                    IsDefault = true,
                    UtilisateurId = TEST_USER_ID1
                },
                new Adresse
                {
                    AdresseId = 2,
                    AdresseRue = "456 Avenue des Champs",
                    AdresseVille = "Lyon",
                    AdresseCodePostal = "690001",
                    AdressePays = "France",
                    IsDefault = false,
                    UtilisateurId = TEST_USER_ID1
                }
            }
        };

    }

    [TestMethod]
    public void ShouldGetUtilisateurById()
    {
        //Given
        _context.RolesUtilisateurs.Add(_defaultUtilisateur1.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur1.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        _context.SaveChanges();
        
        int utilisateurId = _defaultUtilisateur1.UtilisateurId;
        //Act
        var action = _controller.GetUtilisateur(utilisateurId).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(UtilisateurViewDTO));
        var returnElement = okResult.Value as UtilisateurViewDTO;
        Assert.IsNotNull(returnElement);
        Assert.AreEqual(_defaultUtilisateur1.GetId(), returnElement.UtilisateurId);
    }

    [TestMethod]
    public void ShouldGetUtilisateurReturnNotFound()
    {
        //Given : 
        int nonExistentId = 999;
        
        //When : 
        ActionResult<UtilisateurViewDTO> action = _controller.GetUtilisateur(nonExistentId).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
    }
    [TestMethod]
    public void ShouldGetUtilisateurByLogin()
    {
        //Given
        _context.RolesUtilisateurs.Add(_defaultUtilisateur1.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur1.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        _context.SaveChanges();
        //Act
        var action = _controller.GetByLogin(_defaultUtilisateur1.Login).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(UtilisateurViewDTO));
        var returnElement = okResult.Value as UtilisateurViewDTO;
        Assert.IsNotNull(returnElement);
        Assert.AreEqual(_defaultUtilisateur1.GetId(), returnElement.UtilisateurId);
    }
    [TestMethod]
    public void ShouldGetUtilisateurByLoginReturnNotFound()
    {
        //Given : 
        string nonExistentLogin = "";
        //When : 
        ActionResult<UtilisateurViewDTO> action = _controller.GetByLogin(nonExistentLogin).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldDeleteUtilisateurReturnNotFound()
    {
        //Given : 
        AccountDeletionDTO accountDeletionDTO = new AccountDeletionDTO()
        {
            Password = "123"
        };
        //When : 
        IActionResult action = _controller.SuppressionCompte(accountDeletionDTO).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    [TestMethod]
    public void ShouldDeleteUtilisateurReturnUnauthorized()
    {
        //Given : 
        _context.RolesUtilisateurs.Add(_defaultUtilisateur1.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur1.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        _context.SaveChanges();
        AccountDeletionDTO accountDeletionDTO = new AccountDeletionDTO()
        {
            Password = "123"
        };
        //When : 
        IActionResult action = _controller.SuppressionCompte(accountDeletionDTO).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(UnauthorizedObjectResult));
    }
    /*
    [TestMethod]
    public void ShouldDeleteUtilisateur()
    {
        //Given : 
        _context.RolesUtilisateurs.Add(_defaultUtilisateur1.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur1.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        
        _context.SaveChanges();
        AccountDeletionDTO accountDeletionDTO = new AccountDeletionDTO()
        {
            Password = "pwd"
        };
        _moc
            .Setup(s => s.GetAnnoncesByUserId(_defaultUtilisateur1.UtilisateurId))
            .ReturnsAsync(new List<Annonce>()); // ou vos annonces de test
    
        // Mock du service de suppression
        _mockUserDeletionService
            .Setup(s => s.DeleteUtilisateurAsync(_defaultUtilisateur1.UtilisateurId))
            .Returns(Task.CompletedTask);
        //When : 
        IActionResult action = _controller.SuppressionCompte(accountDeletionDTO).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(OkObjectResult));
        var okResult = action as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var apiResponse = okResult.Value as APIResponse<object>;
        Assert.IsNotNull(apiResponse);
        Assert.IsTrue(apiResponse.Success);
    }*/
    

    [TestMethod]
    public void ShouldUpdateNotifMailPreference()
    {
        //Given
        _defaultUtilisateur1.ValidEmail = true;
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        _context.SaveChanges();
        UpdateNotifMailDTO updateNotifMail = new UpdateNotifMailDTO()
        {
            PreferenceNotifMail = true
        };
        //Act
        IActionResult action = _controller.UpdateNotifMailPreference(updateNotifMail).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof (NoContentResult));
        Utilisateur user = _context.Utilisateurs.FirstOrDefaultAsync(u => u.UtilisateurId == _defaultUtilisateur1.UtilisateurId).Result;
        Assert.IsTrue(user.ValidEmail);
    }
    [TestMethod]
    public void ShouldNotUpdateEmailPreferenceBecauseEmailIsNotValid()
    {
        //Given
        _defaultUtilisateur1.ValidEmail = false;
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        _context.SaveChanges();
        UpdateNotifMailDTO updateNotifMail = new UpdateNotifMailDTO()
        {
            PreferenceNotifMail = true
        };
        //Act
        IActionResult action = _controller.UpdateNotifMailPreference(updateNotifMail).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof (BadRequestObjectResult));
    }
    [TestMethod]
    public void ShouldGetSettingsReturnNotFound()
    {
        //Given
        int nonExistentId = 999;
        
        //Act
        ActionResult<UtilisateurSettingsDTO> action = _controller.GetUtilisateurSettings().GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetSettingsByUserId()
    {
        //Given
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        _context.SaveChanges();
        
        //Act
        ActionResult<UtilisateurSettingsDTO> action = _controller.GetUtilisateurSettings().GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(UtilisateurSettingsDTO));
        var returnElement = okResult.Value as UtilisateurSettingsDTO;
        Assert.IsNotNull(returnElement);
        Assert.AreEqual(_defaultUtilisateur1.UtilisateurId, returnElement.UtilisateurId);
        Assert.AreEqual(_defaultUtilisateur1.Login, returnElement.Login);
    }
    [TestMethod]
    public void ShouldUpdateUtilisateurReturnForbid()
    {
        //Given
        UtilisateurPutDTO utilisateurPutDTO = new UtilisateurPutDTO()
        {
            UtilisateurId = 2,
        };
        
        //Act
        IActionResult action = _controller.PutUtilisateur(utilisateurPutDTO).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(ForbidResult));
    }
    [TestMethod]
    public void ShouldUpdateUtilisateur()
    {
        //Given
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        _context.SaveChanges();
    
        UtilisateurPutDTO utilisateurPutDTO = new UtilisateurPutDTO()
        {
            UtilisateurId = _defaultUtilisateur1.UtilisateurId,
            Login = "updatedLogin",
        };
    
        //Act
        IActionResult action = _controller.PutUtilisateur(utilisateurPutDTO).GetAwaiter().GetResult();
    
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
    }
}