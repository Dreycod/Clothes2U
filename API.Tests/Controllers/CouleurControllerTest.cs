using System;
using System.Collections.Generic;
using System.Linq;
using API.Controllers;
//using API.DTO.Couleur;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.DTO.Couleur;

namespace API.Tests.Controllers;

[TestClass]
[TestSubject(typeof(CouleurController))]
public class CouleurControllerTest
{

    private CouleurController _controller;
    private IMapper _mapper;
    private Clothes2UDbContext _context;
    
    private Couleur _default1, _default2;
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

        var manager = new CouleurManager(_context);
        
        _controller = new CouleurController(manager, _mapper);
    }
    
    private void InitializeDefaultSousCategories()
    {
        _default1 = new Couleur
        {
            CouleurId = 1,
            Nom = "rouge"
        };
        _default2 = new Couleur
        {
            CouleurId = 2,
            Nom = "bleu"
        };
    }
    
    [TestMethod]
    public void ShouldGetAllSousCategorie()
    {
        //Arrange
        _context.Couleurs.AddRange(new[] {_default1, _default2});
        _context.SaveChanges();
        
        //Act
        var result = _controller.GetAllCouleurs().GetAwaiter().GetResult();
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult.Value);
        var returnedData = okResult.Value as IEnumerable<CouleurDTO>;
        Assert.IsNotNull(returnedData);
        Assert.AreEqual(2, returnedData.Count());
        
    }
}