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
using Shared.DTO.EtatArticle;
using Shared.DTO.Mesures;

namespace API.Tests.Controllers.integration;


[TestClass]
[TestSubject(typeof(MesureController))]
[TestCategory("integration")]
public class MesureControllerTest
{
    private Clothes2UDbContext _context;
    private MesureController _controller;
    private IMapper _mapper;
    
    private SousCategorie _defaultSousCategorie1, _defaultSousCategorie2;
    private Categorie _defaultCategorie1, _defaultCategorie2;
    private Mesure _default1, _default2, _default3;
    
    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        _context = new Clothes2UDbContext(builder.Options);
        CleanupDatabase();
        InitializeDefaultElements();
        
        var manager = new MesureManager(_context);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AnnonceMappingProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();
        
        _controller = new MesureController(manager, _mapper);
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
        _defaultSousCategorie1 = new SousCategorie()
        {
            SousCategorieId = 1,
            Categorie = _defaultCategorie1,
            LibelleSousCategorie = "SousCategorie1"
            
        };

        _defaultSousCategorie2 = new SousCategorie()
        {
            SousCategorieId = 2,
            Categorie = _defaultCategorie2,
            LibelleSousCategorie = "SousCategorie2"
        };
        _default1 = new Mesure()
        {
            MesureId = 1,
            SousCategorieId = 1,
            SousCategorieMesure = _defaultSousCategorie1
        };
        _default2 = new Mesure()
        {
            MesureId = 2,
            SousCategorieId = 1,
            SousCategorieMesure = _defaultSousCategorie1
        };
        _default3 = new Mesure()
        {
            MesureId = 3,
            SousCategorieId = 2,
            SousCategorieMesure = _defaultSousCategorie2
        };
    }
    
    [TestMethod]
    public void ShouldGetAllElements()
    {
        //Given : 
        _context.Mesures.AddRange(new []{_default1, _default2});
        _context.SaveChanges();
        //When : 
        ActionResult<IEnumerable<MesureDTO>> action = _controller.GetAllMesures().GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<MesureDTO>));
        var returnelements = okResult.Value as IEnumerable<MesureDTO>;
        Assert.IsNotNull(returnelements);
        Assert.AreEqual(returnelements.Count(), 2);
        Assert.IsTrue(returnelements.Any(b => b.GetId() == _default1.GetId()));
        Assert.IsTrue(returnelements.Any(b => b.GetId() == _default2.GetId()));
    }
    /*
    [TestMethod]
    public void ShouldGetElement()
    {
        //Given : 
        _context.Mesures.Add(_default1);
        _context.SaveChanges();
        
        //When : 
        ActionResult<MesureDTO> action = _controller.GetById((_default1.GetId())).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(MesureDTO));
        var returnElement = okResult.Value as MesureDTO;
        Assert.IsNotNull(returnElement);
        Assert.AreEqual(_default1.GetId(), returnElement.GetId());
    }
    [TestMethod]
    public void ShouldGetElementReturnNotFound()
    {
        //Given : 
        int nonExistentId = 999;
        
        //When : 
        ActionResult<MesureDTO> action = _controller.GetById(nonExistentId).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundObjectResult));
    }
    [TestMethod]
    public void ShouldAddElement()
    {
        //Given
        MesureDTO elementToAdd = new MesureDTO()
        {
            MesureId = 3,
            SousCategorieId = 2,
            
        };
        //When
        var action = _controller.AddMesure(elementToAdd).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
        var  createdResult = action.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(nameof(_controller.GetById), createdResult.ActionName);
        Assert.IsInstanceOfType(createdResult.Value, typeof(MesureDTO));
        var returnElement = createdResult.Value as MesureDTO;
        Assert.IsNotNull(returnElement);
        Assert.AreEqual(elementToAdd.LibelleSousCategorie, returnElement.LibelleSousCategorie);
        var elementInDb = _context.SousCategories.FirstOrDefault(c => c.CategorieId == returnElement.GetId());
        Assert.IsNotNull(elementInDb);
        Assert.AreEqual(returnElement.GetId(), elementInDb.GetId());
    }*/
    private void CleanupDatabase()
    {
        if (_context != null)
        {
            try
            {
                var allElements = _context.Mesures.ToList();
                _context.Mesures.RemoveRange(allElements);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du nettoyage des données : {ex.Message}");
            }
        }
    }
}