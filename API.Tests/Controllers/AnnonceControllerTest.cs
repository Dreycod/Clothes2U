using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.DTO.Annonce;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace API.Tests.Controllers;

[TestClass]
[TestSubject(typeof(AnnonceController))]
[TestCategory("integration")]
public class AnnonceControllerTest
{
    private Clothes2UDbContext _context;
    private AnnonceController _controller;
    private IMapper _mapper;
    
    private Annonce _default2, _default1;
    private Utilisateur _defaultUser, _defaultUser2,_defaultUser3;
    private Photo _defaultPhoto;
    private Marque _defaultMarque1, _defaultMarque2;
    private EtatArticle _defaultEtat1,_defaultEtat2;
    private Taille _defaultTaille1, _defaultTaille2;
    private Categorie _defaultCategorie;
    private SousCategorie _defaultSousCategorie1,_defaultSousCategorie2;
    private StatutAnnonce _defaultStatutEnLigne;
    private Favoris _favoris1, _favoris2, _favoris3;


    [TestInitialize]
    public void Init()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        _context = new Clothes2UDbContext(builder.Options);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GenericProfile>();
        });
        
        _mapper = config.CreateMapper();
        
        InitialzeDefaultAnnonces();
        
        var manager = new AnnonceManager(_context);
        _controller = new AnnonceController(manager, _mapper);
    }
    
    private void InitialzeDefaultAnnonces()
    {
        _defaultPhoto = new Photo
        {
            PhotoId = 1,
            PhotoUri = "https://example.com/profile.jpg"
        };

        // Utilisateur
        _defaultUser = new Utilisateur
        {
            UtilisateurId = 1,
            Login = "TestUser",
            Email = "test@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("password"),
            Description = "Test User",
            PhotoId = 1,
            PhotoProfil = _defaultPhoto
        };
        
        _defaultUser2 = new Utilisateur
        {
            UtilisateurId = 2,
            Login = "favorisUtilisateur",
            Email = "test1@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("password"),
            Description = "Test User2",
            PhotoId = 1,
            PhotoProfil = _defaultPhoto
        };
        
        _defaultUser3 = new Utilisateur
        {
            UtilisateurId = 3,
            Login = "favorisUtilisateur3",
            Email = "test3@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("password"),
            Description = "Test User3",
            PhotoId = 1,
            PhotoProfil = _defaultPhoto
        };

        // Marques
        _defaultMarque1 = new Marque
        {
            MarqueId = 1,
            NomMarque = "Levi's"
        };
        _defaultMarque2 = new Marque
        {
            MarqueId = 2,
            NomMarque = "Nike"
        };

        // États
        _defaultEtat1 = new EtatArticle
        {
            EtatArticleId = 1,
            NomEtat = "Neuf"
        };
        _defaultEtat2 = new EtatArticle()
        {
            EtatArticleId = 2,
            NomEtat = "Bon état"
        };

        // Tailles
        _defaultTaille1 = new Taille
        {
            TailleId = 1,
            Libelletaille = "M"
        };
        _defaultTaille2 = new Taille
        {
            TailleId = 2,
            Libelletaille = "42"
        };

        // Catégorie
        _defaultCategorie = new Categorie
        {
            CategorieId = 1,
            LibelleCategorie = "Vêtements"
        };

        // Sous-catégories
        _defaultSousCategorie1 = new SousCategorie
        {
            SousCategorieId = 1,
            LibelleSousCategorie = "Vestes",
            CategorieId = 1
        };
        _defaultSousCategorie2 = new SousCategorie
        {
            SousCategorieId = 2,
            LibelleSousCategorie = "Chaussures",
            CategorieId = 1
        };

        // Statut
        _defaultStatutEnLigne = new StatutAnnonce
        {
            StatutAnnonceId = 1,
            StatutLibelle = "En Ligne"
        };

        _default1 = new Annonce()
        {
            AnnonceId = 1,
            Title = "Veste en jean",
            DateAnnonce = DateTime.Now,
            Negociable = true,
            Prix = 29.99m,
            UtilisateurId = 1,
            EtatId = 1,
            MarqueId = 1,
            TailleId = 1,
            SousCategorieId = 1,
            CategorieId = 1,
            StatutAnnonceId = 1
        };
        _default2 = new Annonce()
        {
            AnnonceId = 2,
            Title = "Chaussures Nike",
            DateAnnonce = DateTime.Now.AddMinutes(-30),
            Negociable = false,
            Prix = 59.90m,
            UtilisateurId = 1,
            EtatId = 2,
            MarqueId = 2,
            TailleId = 2,
            SousCategorieId = 2,
            CategorieId = 1,
            StatutAnnonceId = 1
        };
        
        _favoris1 = new Favoris 
        { 
            UtilisateurId = 2, 
            AnnonceId = 1 
        };
        _favoris2 = new Favoris 
        { 
            UtilisateurId = 2, 
            AnnonceId = 2 
        };

        _favoris3 = new Favoris
        {
            UtilisateurId = 3,
            AnnonceId = 1
        };
    }
    private void SeedDatabase()
    {
        _context.Photos.Add(_defaultPhoto);
        _context.Marques.AddRange(_defaultMarque1, _defaultMarque2);
        _context.EtatArticles.AddRange(_defaultEtat1, _defaultEtat2);
        _context.Tailles.AddRange(_defaultTaille1, _defaultTaille2);
        _context.Categories.Add(_defaultCategorie);
        _context.StatutAnnonces.Add(_defaultStatutEnLigne);
        _context.Favorises.AddRange(_favoris1, _favoris2, _favoris3);
        //_context.SaveChanges();

        _context.Utilisateurs.AddRange(_defaultUser, _defaultUser2, _defaultUser3);
        _context.SousCategories.AddRange(_defaultSousCategorie1, _defaultSousCategorie2);
        _context.SaveChanges();
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    

    [TestMethod]
    public async Task ShouldGetActiveAnnonces()
    {
        //Arrange
        SeedDatabase();
        _context.Annonces.AddRange(new[] {_default1, _default2});;
        _context.SaveChanges();
        
        //Act
        var result = await _controller.GetActiveAnnonces();
        
        //Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(2, annonces.Count());
    }

    [TestMethod]
    public async Task ShouldReturnEmptyListWhenNoActiveAnnonces()
    {
        //Arrange
        SeedDatabase();
        
        //Act
        var result = await _controller.GetActiveAnnonces();
        
        //Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(0, annonces.Count());
    }
     
    [TestMethod]
    public async Task ShouldGetAnnonceById()
    {
        // Arrange
        SeedDatabase();
        _context.Annonces.Add(_default1);
        _context.SaveChanges();
        
        // Act
        var result = await _controller.GetById(1);
        
        // Assert
        AnnonceDetailDTO annonce = null;
        if (result.Value != null)
        {
            annonce = result.Value;
        }
        else if (result.Result is OkObjectResult okResult)
        {
            annonce = okResult.Value as AnnonceDetailDTO;
        }
        
        Assert.IsNotNull(annonce, "L'annonce retournée ne devrait pas être null");
        Assert.AreEqual(1, annonce.AnnonceId);
        Assert.AreEqual("Veste en jean", annonce.Title);
        Assert.AreEqual(29.99m, annonce.Prix);
        Assert.AreEqual("Levi's", annonce.NomMarque);
        Assert.AreEqual("Neuf", annonce.EtatArticle);
        Assert.AreEqual("M", annonce.Taille);
    }

    [TestMethod]
    public async Task ShouldReturnNotFound_GetById()
    {
        // Arrange
        SeedDatabase();
        
        // Act
        var result = await _controller.GetById(100);
        
        // Assert
        Assert.IsNotNull(result.Result);
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task ShouldReturnList_GetAllByCategorieId()
    {
        // Arrange
        SeedDatabase();
        _context.Annonces.AddRange(new[] {_default1, _default2});
        _context.SaveChanges();
        
        // Act
        var result = await _controller.GetAllByCategorieId(1);
        
        // Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(2, annonces.Count());
    }

    [TestMethod]
    public async Task ShouldReturnEmptyList_GetByAllCategorieId()
    {
        // Arrange
        SeedDatabase();
        
        // Act
        var result = await _controller.GetAllByCategorieId(100);
        
        // Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(0, annonces.Count());
    }
    
    [TestMethod]
    public async Task ShouldReturnList_GetAllBySousCategorieId()
    {
        // Arrange
        SeedDatabase();
        _context.Annonces.AddRange(new[] {_default1, _default2});
        _context.SaveChanges();
        
        // Act
        var result = await _controller.GetAllBySousCategorieId(1);
        
        // Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(1, annonces.Count());
    }

    [TestMethod]
    public async Task ShouldReturnEmptyList_GetByAllSousCategorieId()
    {
        // Arrange
        SeedDatabase();
        
        // Act
        var result = await _controller.GetAllBySousCategorieId(100);
        
        // Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(0, annonces.Count());
    }
    
    public async Task ShouldReturnList_GetAllByUtilisateurId()
    {
        // Arrange
        SeedDatabase();
        _context.Annonces.AddRange(new[] {_default1, _default2});
        _context.SaveChanges();
        
        // Act
        var result = await _controller.GetAllByUtilisateurId(1);
        
        // Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(2, annonces.Count());
    }

    [TestMethod]
    public async Task ShouldReturnEmptyList_GetByAllUtilisateurId()
    {
        // Arrange
        SeedDatabase();
        
        // Act
        var result = await _controller.GetAllByUtilisateurId(100);
        
        // Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(0, annonces.Count());
    }

    [TestMethod]
    public async Task ShouldReturnList_GetByFavorisUtilisateur()
    {
        //Arrange
        SeedDatabase();
        _context.Annonces.AddRange(new[] {_default1, _default2});
        _context.SaveChanges();
        
        //Act
        var result = await _controller.GetByFavorisUtilisateur(2);
        
        //Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
    
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(2, annonces.Count());
    
    }
    
    [TestMethod]
    public async Task ShouldReturnEmptyList_GetByFavorisUtilisateur()
    {
        // Arrange
        SeedDatabase();
        
        // Act
        var result = await _controller.GetByFavorisUtilisateur(100);
        
        // Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(0, annonces.Count());
    }

    [TestMethod]
    public async Task ShouldReturnList_GetMostLiked()
    {
        //Arrange
        SeedDatabase();
        _context.Annonces.AddRange(new[] {_default1, _default2});
        _context.SaveChanges();
        
        //Act
        var result = await _controller.GetMostLiked();
        
        //Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
    
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(2, annonces.Count());
        Assert.AreEqual(1, annonces.First().Id);
    }
    
    [TestMethod]
    public async Task ShouldReturnEmptyList_GetMostLiked()
    {
        // Arrange
        SeedDatabase();
        
        // Act
        var result = await _controller.GetMostLiked();
        
        // Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(0, annonces.Count());
    }

    [TestMethod]
    public async Task ShouldReturnList_GetMostRecentAnnonce()
    {
        //Arrange
        SeedDatabase();
        _context.Annonces.AddRange(new[] {_default1, _default2});
        _context.SaveChanges();
        
        //Act
        var result = await _controller.GetMostRecent();
        
        //Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
    
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(2, annonces.Count());
        Assert.AreEqual(1, annonces.First().Id);
    }

    [TestMethod]
    public async Task ShouldReturnEmptyList_GetMostRecent()
    {
        // Arrange
        SeedDatabase();
        
        // Act
        var result = await _controller.GetMostRecent();
        
        // Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(0, annonces.Count());
    }

    [TestMethod]
    public async Task ShouldCreateAnnonce()
    {
        //Arrange
        AnnonceDetailDTO annonceToAdd = _mapper
    }
}