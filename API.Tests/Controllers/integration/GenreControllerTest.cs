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
using Shared.DTO.EtatArticle;

namespace API.Tests.Controllers.integration;



[TestClass]
[TestSubject(typeof(GenreController))]
[TestCategory("integration")]
public class GenreControllerTest
{
    private Clothes2UDbContext _context;
    private GenreController _controller;
    private IMapper _mapper;
    
    private Genre _default1, _default2;
    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        _context = new Clothes2UDbContext(builder.Options);
        CleanupDatabase();
        InitializeDefaultElements();
        
        var manager = new GenreManager(_context);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AnnonceMappingProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();
        
        _controller = new GenreController(manager, _mapper);
    }
    private void InitializeDefaultElements()
    {
        
        _default1 = new Genre()
        {
            GenreId = 1,
            NomGenre = "genre1"
        };

        _default2 = new Genre()
        {
            GenreId = 2,
            NomGenre = "genre2"
        };
    }
    [TestMethod]
    public void ShouldGetAllElements()
    {
        //Given : 
        _context.Genres.AddRange(new []{_default1, _default2});
        _context.SaveChanges();
        //When : 
        ActionResult<IEnumerable<GenreDTO>> action = _controller.GetAllGenres().GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<GenreDTO>));
        var returnelements = okResult.Value as IEnumerable<GenreDTO>;
        Assert.IsNotNull(returnelements);
        Assert.AreEqual(returnelements.Count(), 2);
        Assert.IsTrue(returnelements.Any(b => b.GetId() == _default1.GetId()));
        Assert.IsTrue(returnelements.Any(b => b.GetId() == _default2.GetId()));
    }
    private void CleanupDatabase()
    {
        if (_context != null)
        {
            try
            {
                var allElements = _context.Genres.ToList();
                _context.Genres.RemoveRange(allElements);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du nettoyage des données : {ex.Message}");
            }
        }
    }
}