// using System;
// using System.Collections.Generic;
// using System.Linq;
// using API.Controllers;
// using API.DTO;
// using API.DTO.Annonce;
// using API.Mapper;
// using API.Models;
// using API.Models.EntityFramework;
// using API.Models.Repository.Managers;
// using AutoMapper;
// using JetBrains.Annotations;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
//
// namespace API.Tests.Controllers;
//
// [TestClass]
// [TestSubject(typeof(MarqueController))]
// [TestCategory("integration")]
// public class MarqueControllerTest
// {
//     private MarqueController _controller;
//     private IMapper _mapper;
//     private Clothes2UDbContext _context;
//     private Marque _default1 ,_default2, _default3;
//
//     [TestInitialize]
//     public void Setup()
//     {
//         var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
//         
//         _context = new Clothes2UDbContext(builder.Options);
//         
//         InitializeDefaultMarques();
//         var config = new MapperConfiguration(cfg =>
//         {
//             //cfg.AddProfile<GenericProfile>();
//         });
//         IMapper mapper = config.CreateMapper();
//         _mapper = config.CreateMapper();
//         
//         var manager = new MarqueManager(_context);
//         
//         _controller = new MarqueController(manager, _mapper);
//         
//     }
//
//     public void InitializeDefaultMarques()
//     {
//         _default1 = new Marque
//         {
//             MarqueId = 1,
//             NomMarque = "Nike"
//         };
//         _default2 = new Marque
//         {
//             MarqueId = 2,
//             NomMarque = "Adidas"
//         };
//         _default3 = new Marque
//         {
//             MarqueId = 3,
//             NomMarque = "Chrome Hearts"
//         };
//     }
//     
//     [TestMethod]
//     public void ShouldGetAllMarques()
//     {
//         //Arrange
//         _context.Marques.AddRange(_default1, _default2, _default3);
//         _context.SaveChanges();
//         
//         //Act
//         var result = _controller.GetAllMarques();
//         
//         //Assert
//         Assert.IsNotNull(result.Result);
//         var okResult = result.Result as ActionResult<MarqueDTO>;
//          Assert.IsNotNull(okResult);
//         
//          var okObject = okResult.Result as OkObjectResult;
//          Assert.IsNotNull(okObject);
//          var annonces = okObject.Value as IEnumerable<MarqueDTO>;
//          Assert.IsNotNull(annonces);
//          Assert.AreEqual(3, annonces.Count());
//     }
//     
//     [TestMethod]
//     public void ShouldGetMarqueById()
//     {
//         //Arrange
//         _context.Marques.AddRange(_default1, _default2, _default3);
//         _context.SaveChanges();
//         
//         //Act
//         var result = _controller.GetById(_default3.MarqueId);
//         
//         //Assert
//         Assert.IsNotNull(result.Result);
//         var okResult = result.Result as ActionResult<Marque>;
//         Assert.IsNotNull(okResult);
//         
//         var okObject = okResult.Result as OkObjectResult;
//         Assert.IsNotNull(okObject);
//         var annonces = okObject.Value as Marque;
//         Assert.IsNotNull(annonces);
//         Assert.AreEqual("Chrome Hearts", annonces.NomMarque);
//     }
//     
//     [TestMethod]
//     public void ShouldReturnNotFound_GetById()
//     {
//         // Arrange
//         var id = 12;
//         
//         // Act
//         var result =  _controller.GetById(12).GetAwaiter().GetResult();
//         
//         // Assert
//         Assert.IsNotNull(result.Result);
//         Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
//     }
//
//     [TestMethod]
//     public void ShouldAddAnnonce()
//     {
//         //Arrange
//         
//         //Act
//         var result = _controller.AddMarque(_default1).GetAwaiter().GetResult();
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(ActionResult<Marque>));
//         var createdAtAction = result.Result as CreatedAtActionResult;
//         Assert.IsNotNull(createdAtAction);
//         Assert.AreEqual("GetById", createdAtAction.ActionName);
//         var value = createdAtAction.Value as Marque;
//         Assert.IsNotNull(value);
//         Assert.AreEqual(_default1.MarqueId, value.MarqueId);
//     }
//
//     [TestMethod]
//     public void ShouldReturnBadRequest_AddAnnonce()
//     {
//         //Arrange
//         Marque marque = new Marque
//         {
//             MarqueId = 1,
//             NomMarque = null
//         };
//         _controller.ModelState.AddModelError("NomMarque", "Required");
//         //Act
//         var result = _controller.AddMarque(marque).GetAwaiter().GetResult();
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
//     }
//
//     [TestMethod]
//     public void ShouldDeleteMarque()
//     {
//         //Arrange
//         _context.Marques.Add(_default1);
//         _context.SaveChanges();
//         
//         //Act
//         var result = _controller.DeleteMarque(_default1.MarqueId).GetAwaiter().GetResult();
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(NoContentResult));
//     }
//
//     [TestMethod]
//     public void ShouldNotDeleteMarqueCauseIdDoesNotExist()
//     {
//         //Arrange
//         var id = 1;
//         
//         //Act
//         var result = _controller.DeleteMarque(id).GetAwaiter().GetResult();
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(NotFoundResult));
//     }
//
//     [TestMethod]
//     public void ShouldPutMarque()
//     {
//         //Arrange
//         _context.Marques.Add(_default1);
//         _context.SaveChanges();
//
//         Marque marque = new Marque
//         {
//             MarqueId = 1,
//             NomMarque = "new"
//         };
//         
//         //Act
//         var result = _controller.PutMarque(_default1.MarqueId, marque).GetAwaiter().GetResult();
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(NoContentResult));
//         var newmarque = _context.Marques.FirstOrDefault(m => m.MarqueId == _default1.MarqueId);
//         Assert.IsNotNull(newmarque);
//         Assert.AreEqual("new", newmarque.NomMarque);
//     }
//
//     [TestMethod]
//     public void ShouldNotPutMarqueCauseIdDoesNotExist()
//     {
//         //Arrange
//         Marque marque = new Marque
//         {
//             MarqueId = 1,
//             NomMarque = "new"
//         };
//         
//         //Act
//         var result = _controller.PutMarque(1, marque).GetAwaiter().GetResult();
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(NotFoundResult));
//     }
//
//     [TestMethod]
//     public void ShouldNotPutMarqueCauseIdAreNotTheSame()
//     {
//         //Arrange
//         _context.Marques.Add(_default1);
//         _context.SaveChanges();
//         Marque marque = new Marque
//         {
//             MarqueId = 2,
//             NomMarque = "new"
//         };
//         
//         //Act
//         var result = _controller.PutMarque(_default1.MarqueId, marque).GetAwaiter().GetResult();
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(BadRequestResult));
//     }
// }

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
        Assert.IsTrue(returnBrand.MarqueId > 0);
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
    public void ShouldDeleteOnlySpecifiedBrand()
    {
        //Given 
        _context.Marques.AddRange(new []{_defaultBrand1, _defaultBrand2});
        _context.SaveChanges();
        var brandToDeleteId = _defaultBrand1.MarqueId;
        var brandToKeepId = _defaultBrand2.MarqueId;
        
        //When 
        var action = _marqueController.DeleteMarque(brandToDeleteId).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        var deletedBrand = _context.Marques.FirstOrDefault(m => m.MarqueId == brandToDeleteId);
        Assert.IsNull(deletedBrand);
        var remainingBrand = _context.Marques.FirstOrDefault(m => m.MarqueId == brandToKeepId);
        Assert.IsNotNull(remainingBrand);
        Assert.AreEqual(_defaultBrand2.NomMarque, remainingBrand.NomMarque);
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
    public void ShouldUpdateOnlySpecifiedBrand()
    {
        //Given
        _context.Marques.AddRange(new []{_defaultBrand1, _defaultBrand2});
        _context.SaveChanges();
        var brandToUpdateId = _defaultBrand1.MarqueId;
        var originalName2 = _defaultBrand2.NomMarque;
        var updatedBrandDto = new MarqueDTO
        {
            MarqueID = brandToUpdateId,
            NomMarque = "Updated Brand"
        };
        
        //Act
        var action = _marqueController.PutMarque(brandToUpdateId, updatedBrandDto).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        var updatedBrand = _context.Marques.FirstOrDefault(m => m.MarqueId == brandToUpdateId);
        Assert.IsNotNull(updatedBrand);
        Assert.AreEqual("Updated Brand", updatedBrand.NomMarque);
        var unchangedBrand = _context.Marques.FirstOrDefault(m => m.MarqueId == _defaultBrand2.MarqueId);
        Assert.IsNotNull(unchangedBrand);
        Assert.AreEqual(originalName2, unchangedBrand.NomMarque);
    }
    [TestMethod]
    public void ShouldGetAllBrands_WhenNameIsNull()
    {
        //Given
        _context.Marques.AddRange(new []{_defaultBrand1, _defaultBrand2});
        _context.SaveChanges();
        
        //Act
        var action = _marqueController.GetMarquesByName(null).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var returnBrands = okResult.Value as IEnumerable<MarqueDTO>;
        Assert.IsNotNull(returnBrands);
        Assert.AreEqual(2, returnBrands.Count());
    }

    [TestMethod]
    public void ShouldGetAllBrands_WhenNameIsEmpty()
    {
        //Given
        _context.Marques.AddRange(new []{_defaultBrand1, _defaultBrand2});
        _context.SaveChanges();
        
        //Act
        var action = _marqueController.GetMarquesByName("").GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var returnBrands = okResult.Value as IEnumerable<MarqueDTO>;
        Assert.IsNotNull(returnBrands);
        Assert.AreEqual(2, returnBrands.Count());
    }

    [TestMethod]
    public void ShouldGetAllBrands_WhenNameIsWhitespace()
    {
        //Given
        _context.Marques.AddRange(new []{_defaultBrand1, _defaultBrand2});
        _context.SaveChanges();
        
        //Act
        var action = _marqueController.GetMarquesByName("   ").GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var returnBrands = okResult.Value as IEnumerable<MarqueDTO>;
        Assert.IsNotNull(returnBrands);
        Assert.AreEqual(2, returnBrands.Count());
    }

    [TestMethod]
    public void ShouldGetBrandsByName_WhenNameIsProvided()
    {
        //Given
        _context.Marques.AddRange(new []{_defaultBrand1, _defaultBrand2});
        _context.SaveChanges();
        
        //Act
        var action = _marqueController.GetMarquesByName("marque1").GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var returnBrands = okResult.Value as IEnumerable<MarqueDTO>;
        Assert.IsNotNull(returnBrands);
        Assert.IsTrue(returnBrands.Any(b => b.NomMarque == "marque1"));
    }

    [TestMethod]
    public void ShouldReturnEmptyList_WhenNoMatchingBrands()
    {
        //Given
        _context.Marques.AddRange(new []{_defaultBrand1, _defaultBrand2});
        _context.SaveChanges();
        
        //Act
        var action = _marqueController.GetMarquesByName("NonExistentBrand").GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var returnBrands = okResult.Value as IEnumerable<MarqueDTO>;
        Assert.IsNotNull(returnBrands);
        Assert.AreEqual(0, returnBrands.Count());
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