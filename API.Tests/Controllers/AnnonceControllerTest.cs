using System;
using System.Collections.Generic;
using System.Linq;
using API.Controllers;
using API.DTO.Annonce;
using API.Mapper;
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
[TestSubject(typeof(AnnonceController))]
[TestCategory("integration")]
public class AnnonceControllerTest
{
    private Clothes2UDbContext _context;
    private AnnonceController _controller;
    private IMapper _mapper;
    
    private Annonce _default2, _default1;

    [TestInitialize]
    public void Init()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        _context = new Clothes2UDbContext(builder.Options);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GenericProfile>();
        });
        
        _mapper = config.CreateMapper();
        
        InitialzeDefaultAnnonces();
        
        var manager = new AnnonceManager(_context);
        _controller = new AnnonceController(manager, _mapper);
    }
    
    private void InitialzeDefaultAnnonces()
    {
        _default1 = new Annonce()
        {
            AnnonceId = 1,
            Title = "Veste en jean",
            DateAnnonce = DateTime.Now,
            Negociable = true,
            Prix = 29.99m,
            UtilisateurId = 1,
            EtatId = 1,
            MarqueId = 1,
            TailleId = 1,
            SousCategorieId = 1,
            CategorieId = 1,
            StatutAnnonceId = 1
        };
        _default2 = new Annonce()
        {
            AnnonceId = 2,
            Title = "Chaussures Nike",
            DateAnnonce = DateTime.Now.AddMinutes(-30),
            Negociable = false,
            Prix = 59.90m,
            UtilisateurId = 1,
            EtatId = 2,
            MarqueId = 2,
            TailleId = 2,
            SousCategorieId = 2,
            CategorieId = 1,
            StatutAnnonceId = 1
        };
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void ShouldGetActiveAnnonces()
    {
        //given
        _context.Annonces.AddRange(new[] {_default1, _default2});;
        _context.SaveChanges();
        
        //when
        ActionResult<IEnumerable<AnnonceDTO>> result = _controller.GetActiveAnnonces().GetAwaiter().GetResult();
        
        //then
        Assert.IsNotNull(result.Value);
        Assert.IsInstanceOfType(result.Value, typeof(IEnumerable<AnnonceDTO>));
        IEnumerable<AnnonceDTO> annoncesList = result.Value.ToList();
        Assert.AreEqual(2, annoncesList.Count());
    }

    [TestMethod]
    public void ShouldGetAnnonceById()
    {
        //given
        _context.Annonces.Add(_default1);
        _context.SaveChanges();
        //when
        ActionResult<AnnonceDetailDTO> result = _controller.GetById(1).GetAwaiter().GetResult();
        //then
        AnnonceDetailDTO annonce = null;
        if (result.Value != null)
        {
            annonce = result.Value;
        }
        else if (result.Result is OkObjectResult okResult)
        {
            annonce = okResult.Value as AnnonceDetailDTO;
        }
        Assert.IsNotNull(annonce, "L'annonce retournée ne devrait pas être null");
        Assert.AreEqual("Veste en jean", annonce.Title);
        Assert.AreEqual(29.99m, annonce.Prix);
    }
}