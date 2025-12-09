using System;
using System.IO;
using System.Threading.Tasks;
using API.Controllers;
using API.DTO;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using API.Services;
using AutoMapper;
//sing Castle.Components.DictionaryAdapter.Xml;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace API.Tests.Controllers;

[TestClass]
[TestSubject(typeof(MediasController))]
[TestCategory("integration")]
public class MediasControllerTest
{
    private Clothes2UDbContext _context;
    private MediasController _controller;
    private IPhotoService _service;
    private IMapper _mapper;
    
    private Photo _photo1, _photo2, _photo3;

    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        
        _context = new Clothes2UDbContext(builder.Options);
        
        var config = new MapperConfiguration(cfg =>
        {
            //cfg.AddProfile<GenericProfile>();
        });
        
        _mapper = config.CreateMapper();

        InitializeDefaultMedias();

        var photomanager = new PhotoManager(_context);
        var annoncemanager = new AnnonceManager(_context);
        var illustreannoncemanager = new IllustreAnnonceManager(_context);
        var utilisateurmanager = new UtilisateurManager(_context);
        
        _service = new PhotoService(photomanager, annoncemanager, illustreannoncemanager, utilisateurmanager, _context);
        
        _controller = new MediasController(_service);
    }

    private void InitializeDefaultMedias()
    {
        _photo1 = new Photo
        {
            PhotoId = 1,
            Image = new byte[] { 255, 216, 255, 224, 0, 16, 74, 70, 73, 70 },
        };

        _photo2 = new Photo
        {
            PhotoId = 2,
            Image = new byte[] { 255, 216, 255, 224, 0, 16, 74, 70, 73, 70 },
        };

        _photo3 = new Photo
        {
            PhotoId = 3,
            Image = new byte[] { 255, 216, 255, 224, 0, 16, 74, 70, 73, 70 },
        };
        
    }

    [TestMethod]
    public async Task ShouldGetPhotosById()
    {
        //Arrange
        _context.Photos.AddRange(new [] {_photo1, _photo2, _photo3});
        _context.SaveChanges();
        
        //Act
        var result = await _controller.GetPhotos(_photo2.PhotoId);
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(FileResult));
        
        var fielResult = result as FileResult;
        Assert.IsNotNull(fielResult);
        Assert.AreEqual("image/jpeg", fielResult.ContentType);
    }

    [TestMethod]
    public async Task ShouldNotGetPhotosCauseIdDoesNotExist()
    {
        //Arrange
        
        //Act
        var result = await _controller.GetPhotos(0);
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult)); ;
        var notFoundObjectResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundObjectResult);
        Assert.AreEqual("Photo 0 introuvable", notFoundObjectResult.Value);
    }

    [TestMethod]
    public async Task ShouldUploadPhotoAnnonce()
    {
        //Arrange
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        
        var annonce = new Annonce
        {
            AnnonceId = 1,
            UtilisateurId = 1,
            Title = "Test annonce",
            Description = "Annonce pour test upload photo",
            Prix = 10,
            DateAnnonce = DateTime.Now
        };
        _context.Annonces.Add(annonce);
        _context.SaveChanges();
        
        var fileBytes = new byte[] { 255, 216, 255, 224, 0, 16, 74, 70, 73, 70 };

        var stream = new MemoryStream(fileBytes);

        var formFile = new FormFile(stream, 0, fileBytes.Length, "file", "testfile.jpeg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
        
        
        PhotoDTO photoDto = new PhotoDTO
        {
            PhotoId = 1000,
            File = formFile
        };
        
        //Act
        var result = await _controller.UploadPhotoAnnonce(photoDto, annonce.AnnonceId);
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(FileResult));

        var fileResult = result as FileResult;
        Assert.IsNotNull(fileResult);
        Assert.AreEqual("image/jpeg", fileResult.ContentType);
    }

    [TestMethod]
    public async Task ShouldNotUploadPhotoAnnonceCauseNoFile()
    {
        //Arrange
        var annonce = new Annonce
        {
            AnnonceId = 1,
            UtilisateurId = 1,
            Title = "Test annonce",
            Description = "Annonce pour test upload photo",
            Prix = 10,
            DateAnnonce = DateTime.Now
        };
        _context.Annonces.Add(annonce);
        _context.SaveChanges();
        
        PhotoDTO photoDto = new PhotoDTO
        {
            File = null
        };
        
        //Act
        var result = await _controller.UploadPhotoAnnonce(photoDto, annonce.AnnonceId);
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var badRequestObjectResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestObjectResult);
        Assert.AreEqual("Fichier requis", badRequestObjectResult.Value);
    }
    
    [TestMethod]
    public async Task ShouldNotUploadPhotoAnnonceCauseAnnonceDoesNotExist()
    {
        //Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        
        var fileBytes = new byte[] { 255, 216, 255, 224, 0, 16, 74, 70, 73, 70 };

        var stream = new MemoryStream(fileBytes);

        var formFile = new FormFile(stream, 0, fileBytes.Length, "file", "testfile.jpeg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
        
        PhotoDTO photoDto = new PhotoDTO
        {
            PhotoId = 1000,
            File = formFile
        };
        
        //Act
        var result = await _controller.UploadPhotoAnnonce(photoDto, 1);
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        var notFoundResult = result as NotFoundResult;
        Assert.IsNotNull(notFoundResult);
    }
    
    [TestMethod]
    public async Task ShouldNotUploadPhotoCompteCauseNoFile()
    {
        //Arrange
        var untilisateur = new Utilisateur
        {
            UtilisateurId = 1,
            Login = "test",
            Email = "Email@gmail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("password"),
            Description = "Test User",
        };
        _context.Utilisateurs.Add(untilisateur);
        _context.SaveChanges();
        
        PhotoDTO photoDto = new PhotoDTO
        {
            File = null
        };
        
        //Act
        var result = await _controller.UploadPhotoAnnonce(photoDto, untilisateur.UtilisateurId);
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var badRequestObjectResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestObjectResult);
        Assert.AreEqual("Fichier requis", badRequestObjectResult.Value);
    }

    [TestMethod]
    public async Task ShouldDeletePhoto()
    {
        //Arrange
        _context.Photos.Add(_photo1);
        _context.SaveChanges();
        
        //Act
        var result = await _controller.DeletePhoto(_photo1.PhotoId);
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }
    
    [TestMethod]
    public async Task ShouldNotDeletePhotoCauseIdDoesNotExist()
    {
        //Arrange
        int id = 100;
        
        //Act
        var result = await _controller.DeletePhoto(id);
        
        //Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }
}