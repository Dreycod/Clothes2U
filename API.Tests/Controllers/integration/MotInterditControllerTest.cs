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
using Shared.DTO;
using Shared.DTO.MotInterdit;

namespace API.Tests.Controllers.integration;


[TestClass]
[TestSubject(typeof(MotInterditController))]
[TestCategory("integration")]
public class MotInterditControllerTest
{
    private Clothes2UDbContext _context;
    private MotInterditController _controller;
    private IMapper _mapper;
    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        _context = new Clothes2UDbContext(builder.Options);
        CleanupDatabase();
        InitializeDefaultElements();
        
        var manager = new MotInterditManager(_context);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ModerationMappingProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();
        
        _controller = new MotInterditController(manager, _mapper);
    }
    private void CleanupDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    private MotInterdit _default1, _default2;
    private void InitializeDefaultElements()
    {
        _default1 = new MotInterdit
        {
            MotinterditId = 1,
            LibelleMot = "mot1"
        };
        _default2 = new MotInterdit
        {
            MotinterditId = 2,
            LibelleMot = "mot2"
        };
    }
    [TestMethod]
    public void ShouldGetAllElements()
    {
        //Given : 
        _context.MotsInterdits.AddRange(new []{_default1, _default2});
        _context.SaveChanges();
        //When : 
        ActionResult<IEnumerable<MotInterditDTO>> action = _controller.GetMotInterdit().GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<MotInterditDTO>));
        var returnelements = okResult.Value as IEnumerable<MotInterditDTO>;
        Assert.IsNotNull(returnelements);
        Assert.AreEqual(returnelements.Count(), 2);
        Assert.IsTrue(returnelements.Any(b => b.GetId() == _default1.GetId()));
        Assert.IsTrue(returnelements.Any(b => b.GetId() == _default2.GetId()));
    }

    [TestMethod]
    public void ShouldDeleteReturnNotFound()
    {
        //Given
        int nonExistentId = 999;
        //Act
        var action = _controller.DeleteMotInterdit(nonExistentId).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldDelete()
    {
        //Given
        _context.MotsInterdits.Add(_default1);
        _context.SaveChanges();
        //Act
        var action = _controller.DeleteMotInterdit(_default1.MotinterditId).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        var elementInDb = _context.MotsInterdits
            .FirstOrDefault(e => e.MotinterditId == _default1.MotinterditId);
        Assert.IsNull(elementInDb);
    }

    [TestMethod]
    public void ShouldCreate()
    {
        //Given
        MotInterditDTO elementToAdd = new MotInterditDTO
        {
            LibelleMot = "mot",
            MotInterditId = 1
        };
        //Act
        var action = _controller.AddMotInterdit(elementToAdd).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(ObjectResult));
        var  createdResult = action.Result as ObjectResult;
        Assert.IsNotNull(createdResult);
        Assert.IsInstanceOfType(createdResult.Value, typeof(MotInterditDTO));
        var returnElement = createdResult.Value as MotInterditDTO;
        Assert.IsNotNull(returnElement);
        Assert.AreEqual(elementToAdd.LibelleMot, returnElement.LibelleMot);
        var elementInDb = _context.MotsInterdits.FirstOrDefault(c => c.MotinterditId == returnElement.GetId());
        Assert.IsNotNull(elementInDb);
        Assert.AreEqual(returnElement.GetId(), elementInDb.GetId());
    }
    [TestMethod]
    public void ShouldReturnBadRequest_AddElement_WhenModelStateInvalid()
    {
        //Given
        MotInterditDTO elementToAdd = new MotInterditDTO
        {
            LibelleMot = null,
            MotInterditId = 1
        };
        _controller.ModelState.AddModelError("LibelleMot", "Required");
    
        //When
        var action = _controller.AddMotInterdit(elementToAdd).GetAwaiter().GetResult();
    
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void ShouldReturnBadRequestBecauseMotAlreadyExists()
    {
        //Given
        _context.MotsInterdits.Add(_default1);
        _context.SaveChanges();
        var elementToAdd = new MotInterditDTO()
        {
            LibelleMot = _default1.LibelleMot 
        };
        //When
        var action = _controller.AddMotInterdit(elementToAdd).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(BadRequestObjectResult));
    }
}