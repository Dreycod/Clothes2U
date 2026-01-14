using System;
using System.Collections.Generic;
using System.Linq;
using API.Controllers;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.DTO.Categorie;
using Shared.DTO.Couleur;

namespace API.Tests.Controllers.integration;


[TestClass]
[TestSubject(typeof(CouleurController))]
[TestCategory("integration")]
public class CouleurControllerTest
{
    private Clothes2UDbContext _context;
    private CouleurController _controller;
    private IMapper _mapper;
    
    private Couleur _default1, _default2;

    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        _context = new Clothes2UDbContext(builder.Options);
        CleanupDatabase();
        InitializeDefaultCouleurs();
        
        var manager = new CouleurManager(_context);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AnnonceMappingProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();
        
        _controller = new CouleurController(manager, _mapper);
    }
    private void InitializeDefaultCouleurs()
    {
        _default1 = new Couleur
        {
            CouleurId = 1,
            Nom = "couleur1"
        };

        _default2 = new Couleur
        {
            CouleurId = 2,
            Nom = "couleur2"
        };
    }
    [TestMethod]
    public void ShouldGetElementById()
    {
        //Given : 
        _context.Couleurs.Add(_default1);
        _context.SaveChanges();
        
        //When : 
        ActionResult<Couleur> action = _controller.GetById((_default1.GetId())).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(Couleur));
        var returnelement = okResult.Value as Couleur;
        Assert.IsNotNull(returnelement);
        Assert.AreEqual(_default1.GetId(), returnelement.GetId());
    }
    [TestMethod]
    public void ShouldGetElementReturnNotFound()
    {
        //Given : 
        int nonExistentId = 999;
        
        //When : 
        ActionResult<Couleur> action = _controller.GetById(nonExistentId).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
    }
    [TestMethod]
    public void ShouldGetAllCategories()
    {
        //Given : 
        _context.Couleurs.AddRange(new []{_default1, _default2});
        _context.SaveChanges();
        //When : 
        ActionResult<IEnumerable<CouleurDTO>> action = _controller.GetAllCouleurs().GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<CouleurDTO>));
        var returnelements = okResult.Value as IEnumerable<CouleurDTO>;
        Assert.IsNotNull(returnelements);
        Assert.AreEqual(returnelements.Count(), 2);
        Assert.IsTrue(returnelements.Any(b => b.GetId() == _default1.GetId()));
        Assert.IsTrue(returnelements.Any(b => b.GetId() == _default2.GetId()));
    }
    [TestMethod]
    public void ShouldAddElement()
    {
        //Given
        var newElementDto = new CouleurDTO
        {
            Nom = "element"
        };
        //When
        var action = _controller.AddCouleur(newElementDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
        var  createdResult = action.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(nameof(_controller.GetById), createdResult.ActionName);
        Assert.IsInstanceOfType(createdResult.Value, typeof(Couleur));
        var returnElement = createdResult.Value as Couleur;
        Assert.IsNotNull(returnElement);
        Assert.AreEqual(newElementDto.Nom, returnElement.Nom);
        var categoryInDb = _context.Couleurs.FirstOrDefault(c => c.CouleurId == returnElement.GetId());
        Assert.IsNotNull(categoryInDb);
        Assert.AreEqual(returnElement.GetId(), categoryInDb.GetId());
    }
    [TestMethod]
    public void ShouldReturnBadRequest_AddElement_WhenModelStateInvalid()
    {
        //Given
        var invalidElementDto = new CouleurDTO
        {
            Nom = null 
        };
        _controller.ModelState.AddModelError("Nom", "Required");
    
        //When
        var action = _controller.AddCouleur(invalidElementDto).GetAwaiter().GetResult();
    
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(BadRequestObjectResult));
    }
    [TestMethod]
    public void ShouldDeletedElement()
    {
        //Given 
        _context.Couleurs.Add(_default1);
        _context.SaveChanges();
        var elementId = _default1.CouleurId;
        
        //When 
        var action = _controller.DeleteCouleur(elementId).GetAwaiter().GetResult();
        
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        var elementInDb = _context.Couleurs.FirstOrDefault(m => m.CouleurId == elementId);
        Assert.IsNull(elementInDb);
    }
    [TestMethod]
    public void ShouldReturnNotFound_DeleteElement_WhenIdDoesNotExist()
    {
        //Given 
        var nonExistentId = 9999;
        
        //When 
        var action = _controller.DeleteCouleur(nonExistentId).GetAwaiter().GetResult();
        
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    [TestMethod]
    public void ShouldUpdateElement()
    {
        //Given
        _context.Couleurs.Add(_default1);
        _context.SaveChanges();
        var elementId = _default1.GetId();
        var updatedElementDto = new CouleurDTO
        {
            CouleurId = elementId,
            Nom = "Element"
        };
        
        //Act
        var action = _controller.PutCouleur(elementId, updatedElementDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        var elementInDb = _context.Couleurs.FirstOrDefault(m => m.CouleurId == elementId);
        Assert.IsNotNull(elementInDb);
        Assert.AreEqual("Element", elementInDb.Nom);
    }
    [TestMethod]
    public void ShouldReturnNotFound_PutElement_WhenIdDoesNotExist()
    {
        //Given
        var nonExistentId = 9999;
        var elementDto = new CouleurDTO
        {
            CouleurId = nonExistentId,
            Nom = "Element"
        };
        
        //Act
        var action = _controller.PutCouleur(nonExistentId, elementDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    [TestMethod]
    public void ShouldReturnBadRequest_PutCouleur_WhenIdsDoNotMatch()
    {
        //Given
        _context.Couleurs.Add(_default1);
        _context.SaveChanges();
        var urlId = _default1.CouleurId;
        var elementDto = new CouleurDTO
        {
            CouleurId = urlId + 1,
            Nom = "Element"
        };
        
        //Act
        var action = _controller.PutCouleur(urlId, elementDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
        var elementInDb = _context.Couleurs.FirstOrDefault(m => m.CouleurId == urlId);
        Assert.IsNotNull(elementInDb);
        Assert.AreEqual(_default1.CouleurId, elementInDb.CouleurId);
    }
    private void CleanupDatabase()
    {
        if (_context != null)
        {
            try
            {
                var allElements = _context.Couleurs.ToList();
                _context.Couleurs.RemoveRange(allElements);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du nettoyage des données : {ex.Message}");
            }
        }
    }
}