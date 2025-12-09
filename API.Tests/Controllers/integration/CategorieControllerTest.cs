using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.DTO.Annonce;
using API.DTO.Categorie;
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

        InitializeDefaultCategories();
        
        var manager = new CategorieManager(_context);

        var config = new MapperConfiguration(cfg =>
        {
            //cfg.AddProfile<GenericProfile>();
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
    public async Task ShouldGetAllCategorieWithNavigation()
    {
        //Arrange
        _context.Categories.AddRange(new[] {_default1, _default2});
        _context.SaveChanges();
        
        //Act
        var result = await _controller.GetAllCategorieWithNavigation();
        
        //Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        
        var categories = okResult.Value as IEnumerable<CategorieDTO>;
        Assert.IsNotNull(categories);
        Assert.AreEqual(2, categories.Count());
    }

    [TestMethod]
    public async Task ShouldReturnEmptyList_GetAllCategorieWithNavigation()
    {
        //Arrange
        
        //Act
        var result = await _controller.GetAllCategorieWithNavigation();
        
        //Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var categories = okResult.Value as IEnumerable<CategorieDTO>;
        Assert.IsNotNull(categories);
        Assert.AreEqual(0, categories.Count());
    }
}