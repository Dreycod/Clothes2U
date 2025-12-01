using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Controllers;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace API.Tests.Controllers;

// public class LoginRequest
// {
//     public string? Login { get; set; }
//     public string? Email { get; set; }
//     public string Password { get; set; }
//     public string? PasswordConfirm { get; set; }
// }

[TestClass]
[TestSubject(typeof(LoginController))]
[TestCategory("integration")]
public class LoginControllerTest
{
    
    private Clothes2UDbContext _context;
    private LoginController _controller;
    private IMapper _mapper;
    
    private Utilisateur _default1;

    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

        _context = new Clothes2UDbContext(builder.Options);
        
        InitializeDefaultLogin();
        
        var manager = new UtilisateurManager(_context);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GenericProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();
        
        var fakeConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"Jwt:Key", "lYSbXvvIGcqs4LR0P7p7ZhTqNidibdWCTvCv9oNpIatOszxo4s825RZB1wN3FRn"},
                {"Jwt:Issuer", "http://localhost:5096"},
                {"Jwt:Audience", "http://localhost:5096"}
            })
            .Build();
        
        _controller = new LoginController(fakeConfig, _mapper, manager);
        
        var mockHttpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = mockHttpContext
        };
    }

    private void InitializeDefaultLogin()
    {
        _default1 = new Utilisateur
        {
            UtilisateurId = 1,
            Login = "TestUser",
            Email = "test@example.com",
            Password = BCrypt.Net.BCrypt.HashPassword("password"),
            Description = "Test User",
            // PhotoId = 1,
            // PhotoProfil = _defaultPhoto
        };
    }

    [TestMethod]
    public async Task ShouldLoginWithEmailAndPassword()
    {
        //Arrange
        _context.Utilisateurs.Add(_default1);
        _context.SaveChanges();
        LoginRequest loginRequest = new LoginRequest
        {
            Login = _default1.Login,
            Password = "password",
        };
        
        //Act
        var result = await _controller.Login(loginRequest);
        
        //Assert
        
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var utilisateur = okResult.Value as Utilisateur;
        Assert.IsNotNull(utilisateur);
        Assert.AreEqual(_default1.UtilisateurId, utilisateur.UtilisateurId);

    }
}