using System;
using System.Collections.Generic;
using System.Linq;
using API.Controllers;
using API.DTO.Annonce;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace API.Tests.Controllers;

[TestClass]
[TestSubject(typeof(MarqueController))]
[TestCategory("integration")]
public class MarqueControllerTest
{
    private MarqueController _controller;
    private Clothes2UDbContext _context;
    private Marque _default1 ,_default2, _default3;

    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        
        _context = new Clothes2UDbContext(builder.Options);
        
        InitializeDefaultMarques();
        
        var manager = new MarqueManager(_context);
        
        _controller = new MarqueController(manager);
        
    }

    public void InitializeDefaultMarques()
    {
        _default1 = new Marque
        {
            MarqueId = 1,
            NomMarque = "Nike"
        };
        _default2 = new Marque
        {
            MarqueId = 2,
            NomMarque = "Adidas"
        };
        _default3 = new Marque
        {
            MarqueId = 3,
            NomMarque = "Chrome Hearts"
        };
    }
    
    [TestMethod]
    public void ShouldGetAllMarques()
    {
        //Arrange
        _context.Marques.AddRange(_default1, _default2, _default3);
        _context.SaveChanges();
        
        //Act
        var result = _controller.GetAllMarques();
        
        //Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as ActionResult<Marque>;
         Assert.IsNotNull(okResult);
        
         var okObject = okResult.Result as OkObjectResult;
         Assert.IsNotNull(okObject);
         var annonces = okObject.Value as IEnumerable<Marque>;
         Assert.IsNotNull(annonces);
         Assert.AreEqual(3, annonces.Count());
    }
    
    [TestMethod]
    public void ShouldGetMarqueById()
    {
        //Arrange
        _context.Marques.AddRange(_default1, _default2, _default3);
        _context.SaveChanges();
        
        //Act
        var result = _controller.GetById(_default3.MarqueId);
        
        //Assert
        Assert.IsNotNull(result.Result);
        var okResult = result.Result as ActionResult<Marque>;
        Assert.IsNotNull(okResult);
        
        var okObject = okResult.Result as OkObjectResult;
        Assert.IsNotNull(okObject);
        var annonces = okObject.Value as Marque;
        Assert.IsNotNull(annonces);
        Assert.AreEqual("Chrome Hearts", annonces.NomMarque);
    }
    
    [TestMethod]
    public void ShouldReturnNotFound_GetById()
    {
        // Arrange
        var id = 12;
        
        // Act
        var result =  _controller.GetById(12).GetAwaiter().GetResult();
        
        // Assert
        Assert.IsNotNull(result.Result);
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }
}