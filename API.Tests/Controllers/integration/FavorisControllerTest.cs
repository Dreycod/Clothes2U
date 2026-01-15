using System;
using System.Collections.Generic;
using System.Linq;
using API.Controllers;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using API.Services;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Favoris;

namespace API.Tests.Controllers.integration;

[TestClass]
[TestSubject(typeof(FavorisController))]
[TestCategory("integration")]
public class FavorisControllerTest
{
    private Clothes2UDbContext _context;
    private FavorisController _controller;
    private IMapper _mapper;
    private ICurrentUserService _currentUserService;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    
    private const int TEST_USER_ID = 1;
    
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

        var favorisManager = new FavorisManager(_context);
        var annonceManager = new AnnonceManager(_context);

        var mockCurrentUserService = new Mock<ICurrentUserService>();
        mockCurrentUserService
            .Setup(s => s.GetUserIdOrThrow())
            .ReturnsAsync(TEST_USER_ID);

        var mockSuggestionService = new Mock<ISuggestionService>();

        _controller = new FavorisController(
            favorisManager,
            annonceManager,
            _mapper,
            mockSuggestionService.Object,
            mockCurrentUserService.Object
        );
        InitializeDefaultObjects();
    }

    private void CleanupDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    private Annonce _defaultAnnonce;
    private Utilisateur _defaultUtilisateur;
    private Favoris _defaultFavoris;
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

        var categorie = new Categorie
        {
            CategorieId = 1,
            LibelleCategorie = "Vêtements"
        };

        var sousCategorie = new SousCategorie
        {
            SousCategorieId = 1,
            LibelleSousCategorie = "T-shirts"
        };

        var taille = new Taille
        {
            TailleId = 1,
            Libelletaille = "M"
        };

        var etat = new EtatArticle
        {
            EtatArticleId = 1,
            NomEtat = "Neuf"
        };

        var marque = new Marque
        {
            MarqueId = 1,
            NomMarque = "Nike"
        };

        var genre = new Genre
        {
            GenreId = 1,
            NomGenre = "Homme"
        };

        var statutAnnonce = new StatutAnnonce
        {
            StatutAnnonceId = 1,
            StatutLibelle = "En Ligne"
        };

        _defaultUtilisateur = new Utilisateur
        {
            UtilisateurId = TEST_USER_ID,
            Login = "TestUser",
            Email = "test@example.com",
            Password = "pwd",
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
            Statut = statut
        };

        _defaultAnnonce = new Annonce
        {
            AnnonceId = 1,
            Title = "Test Annonce",
            Description = "Description test",
            Prix = 50,
            UtilisateurId = TEST_USER_ID,
            Utilisateur = _defaultUtilisateur,
            CategorieId = categorie.CategorieId,
            Categorie = categorie,
            SousCategorieId = sousCategorie.SousCategorieId,
            SousCategorie = sousCategorie,
            TailleId = taille.TailleId,
            Taille = taille,
            EtatId = etat.EtatArticleId,
            Etat = etat,
            MarqueId = marque.MarqueId,
            Marque = marque,
            GenreId = genre.GenreId,
            GenreAnnonce = genre,
            StatutAnnonceId = statutAnnonce.StatutAnnonceId,
            Statut = statutAnnonce,
            DateAnnonce = DateTime.Now,
            Negociable = true
        };
    }

    [TestMethod]
    public async Task ShouldAddFavoris()
    {
        // Given
        _context.RolesUtilisateurs.Add(_defaultUtilisateur.Role);
        _context.StatutUtilisateurs.Add(_defaultUtilisateur.Statut);
        _context.Utilisateurs.Add(_defaultUtilisateur);
        _context.Categories.Add(_defaultAnnonce.Categorie);
        _context.SousCategories.Add(_defaultAnnonce.SousCategorie);
        _context.Tailles.Add(_defaultAnnonce.Taille);
        _context.EtatArticles.Add(_defaultAnnonce.Etat);
        _context.Marques.Add(_defaultAnnonce.Marque);
        _context.Genres.Add(_defaultAnnonce.GenreAnnonce);
        _context.StatutAnnonces.Add(_defaultAnnonce.Statut);
        _context.Annonces.Add(_defaultAnnonce);
        
        await _context.SaveChangesAsync();
        
        int annonceId = 1;

        // Act
        var action = await _controller.AddFavoris(annonceId);
        
        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(ObjectResult));
        var objectResult = action.Result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(StatusCodes.Status201Created, objectResult.StatusCode);
    }

    [TestMethod]
    public async Task ShouldAddFavorisReturnNotFound()
    {
        // Given
        int nonExistentAnnonceId = 999;

        // Act
        var action = await _controller.AddFavoris(nonExistentAnnonceId);
        
        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task ShouldAddFavorisReturnConflict()
    {
        // Given - Seed avec toutes les relations nécessaires
        var role = new RoleUtilisateur { RoleUtilisateurId = 1, RoleUtilisateurLibelle = "Utilisateur" };
        var statut = new StatutUtilisateur { StatutUtilisateurId = 1, StatutLibelle = "Actif" };
        var categorie = new Categorie { CategorieId = 1, LibelleCategorie = "Vêtements" };
        var sousCategorie = new SousCategorie { SousCategorieId = 1, LibelleSousCategorie = "T-shirts" };
        var taille = new Taille { TailleId = 1, Libelletaille = "M" };
        var etat = new EtatArticle { EtatArticleId = 1, NomEtat = "Neuf" };
        var marque = new Marque { MarqueId = 1, NomMarque = "Nike" };
        var genre = new Genre { GenreId = 1, NomGenre = "Homme" };
        var statutAnnonce = new StatutAnnonce { StatutAnnonceId = 1, StatutLibelle = "En Ligne" };

        var utilisateur = new Utilisateur
        {
            UtilisateurId = TEST_USER_ID,
            Login = "TestUser",
            Email = "test@example.com",
            Password = "pwd",
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
            Statut = statut
        };

        var annonce = new Annonce
        {
            AnnonceId = 1,
            Title = "Test Annonce",
            Description = "Desc",
            Prix = 50,
            UtilisateurId = TEST_USER_ID,
            Utilisateur = utilisateur,
            CategorieId = categorie.CategorieId,
            Categorie = categorie,
            SousCategorieId = sousCategorie.SousCategorieId,
            SousCategorie = sousCategorie,
            TailleId = taille.TailleId,
            Taille = taille,
            EtatId = etat.EtatArticleId,
            Etat = etat,
            MarqueId = marque.MarqueId,
            Marque = marque,
            GenreId = genre.GenreId,
            GenreAnnonce = genre,
            StatutAnnonceId = statutAnnonce.StatutAnnonceId,
            Statut = statutAnnonce,
            DateAnnonce = DateTime.Now,
            Negociable = true
        };

        var favorisExistant = new Favoris
        {
            UtilisateurId = TEST_USER_ID,
            AnnonceId = annonce.AnnonceId,
            Utilisateur = utilisateur,
            Annonce = annonce
        };

        // Ajouter dans le bon ordre (relations d'abord)
        _context.RolesUtilisateurs.Add(role);
        _context.StatutUtilisateurs.Add(statut);
        _context.Categories.Add(categorie);
        _context.SousCategories.Add(sousCategorie);
        _context.Tailles.Add(taille);
        _context.EtatArticles.Add(etat);
        _context.Marques.Add(marque);
        _context.Genres.Add(genre);
        _context.StatutAnnonces.Add(statutAnnonce);
        _context.Utilisateurs.Add(utilisateur);
        _context.Annonces.Add(annonce);
        _context.Favorises.Add(favorisExistant);
        
        await _context.SaveChangesAsync();

        // Act
        var action = await _controller.AddFavoris(annonce.AnnonceId);

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(ConflictObjectResult));
    }

    [TestMethod]
    public void ShouldDeleteFavorisReturnNoteFound()
    {
        //Given : 
        int nonExistentId = 999;
        
        
        //Act
        var action = _controller.DeleteFavoris(nonExistentId).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    [TestMethod]
    public async Task ShouldDeleteElement()
    {
        // Given - Seed avec toutes les relations nécessaires
        var role = new RoleUtilisateur { RoleUtilisateurId = 1, RoleUtilisateurLibelle = "Utilisateur" };
        var statut = new StatutUtilisateur { StatutUtilisateurId = 1, StatutLibelle = "Actif" };
        var categorie = new Categorie { CategorieId = 1, LibelleCategorie = "Vêtements" };
        var sousCategorie = new SousCategorie { SousCategorieId = 1, LibelleSousCategorie = "T-shirts" };
        var taille = new Taille { TailleId = 1, Libelletaille = "M" };
        var etat = new EtatArticle { EtatArticleId = 1, NomEtat = "Neuf" };
        var marque = new Marque { MarqueId = 1, NomMarque = "Nike" };
        var genre = new Genre { GenreId = 1, NomGenre = "Homme" };
        var statutAnnonce = new StatutAnnonce { StatutAnnonceId = 1, StatutLibelle = "En Ligne" };

        var utilisateur = new Utilisateur
        {
            UtilisateurId = TEST_USER_ID,
            Login = "TestUser",
            Email = "test@example.com",
            Password = "pwd",
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
            Statut = statut
        };

        var annonce = new Annonce
        {
            AnnonceId = 1,
            Title = "Test Annonce",
            Description = "Desc",
            Prix = 50,
            UtilisateurId = TEST_USER_ID,
            Utilisateur = utilisateur,
            CategorieId = categorie.CategorieId,
            Categorie = categorie,
            SousCategorieId = sousCategorie.SousCategorieId,
            SousCategorie = sousCategorie,
            TailleId = taille.TailleId,
            Taille = taille,
            EtatId = etat.EtatArticleId,
            Etat = etat,
            MarqueId = marque.MarqueId,
            Marque = marque,
            GenreId = genre.GenreId,
            GenreAnnonce = genre,
            StatutAnnonceId = statutAnnonce.StatutAnnonceId,
            Statut = statutAnnonce,
            DateAnnonce = DateTime.Now,
            Negociable = true
        };

        var favorisExistant = new Favoris
        {
            UtilisateurId = TEST_USER_ID,
            AnnonceId = annonce.AnnonceId,
            Utilisateur = utilisateur,
            Annonce = annonce
        };
        _context.RolesUtilisateurs.Add(role);
        _context.StatutUtilisateurs.Add(statut);
        _context.Categories.Add(categorie);
        _context.SousCategories.Add(sousCategorie);
        _context.Tailles.Add(taille);
        _context.EtatArticles.Add(etat);
        _context.Marques.Add(marque);
        _context.Genres.Add(genre);
        _context.StatutAnnonces.Add(statutAnnonce);
        _context.Utilisateurs.Add(utilisateur);
        _context.Annonces.Add(annonce);
        _context.Favorises.Add(favorisExistant);
        
        await _context.SaveChangesAsync();

        // Act
        var action = _controller.DeleteFavoris(annonce.AnnonceId).GetAwaiter().GetResult();

        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        var elementInDb = _context.Favorises
            .FirstOrDefault(e => e.AnnonceId == annonce.AnnonceId);
        Assert.IsNull(elementInDb);
    }
}