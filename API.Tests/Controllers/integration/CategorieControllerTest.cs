using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
// using API.DTO.Annonce;
// using API.DTO.Categorie;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using API.Tests.Helpers;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.DTO.Categorie;

namespace API.Tests.Controllers;

[TestClass]
[TestSubject(typeof(CategorieController))]
[TestCategory("integration")]
public class CategorieControllerTest
{
    private Clothes2UDbContext _context;
    private CategorieController _controller;
    private IMapper _mapper;
    
    private Categorie _default1, _default2;

    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        _context = new Clothes2UDbContext(builder.Options);
        CleanupDatabase();
        InitializeDefaultCategories();
        
        var manager = new CategorieManager(_context);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AnnonceMappingProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();
        
        _controller = new CategorieController(manager, _mapper);
    }

    private void InitializeDefaultCategories()
    {
        _default1 = new Categorie
        {
            CategorieId = 1,
            LibelleCategorie = "Homme"
        };

        _default2 = new Categorie
        {
            CategorieId = 2,
            LibelleCategorie = "Femme"
        };
    }
    
    [TestMethod]
    public void ShouldGetCategorie()
    {
        //Given : 
        _context.Categories.Add(_default1);
        _context.SaveChanges();
        
        //When : 
        ActionResult<Categorie> action = _controller.GetById((_default1.GetId())).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(Categorie));
        var returncategorie = okResult.Value as Categorie;
        Assert.IsNotNull(returncategorie);
        Assert.AreEqual(_default1.GetId(), returncategorie.GetId());
    }
    [TestMethod]
    public void ShouldGetCategorieReturnNotFound()
    {
        //Given : 
        int nonExistentId = 999;
        
        //When : 
        ActionResult<Categorie> action = _controller.GetById(nonExistentId).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
    }
    [TestMethod]
    public void ShouldGetAllCategories()
    {
        //Given : 
        _context.Categories.AddRange(new []{_default1, _default2});
        _context.SaveChanges();
        //When : 
        ActionResult<IEnumerable<CategorieDTO>> action = _controller.GetAllCategorieWithNavigation().GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<CategorieDTO>));
        var returncategories = okResult.Value as IEnumerable<CategorieDTO>;
        Assert.IsNotNull(returncategories);
        Assert.AreEqual(returncategories.Count(), 2);
        Assert.IsTrue(returncategories.Any(b => b.GetId() == _default1.GetId()));
        Assert.IsTrue(returncategories.Any(b => b.GetId() == _default2.GetId()));
    }
    

    private void CleanupDatabase()
    {
        if (_context != null)
        {
            try
            {
                var allBrands = _context.Marques.ToList();
                _context.Marques.RemoveRange(allBrands);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du nettoyage des données : {ex.Message}");
            }
        }
    }

    
}