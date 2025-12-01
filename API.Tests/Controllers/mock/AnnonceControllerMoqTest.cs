using System;
using System.Collections.Generic;
using System.Linq;
using API.Controllers;
using API.DTO.Annonce;
using API.Mapper;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace API.Tests.Controllers.mock;

[TestClass]
[TestSubject(typeof(AnnonceController))]
[TestCategory("mock")]  
public class AnnonceControllerMoqTest
{
    private readonly AnnonceController _controller;
    private readonly Mock<IAnnonceRepository<Annonce,int>>  _manager;
    private readonly IMapper _mapper;
    
    private Annonce _default2, _default1, _default3;
    private Utilisateur _defaultUser, _defaultUser2,_defaultUser3;
    private Photo _defaultPhoto;
    private Marque _defaultMarque1, _defaultMarque2;
    private EtatArticle _defaultEtat1,_defaultEtat2;
    private Taille _defaultTaille1, _defaultTaille2;
    private Categorie _defaultCategorie,_defaultCategorie1;
    private SousCategorie _defaultSousCategorie1,_defaultSousCategorie2, _defaultSousCategorie3;
    private StatutAnnonce _defaultStatutEnLigne;
    private Favoris _favoris1, _favoris2, _favoris3;

    public AnnonceControllerMoqTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GenericProfile>();
        });
        _mapper = config.CreateMapper();
        _manager = new Mock<IAnnonceRepository<Annonce,int>>();
        _controller = new AnnonceController(_manager.Object, _mapper);
    }
    
    [TestInitialize]
    public void Setup()
    {
        _defaultPhoto = new Photo
        {
            PhotoId = 1,
            Image = new byte[] { 255, 216, 255, 224, 0, 16, 74, 70, 73, 70 }
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
        _defaultCategorie1 = new Categorie
        {
            CategorieId = 2,
            LibelleCategorie = "Couvre-Chef"
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
        _defaultSousCategorie3 = new SousCategorie
        {
            SousCategorieId = 3,
            LibelleSousCategorie = "Casquettes",
            CategorieId = 2
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
            Description = "Veste en jean de neuf en couleur blanche.",
            DateAnnonce = DateTime.Now.AddMinutes(-35),
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
            Description = "Chaussures Nike en couleur blanche.",
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

        _default3 = new Annonce()
        {
            AnnonceId = 3,
            Title = "Casquette",
            Description = "Casquette en couleur blanche.",
            DateAnnonce = DateTime.Now,
            Negociable = true,
            Prix = 19.99m,
            UtilisateurId = 2,
            EtatId = 1,
            MarqueId = 2,
            TailleId = 1,
            SousCategorieId = 1,
            CategorieId = 2,
            StatutAnnonceId = 1
        };
        
        _favoris1 = new Favoris 
        { 
            UtilisateurId = 1, 
            AnnonceId = 2
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
        
        _default1.UtilisateursFavoris = new List<Favoris> { _favoris3 }; // 1 like
        _default2.UtilisateursFavoris = new List<Favoris> { _favoris1, _favoris2 }; // 2 likes
        _default3.UtilisateursFavoris = new List<Favoris>(); // 0 like
    }
    
    [TestMethod]
    public void ShouldGetActiveAnnoncesMoq()
    {
        // Arrange
        _manager
            .Setup(manager => manager.GetActiveAnnonces())
            .ReturnsAsync(new[] { _default1, _default2 , _default3});
        
        // Act
        var result = _controller.GetActiveAnnonces().GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(3, annonces.Count());
        
        _manager.Verify(manager => manager.GetActiveAnnonces(), Times.Once);
    }

    [TestMethod]
    public void ShouldGetAllAnnonceByCategorieIdMoq()
    {
        // Arrange
        _manager
            .Setup(manager => manager.GetByCategorieId(1))
            .ReturnsAsync(new[] {_default1, _default2});
        
        //Act
        var result = _controller.GetAllByCategorieId(1).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(2, annonces.Count());
        
        _manager.Verify(manager => manager.GetByCategorieId(1), Times.Once);
    }
    
    [TestMethod]
    public void ShouldGetAllAnnonceByUtilisateurIdMoq()
    {
        // Arrange
        _manager
            .Setup(manager => manager.GetByUtilisateurId(1))
            .ReturnsAsync(new[] {_default1, _default2});
        
        //Act
        var result = _controller.GetAllByUtilisateurId(1).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(2, annonces.Count());
        
        _manager.Verify(manager => manager.GetByUtilisateurId(1), Times.Once);
    }
    
    [TestMethod]
    public void ShouldGetAllAnnonceBySousCategorieIdMoq()
    {
        // Arrange
        _manager
            .Setup(manager => manager.GetBySousCategorieId(1))
            .ReturnsAsync(new[] {_default1});
        
        //Act
        var result = _controller.GetAllBySousCategorieId(1).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(1, annonces.Count());
        
        _manager.Verify(manager => manager.GetBySousCategorieId(1), Times.Once);
    }

    [TestMethod]
    public void ShouldGetAnnonceByIdMoq()
    {
        //Arrange
        _manager
            .Setup(manager => manager.GetByIdAsync(_default1.AnnonceId))
            .ReturnsAsync(_default1);
        
        //Act
        ActionResult<AnnonceDetailDTO> result = _controller.GetById(_default1.AnnonceId).GetAwaiter().GetResult();
        
        //Assert
        _manager.Verify(manager => manager.GetByIdAsync(_default1.AnnonceId), Times.Once);
        
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(ActionResult<AnnonceDetailDTO>));
        
        OkObjectResult okObjectResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okObjectResult);
        AnnonceDetailDTO annonceValue = okObjectResult.Value as AnnonceDetailDTO;
        AnnonceDetailDTO annonceDTO = _mapper.Map<AnnonceDetailDTO>(_default1);
        Assert.AreEqual(annonceDTO.AnnonceId, annonceValue.AnnonceId);
        Assert.AreEqual(annonceDTO.Description, annonceValue.Description);
        Assert.AreEqual(annonceDTO.NomMarque, annonceValue.NomMarque);
        Assert.AreEqual(annonceDTO.UtilisateurId, annonceValue.UtilisateurId);
    }

    [TestMethod]
    public void ShouldNotGetAnnonceByIdBecauseAnnonceDoesNotExistMoq()
    {
        //Arrange
        _manager
            .Setup(manager => manager.GetByIdAsync(_default1.AnnonceId))
            .ReturnsAsync((Annonce)null);
        
        //Act
        ActionResult<AnnonceDetailDTO> result = _controller.GetById(_default1.AnnonceId).GetAwaiter().GetResult();
        
        //Assert
        _manager.Verify(manager => manager.GetByIdAsync(_default1.AnnonceId), Times.Once);
        
        Assert.IsNotNull(result);
        var notFoundResult = result.Result as NotFoundResult;
        Assert.IsInstanceOfType(notFoundResult, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetByFavorisUtilisateurMoq()
    {
        //Arrange
        _manager
            .Setup(manager => manager.GetByUtilisateurFavoris(2))
            .ReturnsAsync(new[] { _default1, _default2 });
        
        //Act
        var result = _controller.GetByFavorisUtilisateur(2).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(2, annonces.Count());
        
        _manager.Verify(manager => manager.GetByUtilisateurFavoris(2), Times.Once);
    }

    [TestMethod]
    public void ShouldGetMostLikedMoq()
    {
        //Arrange
        _manager
            .Setup(manager => manager.GetPlusLikeAsync())
            .ReturnsAsync(new[] {_default1, _default2, _default3 }
                .OrderByDescending(annonce => annonce.UtilisateursFavoris.Count));
        
        //Act
        var result = _controller.GetMostLiked().GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(3, annonces.Count());
        Assert.AreEqual(annonces.First().AnnonceId, _default2.AnnonceId);
        
        _manager.Verify(manager => manager.GetPlusLikeAsync(), Times.Once);
    }

    [TestMethod]
    public void ShouldGetMostRecentMoq()
    {
        //Arrange
        _manager
            .Setup(manager => manager.GetMostRecentAsync())
            .ReturnsAsync(new[] { _default1, _default2, _default3 }
                .OrderByDescending(a => a.DateAnnonce));
        
        //Act
        var result = _controller.GetMostRecent().GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var annonces = okResult.Value as IEnumerable<AnnonceDTO>;
        Assert.IsNotNull(annonces);
        Assert.AreEqual(3, annonces.Count());
        Assert.AreEqual(annonces.First().AnnonceId, _default3.AnnonceId);
        
        _manager.Verify(manager => manager.GetMostRecentAsync(), Times.Once);
    }

    [TestMethod]
    public void ShouldAddAnnonceMoq()
    {
        //Arrange
        Annonce AnnonceToAdd = _default1;
        AnnonceDetailDTO AnnonceToAddDTO = _mapper.Map<AnnonceDetailDTO>(AnnonceToAdd);

        _manager
            .Setup(manager => manager.AddAsync(AnnonceToAdd));

        //Act
        var result = _controller.AddAnnonce(AnnonceToAddDTO).GetAwaiter().GetResult();
        
        //Assert
        _manager.Verify(manager => manager.AddAsync(AnnonceToAdd), Times.Once);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(ActionResult<AnnonceDetailDTO>));
        CreatedAtActionResult createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        AnnonceDetailDTO AnnonceDetailDTO = createdResult.Value as AnnonceDetailDTO;
        Assert.AreEqual(AnnonceDetailDTO.AnnonceId, AnnonceToAdd.AnnonceId);
    }

    [TestMethod]
    public void ShouldDeleteAnnonceMoq()
    {
        //Arrange
        _manager
            .Setup(manager => manager.GetByIdAsync(_default1.AnnonceId))
            .ReturnsAsync(_default1);

        _manager
            .Setup(manager => manager.DeleteAsync(_default1));

        //Act
        IActionResult action = _controller.DeleteAnnonce(_default1.AnnonceId).GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        
        _manager.Verify(manager => manager.GetByIdAsync(_default1.AnnonceId), Times.Once);
        _manager.Verify(manager => manager.DeleteAsync(_default1), Times.Once);
    }
    
    [TestMethod]
    public void ShouldNotDeleteAnnonceBecauseAnnonceDoesNotExistMoq()
    {
        // Arrange
        _manager
            .Setup(manager => manager.GetByIdAsync(_default1.AnnonceId))
            .ReturnsAsync((Annonce)null);
        
        // Act
        IActionResult action = _controller.DeleteAnnonce(_default1.AnnonceId).GetAwaiter().GetResult();
        
        // Assert
        _manager.Verify(manager => manager.GetByIdAsync(_default1.AnnonceId), Times.Once);

        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
}