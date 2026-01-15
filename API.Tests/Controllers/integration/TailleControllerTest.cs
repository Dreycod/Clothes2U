using System;
using System.Collections.Generic;
using System.Linq;
using API.Controllers;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.DTO.Mesures;
using Shared.DTO.SousCategorie;
using Shared.DTO.Taille;

namespace API.Tests.Controllers.integration;



[TestClass]
[TestSubject(typeof(TailleController))]
[TestCategory("integration")]
public class TailleControllerTest
{
    private Clothes2UDbContext _context;
    private TailleController _controller;
    private IMapper _mapper;

    private Categorie _defaultCategorie1;
    private SousCategorie _defaultSousCategorie1;
    private Taille _default1, _default2;
    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        _context = new Clothes2UDbContext(builder.Options);
        CleanupDatabase();
        InitializeDefaultElements();
        
        var manager = new TailleManager(_context);
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AnnonceMappingProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();
        
        _controller = new TailleController(manager, _mapper);
    }
    private void InitializeDefaultElements()
    {
        
        _default1 = new Taille()
        {
            TailleId = 1,
            Libelletaille = "taille1"
            
        };

        _default2 = new Taille()
        {
            TailleId = 2,
            Libelletaille = "taille2"
            
        };
        _defaultCategorie1 = new Categorie
        {
            CategorieId = 1,
            LibelleCategorie = "Homme"
        };
        _defaultSousCategorie1 = new SousCategorie()
        {
            SousCategorieId = 1,
            Categorie = _defaultCategorie1,
            LibelleSousCategorie = "SousCategorie1"
            
        };
    }
    [TestMethod]
    public void ShouldGetElement()
    {
        //Given : 
        _context.Tailles.Add(_default1);
        _context.SaveChanges();
        
        //When : 
        ActionResult<Taille> action = _controller.GetById((_default1.GetId())).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(Taille));
        var returnElement = okResult.Value as Taille;
        Assert.IsNotNull(returnElement);
        Assert.AreEqual(_default1.GetId(), returnElement.GetId());
    }
    [TestMethod]
    public void ShouldGetElementReturnNotFound()
    {
        //Given : 
        int nonExistentId = 999;
        
        //When : 
        ActionResult<Taille> action = _controller.GetById(nonExistentId).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
    }
    [TestMethod]
    public void ShouldGetAllElements()
    {
        //Given : 
        _context.Tailles.AddRange(new []{_default1, _default2});
        _context.SaveChanges();
        //When : 
        ActionResult<IEnumerable<TailleDTO>> action = _controller.GetAllTaille().GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<TailleDTO>));
        var returnelements = okResult.Value as IEnumerable<TailleDTO>;
        Assert.IsNotNull(returnelements);
        Assert.AreEqual(returnelements.Count(), 2);
        Assert.IsTrue(returnelements.Any(b => b.GetId() == _default1.GetId()));
        Assert.IsTrue(returnelements.Any(b => b.GetId() == _default2.GetId()));
    }
    [TestMethod]
    public void ShouldAddElement()
    {
        //Given
        var newElementDto = new TailleDTO
        {
            Libelletaille = "element"
        };
        //When
        var action = _controller.AddTaille(newElementDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
        var  createdResult = action.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(nameof(_controller.GetById), createdResult.ActionName);
        Assert.IsInstanceOfType(createdResult.Value, typeof(Taille));
        var returnElement = createdResult.Value as Taille;
        Assert.IsNotNull(returnElement);
        Assert.AreEqual(newElementDto.Libelletaille, returnElement.Libelletaille);
        var elementInDb = _context.Tailles.FirstOrDefault(c => c.TailleId == returnElement.GetId());
        Assert.IsNotNull(elementInDb);
        Assert.AreEqual(returnElement.GetId(), elementInDb.GetId());
    }
    [TestMethod]
    public void ShouldReturnBadRequest_AddElement_WhenModelStateInvalid()
    {
        //Given
        var invalidElementDto = new TailleDTO
        {
            Libelletaille = null
        };
        _controller.ModelState.AddModelError("LibelleTaille", "Required");
    
        //When
        var action = _controller.AddTaille(invalidElementDto).GetAwaiter().GetResult();
    
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(BadRequestObjectResult));
    }
    [TestMethod]
    public void ShouldDeletedElement()
    {
        //Given 
        _context.Tailles.Add(_default1);
        _context.SaveChanges();
        var elementId = _default1.TailleId;
        
        //When 
        var action = _controller.DeleteTaille(elementId).GetAwaiter().GetResult();
        
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        var elementInDb = _context.Tailles.FirstOrDefault(m => m.TailleId == elementId);
        Assert.IsNull(elementInDb);
    }
    [TestMethod]
    public void ShouldReturnNotFound_DeleteElement_WhenIdDoesNotExist()
    {
        //Given 
        var nonExistentId = 9999;
        
        //When 
        var action = _controller.DeleteTaille(nonExistentId).GetAwaiter().GetResult();
        
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    [TestMethod]
    public void ShouldUpdateElement()
    {
        //Given
        _context.Tailles.Add(_default1);
        _context.SaveChanges();
        var elementId = _default1.GetId();
        var updatedElementDto = new TailleDTO()
        {
            TailleId = elementId,
            Libelletaille = "element"
        };
        
        //Act
        var action = _controller.PutTaille(elementId, updatedElementDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        var elementInDb = _context.Tailles.FirstOrDefault(m => m.TailleId == elementId);
        Assert.IsNotNull(elementInDb);
        Assert.AreEqual("element", elementInDb.Libelletaille);
    }
    [TestMethod]
    public void ShouldReturnNotFound_PutElement_WhenIdDoesNotExist()
    {
        //Given
        var nonExistentId = 9999;
        var elementDto = new TailleDTO()
        {
            TailleId = nonExistentId,
            Libelletaille = "element"
        };
        
        //Act
        var action = _controller.PutTaille(nonExistentId, elementDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    [TestMethod]
    public void ShouldReturnBadRequest_PutElement_WhenIdsDoNotMatch()
    {
        //Given
        _context.Tailles.Add(_default1);
        _context.SaveChanges();
        var urlId = _default1.GetId();
        var elementDto = new TailleDTO()
        {
            TailleId = urlId + 1,
            Libelletaille = "element"
        };
        
        //Act
        var action = _controller.PutTaille(urlId, elementDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
        var elementInDb = _context.Tailles.FirstOrDefault(m => m.TailleId == urlId);
        Assert.IsNotNull(elementInDb);
        Assert.AreEqual(_default1.GetId(), elementInDb.GetId());
    }
    [TestMethod]
    public void ShouldUpdateTailleMesuresReturnBadRequest()
    {
        //Given
        MesureDTO mesureDTO = new MesureDTO()
        {
            MesureId = 1,
            TailleId = _default1.GetId(),
            SousCategorieId = _defaultSousCategorie1.GetId()
        };
        int badId = 999;
        List<MesureDTO> mesures = new List<MesureDTO>(new []{mesureDTO});
        //Act
        var action = _controller.PutTailleMesures(badId, mesures).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
    }
    [TestMethod]
    public void ShouldUpdateTailleMesuresReturnNoContent()
    {
        //Given
        MesureDTO mesureDTO = new MesureDTO()
        {
            MesureId = 1,
            TailleId = _default1.GetId(),
            SousCategorieId = _defaultSousCategorie1.GetId()
        };
        _context.Tailles.Add(_default1);
        _context.SaveChanges();
        List<MesureDTO> mesures = new List<MesureDTO>(new []{mesureDTO});
        //Act
        var action = _controller.PutTailleMesures(_default1.GetId(), mesures).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
    }
    private void CleanupDatabase()
    {
        if (_context != null)
        {
            try
            {
                var allElements = _context.Tailles.ToList();
                _context.Tailles.RemoveRange(allElements);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du nettoyage des données : {ex.Message}");
            }
        }
    }
}