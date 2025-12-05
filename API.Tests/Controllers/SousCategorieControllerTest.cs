using System;
using API.Controllers;
using API.Mapper;
using API.Models;
using API.Models.Repository.Managers;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace API.Tests.Controllers;

[TestClass]
[TestSubject(typeof(SousCategorieController))]
public class SousCategorieControllerTest
{
    private SousCategorieController _controller;
    private IMapper _mapper;
    private Clothes2UDbContext _context;

    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        
        _context = new Clothes2UDbContext(builder.Options);
        
        //InitializeDefaultMarques();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GenericProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();

        var manager = new SousCategorieManager(_context);
        
        _controller = new SousCategorieController(manager, _mapper);
    }
    
    [TestMethod]
    public void ShouldGetAllSousCategorie()
    {
        //Arrange
        
    }
}