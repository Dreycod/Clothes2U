using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared.DTO.Bloque;

namespace API.Tests.Controllers.integration;

[TestClass]
[TestSubject(typeof(BloqueController))]
[TestCategory("integration")]
public class BloqueControllerTest
{
    private Clothes2UDbContext _context;
    private BloqueController _controller;
    private IMapper _mapper;
    private ICurrentUserService _currentUserService;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    
    private const int TEST_USER_BLOQUEUR_ID = 1;
    private const int TEST_USER_BLOQUE_ID = 2;
    private const int TEST_USER_AUTRE_ID = 3;
    
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
            cfg.AddProfile<ActivityMappingProfile>();
        });
        _mapper = config.CreateMapper();

        var bloqueManager = new BloqueManager(_context);
        AbonnementManager _abonnementManager = new AbonnementManager(_context);
        var mockCurrentUserService = new Mock<ICurrentUserService>();
        mockCurrentUserService
            .Setup(s => s.GetUserIdOrThrow())
            .ReturnsAsync(TEST_USER_BLOQUEUR_ID);

        _controller = new BloqueController(
            bloqueManager,
            mockCurrentUserService.Object,
            _abonnementManager,
            _mapper
        );
        InitializeDefaultObjects();
    }
    private void CleanupDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }
    private Utilisateur _userBloqueur;
    private Utilisateur _userBloque;
    private Utilisateur _userAutre;
    private RoleUtilisateur _defaultRole;
    private StatutUtilisateur _defaultStatut;

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

        _userBloqueur = new Utilisateur
        {
            UtilisateurId = TEST_USER_BLOQUEUR_ID,
            Login = "UserBloqueur",
            Email = "bloqueur@example.com",
            Password = "pwd123",
            Description = "Utilisateur qui bloque",
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

        _userBloque = new Utilisateur
        {
            UtilisateurId = TEST_USER_BLOQUE_ID,
            Login = "UserBloque",
            Email = "bloque@example.com",
            Password = "pwd456",
            Description = "Utilisateur bloqué",
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

        _userAutre = new Utilisateur
        {
            UtilisateurId = TEST_USER_AUTRE_ID,
            Login = "UserAutre",
            Email = "autre@example.com",
            Password = "pwd789",
            Description = "Autre utilisateur",
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
    }

    private async Task SeedBasicUsers()
    {
        _context.RolesUtilisateurs.Add(_defaultRole);
        _context.StatutUtilisateurs.Add(_defaultStatut);
        _context.Utilisateurs.AddRange(_userBloqueur, _userBloque, _userAutre);
        await _context.SaveChangesAsync();
    }
    [TestMethod]
    public async Task ShouldCreateBloque()
    {
        // Given
        await SeedBasicUsers();
        int utilisateurBloqueId = TEST_USER_BLOQUE_ID;

        // Act
        var action = await _controller.Create(utilisateurBloqueId);

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(BloqueDTO));
        
        var bloqueDto = okResult.Value as BloqueDTO;
        Assert.AreEqual(TEST_USER_BLOQUEUR_ID, bloqueDto.BloqueurId);
        Assert.AreEqual(TEST_USER_BLOQUE_ID, bloqueDto.UtilisateurBloqueId);
    }
    [TestMethod]
    public async Task ShouldCreateBloqueAndDeleteAbonnement()
    {
        // Given
        await SeedBasicUsers();
        Abonnement abonnement = new Abonnement()
        {
            AbonnementId = 1,
            UtilisateurSuiveur = _userBloqueur,
            UtilisateurSuiveurId = _userBloqueur.UtilisateurId,
            UtilisateurSuivis = _userBloque,
            UtilisateurSuivisId = _userBloque.UtilisateurId
        };
        _context.Abonnements.Add(abonnement);
        await _context.SaveChangesAsync();
        _context.Entry(abonnement).State = EntityState.Detached;

        int utilisateurBloqueId = TEST_USER_BLOQUE_ID;

        // Act
        var action = await _controller.Create(utilisateurBloqueId);

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(BloqueDTO));
        
        var bloqueDto = okResult.Value as BloqueDTO;
        Assert.AreEqual(TEST_USER_BLOQUEUR_ID, bloqueDto.BloqueurId);
        Assert.AreEqual(TEST_USER_BLOQUE_ID, bloqueDto.UtilisateurBloqueId);
        var abonnementInDb = await _context.Abonnements.FirstOrDefaultAsync(a => a.AbonnementId == 1);
        Assert.IsNull(abonnementInDb);
    }

    [TestMethod]
    public async Task ShouldCreateBloqueReturnBadRequestWhenAlreadyBlocked()
    {
        // Given
        await SeedBasicUsers();
        
        var bloqueExistant = new Bloque
        {
            BloqueId = 1,
            UtilisateurBloqueurId = TEST_USER_BLOQUEUR_ID,
            UtilisateurBloqueId = TEST_USER_BLOQUE_ID,
            UtilisateurBloqueur = _userBloqueur,
            UtilisateurBloque = _userBloque
        };
        
        _context.Bloques.Add(bloqueExistant);
        await _context.SaveChangesAsync();

        // Act
        var action = await _controller.Create(TEST_USER_BLOQUE_ID);

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(BadRequestObjectResult));
        var badRequest = action.Result as BadRequestObjectResult;
        Assert.AreEqual("Cet utilisateur est déjà bloqué.", badRequest.Value);
    }

    [TestMethod]
    public async Task ShouldGetByUtilisateurBloqueur()
    {
        // Given
        await SeedBasicUsers();
        
        var bloque1 = new Bloque
        {
            BloqueId = 1,
            UtilisateurBloqueurId = TEST_USER_BLOQUEUR_ID,
            UtilisateurBloqueId = TEST_USER_BLOQUE_ID,
            UtilisateurBloqueur = _userBloqueur,
            UtilisateurBloque = _userBloque
        };
        
        var bloque2 = new Bloque
        {
            BloqueId = 2,
            UtilisateurBloqueurId = TEST_USER_BLOQUEUR_ID,
            UtilisateurBloqueId = TEST_USER_AUTRE_ID,
            UtilisateurBloqueur = _userBloqueur,
            UtilisateurBloque = _userAutre
        };
        
        _context.Bloques.AddRange(bloque1, bloque2);
        await _context.SaveChangesAsync();

        // Act
        var action = await _controller.GetByUtilisateurBloqueur(TEST_USER_BLOQUEUR_ID);

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        
        var bloques = okResult.Value as IEnumerable<BloqueDetailDTO>;
        Assert.IsNotNull(bloques);
        Assert.AreEqual(2, bloques.Count());
        Assert.IsTrue(bloques.All(b => b.BloqueurId == TEST_USER_BLOQUEUR_ID));
    }

    [TestMethod]
    public async Task ShouldGetByUtilisateurBloque()
    {
        // Given
        await SeedBasicUsers();
        
        var bloque1 = new Bloque
        {
            BloqueId = 1,
            UtilisateurBloqueurId = TEST_USER_BLOQUEUR_ID,
            UtilisateurBloqueId = TEST_USER_BLOQUE_ID,
            UtilisateurBloqueur = _userBloqueur,
            UtilisateurBloque = _userBloque
        };
        
        var bloque2 = new Bloque
        {
            BloqueId = 2,
            UtilisateurBloqueurId = TEST_USER_AUTRE_ID,
            UtilisateurBloqueId = TEST_USER_BLOQUE_ID,
            UtilisateurBloqueur = _userAutre,
            UtilisateurBloque = _userBloque
        };
        
        _context.Bloques.AddRange(bloque1, bloque2);
        await _context.SaveChangesAsync();

        // Act
        var action = await _controller.GetByUtilisateurBloque(TEST_USER_BLOQUE_ID);

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        
        var bloques = okResult.Value as IEnumerable<BloqueDetailDTO>;
        Assert.IsNotNull(bloques);
        Assert.AreEqual(2, bloques.Count());
        Assert.IsTrue(bloques.All(b => b.UtilisateurBloqueId == TEST_USER_BLOQUE_ID));
    }

    [TestMethod]
    public async Task ShouldDeleteBloque()
    {
        // Given
        await SeedBasicUsers();
        
        var bloque = new Bloque
        {
            BloqueId = 1,
            UtilisateurBloqueurId = TEST_USER_BLOQUEUR_ID,
            UtilisateurBloqueId = TEST_USER_BLOQUE_ID,
            UtilisateurBloqueur = _userBloqueur,
            UtilisateurBloque = _userBloque
        };
        
        _context.Bloques.Add(bloque);
        await _context.SaveChangesAsync();

        // Act
        var action = await _controller.Delete(TEST_USER_BLOQUE_ID);

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        
        var bloqueDeleted = await _context.Bloques.FindAsync(bloque.BloqueId);
        Assert.IsNull(bloqueDeleted);
    }

    [TestMethod]
    public async Task ShouldDeleteBloqueReturnNotFoundWhenNotExists()
    {
        // Given
        await SeedBasicUsers();
        int nonExistentUserId = 999;

        // Act
        var action = await _controller.Delete(nonExistentUserId);

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task ShouldGetByUtilisateurBloqueurReturnEmptyWhenNoBlocks()
    {
        // Given
        await SeedBasicUsers();

        // Act
        var action = await _controller.GetByUtilisateurBloqueur(TEST_USER_BLOQUEUR_ID);

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        var bloques = okResult.Value as IEnumerable<BloqueDetailDTO>;
        Assert.IsNotNull(bloques);
        Assert.AreEqual(0, bloques.Count());
    }

    [TestMethod]
    public async Task ShouldGetByUtilisateurBloqueReturnEmptyWhenNotBlocked()
    {
        // Given
        await SeedBasicUsers();

        // Act
        var action = await _controller.GetByUtilisateurBloque(TEST_USER_BLOQUE_ID);

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        var bloques = okResult.Value as IEnumerable<BloqueDetailDTO>;
        Assert.IsNotNull(bloques);
        Assert.AreEqual(0, bloques.Count());
    }
}