// using System;
// using System.Collections.Generic;
// using System.Security.Claims;
// using System.Threading.Tasks;
// using API.Controllers;
// using API.Mapper;
// using API.Models;
// using API.Models.EntityFramework;
// using API.Models.Repository.Managers;
// using API.Services;
// using AutoMapper;
// using JetBrains.Annotations;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
//
// namespace API.Tests.Controllers;
//
// [TestClass]
// [TestSubject(typeof(LoginController))]
// [TestCategory("integration")]
// public class LoginControllerTest
// {
//     
//     private Clothes2UDbContext _context;
//     private LoginController _controller;
//     private IMapper _mapper;
//     private ILoginService _loginService;
//     
//     private Utilisateur _default1;
//
//     [TestInitialize]
//     public void Setup()
//     {
//         var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
//
//         _context = new Clothes2UDbContext(builder.Options);
//         
//         InitializeDefaultLogin();
//         
//         var manager = new UtilisateurManager(_context);
//
//         var config = new MapperConfiguration(cfg =>
//         {
//             cfg.AddProfile<GenericProfile>();
//         });
//         IMapper mapper = config.CreateMapper();
//         _mapper = config.CreateMapper();
//         
//         var fakeConfig = new ConfigurationBuilder()
//             .AddInMemoryCollection(new Dictionary<string, string>
//             {
//                 {"Jwt:Key", "lYSbXvvIGcqs4LR0P7p7ZhTqNidibdWCTvCv9oNpIatOszxo4s825RZB1wN3FRn"},
//                 {"Jwt:Issuer", "http://localhost:5096"},
//                 {"Jwt:Audience", "http://localhost:5096"}
//             })
//             .Build();
//
//         _loginService = new LoginService(fakeConfig);
//         
//         _controller = new LoginController(fakeConfig, _mapper, manager, _loginService);
//         
//         var mockHttpContext = new DefaultHttpContext();
//         _controller.ControllerContext = new ControllerContext()
//         {
//             HttpContext = mockHttpContext
//         };
//     }
//
//     private void InitializeDefaultLogin()
//     {
//         _default1 = new Utilisateur
//         {
//             UtilisateurId = 1,
//             Login = "TestUser",
//             Email = "test@example.com",
//             Password = BCrypt.Net.BCrypt.HashPassword("password"),
//             Description = "Test User",
//             Telephone = "0777777777"
//             // PhotoId = 1,
//             // PhotoProfil = _defaultPhoto
//         };
//     }
//
//     [TestMethod]
//     public async Task ShouldLoginWithLoginAndPassword()
//     {
//         //Arrange
//         _context.Utilisateurs.Add(_default1);
//         _context.SaveChanges();
//         LoginRequest loginRequest = new LoginRequest
//         {
//             Login = _default1.Login,
//             Password = "password",
//         };
//         
//         //Act
//         var result = await _controller.Login(loginRequest);
//         
//         //Assert
//         
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(OkObjectResult));
//         var okResult = result as OkObjectResult;
//         Assert.IsNotNull(okResult);
//         var utilisateur = okResult.Value as Utilisateur;
//         Assert.IsNotNull(utilisateur);
//         Assert.AreEqual(_default1.UtilisateurId, utilisateur.UtilisateurId);
//
//     }
//
//     [TestMethod]
//     public async Task ShouldLoginWithEmailAndPassword()
//     {
//         //Arrange
//         _context.Utilisateurs.Add(_default1);
//         _context.SaveChanges();
//         LoginRequest loginRequest = new LoginRequest
//         {
//             Login = _default1.Email,
//             Password = "password",
//         };
//         
//         //Act
//         var result = await _controller.Login(loginRequest);
//         
//         //Assert
//         
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(OkObjectResult));
//         var okResult = result as OkObjectResult;
//         Assert.IsNotNull(okResult);
//         var utilisateur = okResult.Value as Utilisateur;
//         Assert.IsNotNull(utilisateur);
//         Assert.AreEqual(_default1.UtilisateurId, utilisateur.UtilisateurId);
//     }
//
//     [TestMethod]
//     public async Task ShouldNotLoginCauseIncorrectLogin()
//     {
//         //Arrange
//         _context.Utilisateurs.Add(_default1);
//         _context.SaveChanges();
//         LoginRequest loginRequest = new LoginRequest
//         {
//             Login = "fakeLogin",
//             Password = "password",
//         };
//         
//         //Act
//         var result = await _controller.Login(loginRequest);
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
//         var unauthorizedObjectResult = result as UnauthorizedObjectResult;
//         Assert.IsNotNull(unauthorizedObjectResult);
//         Assert.AreEqual("Utilisateur inconnu.", unauthorizedObjectResult.Value);
//     }
//     
//     [TestMethod]
//     public async Task ShouldNotLoginCauseIncorrectEmail()
//     {
//         //Arrange
//         _context.Utilisateurs.Add(_default1);
//         _context.SaveChanges();
//         LoginRequest loginRequest = new LoginRequest
//         {
//             Login = "fake@email.com",
//             Password = "password",
//         };
//         
//         //Act
//         var result = await _controller.Login(loginRequest);
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
//         var unauthorizedObjectResult = result as UnauthorizedObjectResult;
//         Assert.IsNotNull(unauthorizedObjectResult);
//         Assert.AreEqual("Utilisateur inconnu.", unauthorizedObjectResult.Value);
//     }
//
//     [TestMethod]
//     public async Task ShouldNotLoginCauseIncorrectPassword()
//     {
//         //Arrange
//         _context.Utilisateurs.Add(_default1);
//         _context.SaveChanges();
//         LoginRequest loginRequest = new LoginRequest
//         {
//             Login = _default1.Login,
//             Password = "fakepassword",
//         };
//         
//         //Act
//         var result = await _controller.Login(loginRequest);
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
//         var unauthorizedObjectResult = result as UnauthorizedObjectResult;
//         Assert.IsNotNull(unauthorizedObjectResult);
//         Assert.AreEqual("Votre mot de passe est incorrect.", unauthorizedObjectResult.Value);
//     }
//
//     [TestMethod]
//     public async Task ShouldNotLoginCauseEmptyLogin()
//     {
//         //Arrange
//         _context.Utilisateurs.Add(_default1);
//         _context.SaveChanges();
//         LoginRequest loginRequest = new LoginRequest
//         {
//             Login = "",
//             Password = "password",
//         };
//         
//         //Act
//         var result = await _controller.Login(loginRequest);
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
//         var badRequestResult = result as BadRequestObjectResult;
//         Assert.IsNotNull(badRequestResult);
//         Assert.AreEqual("Email ou login obligatoires.", badRequestResult.Value);
//     }
//
//     [TestMethod]
//     public async Task ShouldSignUp()
//     {
//         //Arrange
//         LoginRequest loginRequest = new LoginRequest
//         {
//             Email = "test@gmail.com",
//             Login = "test",
//             Password = "Qwerty123!",
//             PasswordConfirm = "Qwerty123!"
//         };
//         
//         //Act
//         var result = await _controller.SignUp(loginRequest);
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(OkObjectResult));
//         var okResult = result as OkObjectResult;
//         Assert.IsNotNull(okResult);
//         var utilisateur = okResult.Value as Utilisateur;
//         Assert.IsNotNull(utilisateur);
//         Assert.AreEqual(loginRequest.Email, utilisateur.Email);
//     }
//
//     [TestMethod]
//     public async Task ShouldNotSignUpCauseNoLogin()
//     {
//         //Arrange
//         LoginRequest loginRequest = new LoginRequest
//         {
//             Email = "test@example.com",
//             Login = "",
//             Password = "Qwerty123!",
//             PasswordConfirm = "Qwerty123!"
//         };
//         
//         //Act
//         var result = await _controller.SignUp(loginRequest);
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
//         var badRequestObject = result as BadRequestObjectResult;
//         Assert.IsNotNull(badRequestObject);
//         Assert.AreEqual("Login obligatoires.", badRequestObject.Value);
//     }   
//     
//     [TestMethod]
//     public async Task ShouldNotSignUpCauseNoEmail()
//     {
//         //Arrange
//         LoginRequest loginRequest = new LoginRequest
//         {
//             Email = "",
//             Login = "test",
//             Password = "Qwerty123!",
//             PasswordConfirm = "Qwerty123!"
//         };
//         
//         //Act
//         var result = await _controller.SignUp(loginRequest);
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
//         var badRequestObject = result as BadRequestObjectResult;
//         Assert.IsNotNull(badRequestObject);
//         Assert.AreEqual("Email obligatoire.", badRequestObject.Value);
//     }   
//     
//     [TestMethod]
//     public async Task ShouldNotSignUpCauseNoPasswordConfirmation()
//     {
//         //Arrange
//         LoginRequest loginRequest = new LoginRequest
//         {
//             Email = "test@example.com",
//             Login = "test",
//             Password = "Qwerty123!",
//             PasswordConfirm = ""
//         };
//         
//         //Act
//         var result = await _controller.SignUp(loginRequest);
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
//         var badRequestObject = result as BadRequestObjectResult;
//         Assert.IsNotNull(badRequestObject);
//         Assert.AreEqual("Confirmation de mot de passe obligatoire.", badRequestObject.Value);
//     }
//     
//     [TestMethod]
//     public async Task ShouldNotSignUpCauseNoPasswordConfirmationNotValid()
//     {
//         //Arrange
//         LoginRequest loginRequest = new LoginRequest
//         {
//             Email = "test@example.com",
//             Login = "test",
//             Password = "Qwerty123!",
//             PasswordConfirm = "Azerty123!"
//         };
//         
//         //Act
//         var result = await _controller.SignUp(loginRequest);
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
//         var badRequestObject = result as BadRequestObjectResult;
//         Assert.IsNotNull(badRequestObject);
//         Assert.AreEqual("Les mots de passe ne correspondent pas", badRequestObject.Value);
//     }
//     
//     [TestMethod]
//     public async Task ShouldNotSignUpCauseEmailExist()
//     {
//         //Arrange
//         _context.Utilisateurs.Add(_default1);
//         _context.SaveChanges();
//         LoginRequest loginRequest = new LoginRequest
//         {
//             Email = "test@example.com",
//             Login = "test",
//             Password = "Qwerty123!",
//             PasswordConfirm = "Qwerty123!"
//         };
//         
//         //Act
//         var result = await _controller.SignUp(loginRequest);
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
//         var badRequestObject = result as BadRequestObjectResult;
//         Assert.IsNotNull(badRequestObject);
//         Assert.AreEqual("Cet email est déjà utilisé.", badRequestObject.Value);
//     }
//     
//     [TestMethod]
//     public async Task ShouldNotSignUpCauseLoginExist()
//     {
//         //Arrange
//         _context.Utilisateurs.Add(_default1);
//         _context.SaveChanges();
//         LoginRequest loginRequest = new LoginRequest
//         {
//             Email = "test2@example.com",
//             Login = _default1.Login,
//             Password = "Qwerty123!",
//             PasswordConfirm = "Qwerty123!"
//         };
//         
//         //Act
//         var result = await _controller.SignUp(loginRequest);
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
//         var badRequestObject = result as BadRequestObjectResult;
//         Assert.IsNotNull(badRequestObject);
//         Assert.AreEqual("Ce login est déjà utilisé.", badRequestObject.Value);
//     }
//
//     [TestMethod]
//     public void ShouldLogout()
//     {
//         _controller.ControllerContext.HttpContext.Response.Cookies.Append("authToken", "fakeToken");
//         
//         //Act
//         var result = _controller.Logout();
//
//         // Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(OkObjectResult));
//         var okResult = result as OkObjectResult;
//         Assert.AreEqual("Déconnexion réussie", okResult.Value);
//     }
//
//     [TestMethod]
//     public void ShouldGetCurrentUser()
//     {
//         //Arrange
//         _context.Utilisateurs.Add(_default1);
//         _context.SaveChanges();
//         // Simuler un utilisateur authentifié
//         var realId = _default1.UtilisateurId;
//         var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
//         {
//             new Claim("userId", realId.ToString()),
//         }, "TestAuthentication"));
//
//         _controller.ControllerContext.HttpContext.User = user;
//
//         //Act
//         var result = _controller.GetCurrentUser().GetAwaiter().GetResult();
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(OkObjectResult));
//         var okResult = result as OkObjectResult;
//         Assert.IsNotNull(okResult);
//         var utilisateur = okResult.Value as Utilisateur;
//         Assert.IsNotNull(utilisateur);
//         Assert.AreEqual(_default1.UtilisateurId, utilisateur.UtilisateurId);
//     }
//
//     [TestMethod]
//     public async Task ShouldGetUnauthorized_GetCurrentUser()
//     {
//         //Act
//         var result = await _controller.GetCurrentUser();
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
//     }
//
//     [TestMethod]
//     public async Task ShouldGetNotFound_GetCurrentUser()
//     {
//         //Arrange
//         var claims = new List<Claim>
//         {
//             new Claim("userId", _default1.UtilisateurId.ToString()),
//             new Claim("login", _default1.Login)
//         };
//
//         var identity = new ClaimsIdentity(claims, "TestAuth");
//         var principal = new ClaimsPrincipal(identity);
//
//         _controller.ControllerContext.HttpContext.User = principal;
//         
//         //Act
//         var result = await _controller.GetCurrentUser();
//         
//         //Assert
//         Assert.IsNotNull(result);
//         Assert.IsInstanceOfType(result, typeof(NotFoundResult));
//     }
// }