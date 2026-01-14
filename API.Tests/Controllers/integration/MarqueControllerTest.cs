using System;
using System.Collections.Generic;
using System.Linq;
using API.Controllers;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using API.Tests.Helpers;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.DTO.Marque;

namespace API.Tests.Controllers.integration;

[TestClass]
[TestSubject(typeof(MarqueController))]
[TestCategory("integration")]
public class MarqueControllerTest
{
    private Clothes2UDbContext _context;
    private IMapper _mapper;
    private MarqueController _marqueController;
    
    private Marque _defaultBrand1;
    private Marque _defaultBrand2;
    private Marque _defaultBrand3;
    
    [TestInitialize]
    public void SetUp()
    {
        
        _context = DbContextHelper.GetInMemoryContext();
        CleanupDatabase();
        InitializeDefaultBrands();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AnnonceMappingProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();
        
        var manager = new MarqueManager(_context);
   
        _marqueController = new MarqueController(manager, mapper);
    }
    private void InitializeDefaultBrands()
    {
        _defaultBrand1 = new Marque
        {
            NomMarque = "marque1"
        };
        _defaultBrand2 = new Marque
        {
            NomMarque = "marque2"
        };
        _defaultBrand3 = new Marque
        {
            NomMarque = "marque3"
        };
    }
    
    
    [TestMethod]
    public void ShouldGetBrand()
    {
        //Given : 
        _context.Marques.Add(_defaultBrand1);
        _context.SaveChanges();
        
        //When : appelle une marque avec getbyid
        ActionResult<Marque> action = _marqueController.GetById((_defaultBrand1.MarqueId)).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(Marque));
        var returnBrand = okResult.Value as Marque;
        Assert.IsNotNull(returnBrand);
        Assert.AreEqual(_defaultBrand1.MarqueId, returnBrand.MarqueId);
    }
    [TestMethod]
    public void ShouldGetBrandReturnNotFound()
    {
        //Given : pas besoin d'ajouter des elements
        int nonExistentId = 999;
        
        //When : appelle une marque avec getbyid
        ActionResult<Marque> action = _marqueController.GetById(nonExistentId).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
    }
    [TestMethod]
    public void ShouldGetAllBrands()
    {
        //Given : 
        _context.Marques.AddRange(new []{_defaultBrand1, _defaultBrand2});
        _context.SaveChanges();
        //When : 
        var action = _marqueController.GetAllMarques().GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<MarqueDTO>));
        var returnBrands = okResult.Value as IEnumerable<MarqueDTO>;
        Assert.IsNotNull(returnBrands);
        Assert.AreEqual(returnBrands.Count(), 2);
        Assert.IsTrue(returnBrands.Any(b => b.MarqueID == _defaultBrand1.MarqueId));
        Assert.IsTrue(returnBrands.Any(b => b.MarqueID == _defaultBrand2.MarqueId));
    }
    
    [TestMethod]
    public void ShouldAddBrand()
    {
        //Given
        var newBrandDto = new MarqueDTO
        {
            NomMarque = "Nike"
        };
    
        //When 
        var action = _marqueController.AddMarque(newBrandDto).GetAwaiter().GetResult();
    
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
        var createdResult = action.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(nameof(_marqueController.GetById), createdResult.ActionName);
        Assert.IsInstanceOfType(createdResult.Value, typeof(Marque));
        var returnBrand = createdResult.Value as Marque;
        Assert.IsNotNull(returnBrand);
        Assert.AreEqual(newBrandDto.NomMarque, returnBrand.NomMarque);
        var brandInDb = _context.Marques.FirstOrDefault(m => m.MarqueId == returnBrand.MarqueId);
        Assert.IsNotNull(brandInDb);
        Assert.AreEqual(newBrandDto.NomMarque, brandInDb.NomMarque);
    }
    
    [TestMethod]
    public void ShouldReturnBadRequest_AddBrand_WhenModelStateInvalid()
    {
        //Given
        var invalidBrandDto = new MarqueDTO
        {
            NomMarque = null 
        };
        _marqueController.ModelState.AddModelError("NomMarque", "Required");
    
        //When
        var action = _marqueController.AddMarque(invalidBrandDto).GetAwaiter().GetResult();
    
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(BadRequestObjectResult));
    }
    
    [TestMethod]
    public void ShouldDeleteBrand()
    {
        //Given 
        _context.Marques.Add(_defaultBrand1);
        _context.SaveChanges();
        var brandId = _defaultBrand1.MarqueId;
        
        //When 
        var action = _marqueController.DeleteMarque(brandId).GetAwaiter().GetResult();
        
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        var brandInDb = _context.Marques.FirstOrDefault(m => m.MarqueId == brandId);
        Assert.IsNull(brandInDb);
    }

    [TestMethod]
    public void ShouldReturnNotFound_DeleteBrand_WhenIdDoesNotExist()
    {
        //Given 
        var nonExistentId = 9999;
        
        //When 
        var action = _marqueController.DeleteMarque(nonExistentId).GetAwaiter().GetResult();
        
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    
    [TestMethod]
    public void ShouldUpdateBrand()
    {
        //Given
        _context.Marques.Add(_defaultBrand1);
        _context.SaveChanges();
        var brandId = _defaultBrand1.MarqueId;
        var updatedBrandDto = new MarqueDTO
        {
            MarqueID = brandId,
            NomMarque = "Nike Updated"
        };
        
        //Act
        var action = _marqueController.PutMarque(brandId, updatedBrandDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        var brandInDb = _context.Marques.FirstOrDefault(m => m.MarqueId == brandId);
        Assert.IsNotNull(brandInDb);
        Assert.AreEqual("Nike Updated", brandInDb.NomMarque);
    }

    [TestMethod]
    public void ShouldReturnNotFound_PutBrand_WhenIdDoesNotExist()
    {
        //Given
        var nonExistentId = 9999;
        var brandDto = new MarqueDTO
        {
            MarqueID = nonExistentId,
            NomMarque = "Test Brand"
        };
        
        //Act
        var action = _marqueController.PutMarque(nonExistentId, brandDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldReturnBadRequest_PutBrand_WhenIdsDoNotMatch()
    {
        //Given
        _context.Marques.Add(_defaultBrand1);
        _context.SaveChanges();
        var urlId = _defaultBrand1.MarqueId;
        var brandDto = new MarqueDTO
        {
            MarqueID = urlId + 1,
            NomMarque = "Test Brand"
        };
        
        //Act
        var action = _marqueController.PutMarque(urlId, brandDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
        var brandInDb = _context.Marques.FirstOrDefault(m => m.MarqueId == urlId);
        Assert.IsNotNull(brandInDb);
        Assert.AreEqual(_defaultBrand1.NomMarque, brandInDb.NomMarque);
    }

    [TestMethod]
    public void ShouldGetMarqueByNameReturnAll()
    {
        //Given
        _context.Marques.Add(_defaultBrand1);
        _context.Marques.Add(_defaultBrand2);
        _context.Marques.Add(_defaultBrand3);
        _context.SaveChanges();
        string query = "";
        //Act
        ActionResult<IEnumerable<MarqueDTO>> action = _marqueController.GetMarquesByName(query).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<MarqueDTO>));
        var returnelements = okResult.Value as IEnumerable<MarqueDTO>;
        Assert.IsNotNull(returnelements);
        Assert.AreEqual(returnelements.Count(), 3);
    }
    [TestMethod]
    public void ShouldGetMarqueByNameReturn()
    {
        //Given
        _context.Marques.Add(_defaultBrand1);
        _context.Marques.Add(_defaultBrand2);
        _context.Marques.Add(_defaultBrand3);
        _context.SaveChanges();
        string query = _defaultBrand1.NomMarque;
        //Act
        ActionResult<IEnumerable<MarqueDTO>> action = _marqueController.GetMarquesByName(query).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<MarqueDTO>));
        var returnelements = okResult.Value as IEnumerable<MarqueDTO>;
        Assert.IsNotNull(returnelements);
        Assert.AreEqual(returnelements.Count(), 1);
        Assert.IsTrue(returnelements.Any(b => b.NomMarque == _defaultBrand1.NomMarque));
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