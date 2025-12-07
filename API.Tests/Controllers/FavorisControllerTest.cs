using System;
using System.Linq;
using System.Security.Claims;
using API.Controllers;
using API.DTO.Favoris;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace API.Tests.Controllers;

[TestClass]
[TestSubject(typeof(FavorisController))]
[TestCategory("integration")]
public class FavorisControllerTest
{

    private FavorisController _controller;
    private IMapper _mapper;
    private Clothes2UDbContext _context;
    
    private Favoris _default1;
    private Utilisateur _defaultUser2;
    private Annonce _annonce;
    private Favoris _favoris1, _favoris2, _favoris3;
    private Marque _defaultMarque1, _defaultMarque2;
    private EtatArticle _defaultEtat1,_defaultEtat2;
    private Taille _defaultTaille1, _defaultTaille2;
    private Categorie _defaultCategorie;
    private StatutAnnonce _defaultStatutEnLigne;

    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        
        _context = new Clothes2UDbContext(builder.Options);
        
        InitializeDefaultSousCategories();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GenericProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();

        var manager = new FavorisManager(_context);
        var annonceManager = new AnnonceManager(_context);
        
        _controller = new FavorisController(manager,annonceManager, _mapper);
    }
    
    private void InitializeDefaultSousCategories()
    {
        
        _default1 = new Favoris
        {
            FavorisId = 1,
            UtilisateurId = 1,
            AnnonceId = 1
        };
    }

    private void SeedDatabase()
    {
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

         

         // Statut
         _defaultStatutEnLigne = new StatutAnnonce
         {
             StatutAnnonceId = 1,
             StatutLibelle = "En Ligne"
         };
         
        _defaultUser2 = new Utilisateur
        {
            UtilisateurId = 1,
            Login = "favorisUtilisateur",
            Email = "test1@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("password"),
            Description = "Test User2",
            
            PhotoId = 1,
        };
        
        _context.Utilisateurs.Add(_defaultUser2);

        _annonce = new Annonce
        {
            AnnonceId = 1,
            Title = "Veste en jean",
            Description = "Veste en jean de neuf en couleur blanche.",
            DateAnnonce = DateTime.Now,
            Negociable = true,
            Prix = 29.99m,
            UtilisateurId = 5,
            EtatId = 1,
            MarqueId = 1,
            TailleId = 1,
            SousCategorieId = 1,
            CategorieId = 1,
            StatutAnnonceId = 1
        };
        
        _context.Annonces.Add(_annonce);
         _context.Marques.AddRange(_defaultMarque1, _defaultMarque2);
         _context.EtatArticles.AddRange(_defaultEtat1, _defaultEtat2);
         _context.Tailles.AddRange(_defaultTaille1, _defaultTaille2);
         _context.Categories.Add(_defaultCategorie);
         _context.StatutAnnonces.Add(_defaultStatutEnLigne);
         //_context.SaveChanges();
        _context.SaveChanges();
        _context.Entry(_annonce).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    
        // Recharger pour être sûr
        _annonce = _context.Annonces.Find(1);
    }

    [TestMethod]
    public void ShouldGetFavorisById()
    {
        
        //Arrange
        SeedDatabase();
        _context.Favorises.Add(_default1);
        _context.SaveChanges();
        
        //Act
        var result = _controller.GetById(_default1.FavorisId).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Value, typeof(Favoris));
        Assert.AreEqual(_default1.FavorisId, result.Value.FavorisId);
    }

    [TestMethod]
    public void ShouldReturnNotFoundGetById()
    {
        //Arrange
        SeedDatabase();
        
        //Act
        var result = _controller.GetById(100).GetAwaiter().GetResult();
        
        //Assert
        
        Assert.IsNotNull(result.Result);
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldAddFavoris()
    {
        //Arrange
        SeedDatabase();
        
        
        
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("userId", "1")
        }, "TestAuthentication"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        //Act
        var result = _controller.AddFavoris(_annonce.AnnonceId).GetAwaiter().GetResult();
        
        //Assert
        
        var exists = _context.Annonces.Any(a => a.AnnonceId == _annonce.AnnonceId);
        Assert.IsTrue(exists);
        
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        
    }
    
    [TestMethod]
    public void ShouldNotAddFavorisCauseAnnonceDoesNotExist()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("userId", "1")
        }, "TestAuthentication"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        //Act
        var result = _controller.AddFavoris(0).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));

    }

    [TestMethod]
    public void ShouldNotAddAnnonceCauseNotConnected()
    {
        //Arrange
        SeedDatabase();
        
        //Act
        var result = _controller.AddFavoris(_annonce.AnnonceId).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    public void ShouldNotAddFavorisCauseIdInvalide()
    {
        //Arrange
        SeedDatabase();
        
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("userId", "nullId")
        }, "TestAuthentication"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        //Act
        var result = _controller.AddFavoris(_annonce.AnnonceId).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
        var unAuthorizedResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(unAuthorizedResult);
        Assert.AreEqual(unAuthorizedResult.Value, "ID utilisateur invalide");

    }
    
    [TestMethod]
    public void ShouldNotAddFavorisCauseAnnonceAlreadyInFavoris()
    {
        //Arrange
        SeedDatabase();
        _context.Favorises.Add(_default1);
        _context.SaveChanges();
        
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("userId", "1")
        }, "TestAuthentication"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        //Act
        var result = _controller.AddFavoris(_annonce.AnnonceId).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(ConflictObjectResult));
        var conflit = result.Result as ConflictObjectResult;
        Assert.IsNotNull(conflit);
        Assert.AreEqual(conflit.Value, "Vous avez déjà mis ajouté cette annonce à vos favoris");
    }
    
    [TestMethod]
    public void ShouldDeleteFavoris()
    {
        //Arrange
        SeedDatabase();
        _context.Favorises.Add(_default1);
        _context.SaveChanges();
        
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("userId", "1")
        }, "TestAuthentication"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        //Act
        var result = _controller.DeleteFavoris(_default1.FavorisId).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }
    
    [TestMethod]
    public void ShouldNotDeleteFavorisCauseIdDoesNotExist()
    {
        //Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("userId", "1")
        }, "TestAuthentication"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        //Act
        var result = _controller.DeleteFavoris(0).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldNotDeleteFavorisCauseUserNotConnected()
    {
        //Arrange
        SeedDatabase();
        _context.Favorises.Add(_default1);
        _context.SaveChanges();
        
        //Act
        var result = _controller.DeleteFavoris(_default1.FavorisId).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        var unauthorizedObjectResult = result as UnauthorizedObjectResult;
        Assert.AreEqual(unauthorizedObjectResult.Value, "Vous devez être connecté pour ajouter un favori");
    }
    
    [TestMethod]
    public void ShouldNotDeleteFavorisCauseIdInvalide()
    {
        //Arrange
        SeedDatabase();
        _context.Favorises.Add(_default1);
        _context.SaveChanges();
        
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("userId", "null")
        }, "TestAuthentication"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        //Act
        var result = _controller.DeleteFavoris(_default1.FavorisId).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        var unAuthorizedObjectResult = result as UnauthorizedObjectResult;
        Assert.AreEqual(unAuthorizedObjectResult.Value, "ID utilisateur invalide");
    }
}