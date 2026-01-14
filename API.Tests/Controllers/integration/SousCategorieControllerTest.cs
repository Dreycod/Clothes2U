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
using Shared.DTO.SousCategorie;

namespace API.Tests.Controllers.integration;


[TestClass]
[TestSubject(typeof(SousCategorieController))]
[TestCategory("integration")]
public class SousCategorieControllerTest
{
    private Clothes2UDbContext _context;
    private SousCategorieController _controller;
    private IMapper _mapper;
    
    private SousCategorie _default1, _default2;
    private Categorie _defaultCategorie1, _defaultCategorie2;
    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        _context = new Clothes2UDbContext(builder.Options);
        CleanupDatabase();
        InitializeDefaultElements();
        
        var manager = new SousCategorieManager(_context);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AnnonceMappingProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();
        
        _controller = new SousCategorieController(manager, _mapper);
    }
    private void InitializeDefaultElements()
    {
        _defaultCategorie1 = new Categorie
        {
            CategorieId = 1,
            LibelleCategorie = "Homme"
        };

        _defaultCategorie2 = new Categorie
        {
            CategorieId = 2,
            LibelleCategorie = "Femme"
        };
        _default1 = new SousCategorie()
        {
            SousCategorieId = 1,
            Categorie = _defaultCategorie1,
            LibelleSousCategorie = "SousCategorie1"
            
        };

        _default2 = new SousCategorie()
        {
            SousCategorieId = 2,
            Categorie = _defaultCategorie2,
            LibelleSousCategorie = "SousCategorie2"
        };
    }
    [TestMethod]
    public void ShouldGetElement()
    {
        //Given : 
        _context.SousCategories.Add(_default1);
        _context.SaveChanges();
        
        //When : 
        ActionResult<SousCategorie> action = _controller.GetById((_default1.GetId())).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(SousCategorie));
        var returnElement = okResult.Value as SousCategorie;
        Assert.IsNotNull(returnElement);
        Assert.AreEqual(_default1.GetId(), returnElement.GetId());
    }
    [TestMethod]
    public void ShouldGetElementReturnNotFound()
    {
        //Given : 
        int nonExistentId = 999;
        
        //When : 
        ActionResult<SousCategorie> action = _controller.GetById(nonExistentId).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
    }
    [TestMethod]
    public void ShouldGetAllElements()
    {
        //Given : 
        _context.SousCategories.AddRange(new []{_default1, _default2});
        _context.SaveChanges();
        //When : 
        ActionResult<IEnumerable<SousCategorieDTO>> action = _controller.GetAll().GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<SousCategorieDTO>));
        var returnelements = okResult.Value as IEnumerable<SousCategorieDTO>;
        Assert.IsNotNull(returnelements);
        Assert.AreEqual(returnelements.Count(), 2);
        Assert.IsTrue(returnelements.Any(b => b.GetId() == _default1.GetId()));
        Assert.IsTrue(returnelements.Any(b => b.GetId() == _default2.GetId()));
    }
    [TestMethod]
    public void ShouldAddElement()
    {
        //Given
        SousCategoriePostDTO elementToAdd = new SousCategoriePostDTO()
        {
            SousCategorieId = 1,
            LibelleSousCategorie = "SousCategorie1",
            CategorieId = 1
            
        };
        //When
        var action = _controller.AddSousCategorie(elementToAdd).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
        var  createdResult = action.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(nameof(_controller.GetById), createdResult.ActionName);
        Assert.IsInstanceOfType(createdResult.Value, typeof(SousCategorie));
        var returnElement = createdResult.Value as SousCategorie;
        Assert.IsNotNull(returnElement);
        Assert.AreEqual(elementToAdd.LibelleSousCategorie, returnElement.LibelleSousCategorie);
        var elementInDb = _context.SousCategories.FirstOrDefault(c => c.CategorieId == returnElement.GetId());
        Assert.IsNotNull(elementInDb);
        Assert.AreEqual(returnElement.GetId(), elementInDb.GetId());
    }
    [TestMethod]
    public void ShouldReturnBadRequest_AddElement_WhenModelStateInvalid()
    {
        //Given
        SousCategoriePostDTO invalidElementDto = new SousCategoriePostDTO()
        {
            SousCategorieId = 1,
            LibelleSousCategorie = null,
            CategorieId = 1
            
        };
        _controller.ModelState.AddModelError("LibelleSousCategorie", "Required");
    
        //When
        var action = _controller.AddSousCategorie(invalidElementDto).GetAwaiter().GetResult();
    
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(BadRequestObjectResult));
    }
    [TestMethod]
    public void ShouldDeletedElement()
    {
        //Given 
        _context.SousCategories.Add(_default1);
        _context.SaveChanges();
        var elementId = _default1.SousCategorieId;
        
        //When 
        var action = _controller.DeleteSousCategorie(elementId).GetAwaiter().GetResult();
        
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        var categorieInDb = _context.SousCategories.FirstOrDefault(m => m.SousCategorieId == elementId);
        Assert.IsNull(categorieInDb);
    }
    [TestMethod]
    public void ShouldReturnNotFound_DeleteElement_WhenIdDoesNotExist()
    {
        //Given 
        var nonExistentId = 9999;
        
        //When 
        var action = _controller.DeleteSousCategorie(nonExistentId).GetAwaiter().GetResult();
        
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    
    [TestMethod]
    public void ShouldUpdateCategory()
    {
        //Given
        _context.SousCategories.Add(_default1);
        _context.SaveChanges();
        var elementId = _default1.GetId();
        var updatedElementDto = new SousCategoriePostDTO
        {
            SousCategorieId = elementId,
            CategorieId = 2,
            LibelleSousCategorie = "SousCategorie"
        };
        
        //Act
        var action = _controller.PutSousCategorie(elementId, updatedElementDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        var elementInDb = _context.SousCategories.FirstOrDefault(m => m.SousCategorieId == elementId);
        Assert.IsNotNull(elementInDb);
        Assert.AreEqual("SousCategorie", elementInDb.LibelleSousCategorie);
    }
    [TestMethod]
    public void ShouldReturnNotFound_PutElement_WhenIdDoesNotExist()
    {
        //Given
        var nonExistentId = 9999;
        var elementDto = new SousCategoriePostDTO
        {
            SousCategorieId = nonExistentId,
            CategorieId = 2,
            LibelleSousCategorie = "SousCategorie"
        };
        
        //Act
        var action = _controller.PutSousCategorie(nonExistentId, elementDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    [TestMethod]
    public void ShouldReturnBadRequest_PutCategory_WhenIdsDoNotMatch()
    {
        //Given
        _context.SousCategories.Add(_default1);
        _context.SaveChanges();
        var urlId = _default1.CategorieId;
        var elementDto = new SousCategoriePostDTO
        {
            SousCategorieId = urlId + 1,
            CategorieId = 2,
            LibelleSousCategorie = "SousCategorie"
        };
        
        //Act
        var action = _controller.PutSousCategorie(urlId, elementDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
        var elementInDb = _context.SousCategories.FirstOrDefault(m => m.CategorieId == urlId);
        Assert.IsNotNull(elementInDb);
        Assert.AreEqual(_default1.CategorieId, elementInDb.CategorieId);
    }

    private void CleanupDatabase()
    {
        if (_context != null)
        {
            try
            {
                var allElements = _context.SousCategories.ToList();
                _context.SousCategories.RemoveRange(allElements);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du nettoyage des données : {ex.Message}");
            }
        }
    }
}