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
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared.DTO.Abonnement;
using Shared.DTO.Utilisateur;

namespace API.Tests.Controllers.integration;


[TestClass]
[TestSubject(typeof(AbonnementController))]
[TestCategory("integration")]
public class AbonnementControllerTest
{
    private Clothes2UDbContext _context;
    private AbonnementController _controller;
    private IMapper _mapper;
    private ICurrentUserService _currentUserService;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    
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
            cfg.AddProfile<ActivityMappingProfile>();
            cfg.AddProfile<UtilisateurMappingProfile>();
        });
        _mapper = config.CreateMapper();

        var Manager = new AbonnementManager(_context);

        var mockCurrentUserService = new Mock<ICurrentUserService>();
        mockCurrentUserService
            .Setup(s => s.GetUserIdOrThrow())
            .ReturnsAsync(TEST_USER_ID1);

        _controller = new AbonnementController(
            Manager,
            _mapper,
            mockCurrentUserService.Object
        );
        InitializeDefaultObjects();
    }
    private void CleanupDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }
    
    private Utilisateur _defaultUtilisateur1;
    private Utilisateur _defaultUtilisateur2;
    private RoleUtilisateur _defaultRole;
    private StatutUtilisateur _defaultStatut;
    private Abonnement _defaultAbonnement;
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

        _defaultUtilisateur1 = new Utilisateur
        {
            UtilisateurId = TEST_USER_ID1,
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

        _defaultUtilisateur2 = new Utilisateur
        {
            UtilisateurId = TEST_USER_ID2,
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
        _defaultAbonnement = new Abonnement
        {
            AbonnementId = 1,
            UtilisateurSuiveur = _defaultUtilisateur1,
            UtilisateurSuiveurId = _defaultUtilisateur1.UtilisateurId,
            UtilisateurSuivis = _defaultUtilisateur2,
            UtilisateurSuivisId = _defaultUtilisateur2.UtilisateurId,
        };

    }

    [TestMethod]
    public void ShouldGetAllAbonnement()
    {
        //Given
        _context.RolesUtilisateurs.Add(_defaultUtilisateur1.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur1.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        _context.RolesUtilisateurs.Add(_defaultUtilisateur2.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur2.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur2);
        _context.Abonnements.Add(_defaultAbonnement);
        _context.SaveChangesAsync();
        
        //Act
        ActionResult<IEnumerable<AbonnementDetailDTO>> action = _controller.GetAllUtilisateurSuiviByFollower(_defaultUtilisateur1.UtilisateurId).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<AbonnementDetailDTO>));
        var returnelements = okResult.Value as IEnumerable<AbonnementDetailDTO>;
        Assert.IsNotNull(returnelements);
        Assert.AreEqual(returnelements.Count(), 1);
        Assert.IsTrue(returnelements.Any(b => b.UtilisateurSuiviID == _defaultUtilisateur2.GetId()));
    }
    [TestMethod]
    public void ShouldGetAllAbonnes()
    {
        //Given
        _context.RolesUtilisateurs.Add(_defaultUtilisateur1.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur1.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        _context.RolesUtilisateurs.Add(_defaultUtilisateur2.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur2.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur2);
        _context.Abonnements.Add(_defaultAbonnement);
        _context.SaveChangesAsync();
        
        //Act
        ActionResult<IEnumerable<AbonnementDetailDTO>> action = _controller.GetAllFollowersByUtilisateurSuivi(_defaultUtilisateur2.UtilisateurId).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<AbonnementDetailDTO>));
        var returnelements = okResult.Value as IEnumerable<AbonnementDetailDTO>;
        Assert.IsNotNull(returnelements);
        Assert.AreEqual(returnelements.Count(), 1);
        Assert.IsTrue(returnelements.Any(b => b.UtilisateurSuiveurID == _defaultUtilisateur1.GetId()));
    }

    [TestMethod]
    public async Task ShouldAddAbonnement()
    {
        //Given
        _context.RolesUtilisateurs.Add(_defaultUtilisateur1.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur1.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        _context.RolesUtilisateurs.Add(_defaultUtilisateur2.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur2.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur2);
        _context.SaveChanges();
        
        //Act
        ActionResult<AbonnementDTO> action = _controller.Create(_defaultUtilisateur2.UtilisateurId).GetAwaiter().GetResult();  
    
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var objectResult = action.Result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.IsInstanceOfType(objectResult.Value, typeof(AbonnementDTO));
        var returnObject =  objectResult.Value as AbonnementDTO;
        var elementInDb = await _context.Abonnements
            .FirstOrDefaultAsync(a => a.UtilisateurSuiveurId == _defaultUtilisateur1.UtilisateurId 
                                      && a.UtilisateurSuivisId == _defaultUtilisateur2.UtilisateurId);
        Assert.IsNotNull(elementInDb);
        Assert.IsInstanceOfType(elementInDb, typeof(Abonnement));
        Assert.AreEqual(elementInDb.AbonnementId, returnObject.AbonnementId);
    }
    [TestMethod]
    public async Task ShouldAddAbonnementReturnBadRequestBecauseIdsAreTheSames()
    {
        //Given
        
        //Act
        ActionResult<AbonnementDTO> action = _controller.Create(TEST_USER_ID1).GetAwaiter().GetResult();  
    
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(BadRequestResult));
    }
    [TestMethod]
    public async Task ShouldAddAbonnementReturnBadRequestBecauseAbonnementAlreadyExists()
    {
        //Given
        _context.RolesUtilisateurs.Add(_defaultUtilisateur1.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur1.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        _context.RolesUtilisateurs.Add(_defaultUtilisateur2.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur2.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur2);
        _context.Abonnements.Add(_defaultAbonnement);
        _context.SaveChanges();
        //Act
        ActionResult<AbonnementDTO> action = _controller.Create(_defaultUtilisateur2.UtilisateurId).GetAwaiter().GetResult();  
    
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(BadRequestObjectResult));
    }
    [TestMethod]
    public async Task ShouldDeleteAbonnementReturnNotFound()
    {
        //Given
        int nonExistingAbonnement = 999;
        //Act
        IActionResult action = _controller.Delete(nonExistingAbonnement).GetAwaiter().GetResult();  
    
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    [TestMethod]
    public async Task ShouldDeleteAbonnement()
    {
        //Given
        _context.RolesUtilisateurs.Add(_defaultUtilisateur1.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur1.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        _context.RolesUtilisateurs.Add(_defaultUtilisateur2.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur2.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur2);
        _context.Abonnements.Add(_defaultAbonnement);
        _context.SaveChanges();
        //Act
        IActionResult action = _controller.Delete(_defaultUtilisateur2.UtilisateurId).GetAwaiter().GetResult();  
    
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
    }

    [TestMethod]
    public async Task ShouldGetAbonnementsByUserId()
    {
        //Given
        _context.RolesUtilisateurs.Add(_defaultUtilisateur1.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur1.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur1);
        _context.RolesUtilisateurs.Add(_defaultUtilisateur2.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur2.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur2);
        _context.Abonnements.Add(_defaultAbonnement);
        _context.SaveChangesAsync();
        
        //Act
        ActionResult<IEnumerable<UtilisateurCardDTO>> action = _controller.GetAbonnements().GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<UtilisateurCardDTO>));
        var returnelements = okResult.Value as IEnumerable<UtilisateurCardDTO>;
        Assert.IsNotNull(returnelements);
        Assert.AreEqual(returnelements.Count(), 1);
        Assert.IsTrue(returnelements.Any(b => b.UtilisateurId == _defaultUtilisateur2.GetId()));
    }
}