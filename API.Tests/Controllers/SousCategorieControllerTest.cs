using System;
using System.Collections.Generic;
using System.Linq;
using API.Controllers;
//using API.DTO.SousCategorie;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.DTO.SousCategorie;

namespace API.Tests.Controllers;
/*
[TestClass]
[TestSubject(typeof(SousCategorieController))]
public class SousCategorieControllerTest
{
    private SousCategorieController _controller;
    private IMapper _mapper;
    private Clothes2UDbContext _context;
    
    private SousCategorie _default1, _default2;
    private Categorie _categorie;

    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        
        _context = new Clothes2UDbContext(builder.Options);
        
        InitializeDefaultSousCategories();
        var config = new MapperConfiguration(cfg =>
        {
            //cfg.AddProfile<GenericProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();

        var manager = new SousCategorieManager(_context);
        
        _controller = new SousCategorieController(manager, _mapper);
    }
    
    private void InitializeDefaultSousCategories()
    {
        
        _categorie = new Categorie
        {
            CategorieId = 1,
            LibelleCategorie = "test"
        };
        _context.Categories.Add(_categorie);
        _context.SaveChanges();
        _default1 = new SousCategorie
        {
            SousCategorieId = 1,
            LibelleSousCategorie = "Homme",
            CategorieId = 1
        };
        _default2 = new SousCategorie
        {
            SousCategorieId = 2,
            LibelleSousCategorie = "Femme",
            CategorieId = 1
        };
    }
    
    [TestMethod]
    public void ShouldGetAllSousCategorie()
    {
        //Arrange
        _context.SousCategories.AddRange(new[] {_default1, _default2});
        _context.SaveChanges();
        
        //Act
        var result = _controller.GetAll().GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult.Value);
        var returnedData = okResult.Value as IEnumerable<SousCategorieDTO>;
        Assert.IsNotNull(returnedData);
        Assert.AreEqual(2, returnedData.Count());
        
    }
}*/