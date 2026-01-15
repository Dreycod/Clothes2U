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
using Shared.DTO.Annonce;
using Shared.DTO.EtatArticle;
using Shared.DTO.SousCategorie;

namespace API.Tests.Controllers.integration;

[TestClass]
[TestSubject(typeof(EtatArticleController))]
[TestCategory("integration")]
public class EtatArticleControllerTest
{
    private Clothes2UDbContext _context;
    private EtatArticleController _controller;
    private IMapper _mapper;
    
    private EtatArticle _default1, _default2;
    
    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        _context = new Clothes2UDbContext(builder.Options);
        CleanupDatabase();
        InitializeDefaultElements();
        
        var manager = new EtatArticleManager(_context);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AnnonceMappingProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();
        
        _controller = new EtatArticleController(manager, _mapper);
    }
    private void InitializeDefaultElements()
    {
        _default1 = new EtatArticle()
        {
            EtatArticleId = 1,
            NomEtat = "etat1"
            
        };
        _default2 = new EtatArticle()
        {
            EtatArticleId = 2,
            NomEtat = "etat2"
        };
    }
    [TestMethod]
    public void ShouldGetAllElements()
    {
        //Given : 
        _context.EtatArticles.AddRange(new []{_default1, _default2});
        _context.SaveChanges();
        //When : 
        ActionResult<IEnumerable<EtatArticleDTO>> action = _controller.GetAllEtats().GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<EtatArticleDTO>));
        var returnelements = okResult.Value as IEnumerable<EtatArticleDTO>;
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
                var allElements = _context.EtatArticles.ToList();
                _context.EtatArticles.RemoveRange(allElements);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du nettoyage des données : {ex.Message}");
            }
        }
    }
}