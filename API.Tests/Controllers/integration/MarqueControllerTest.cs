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