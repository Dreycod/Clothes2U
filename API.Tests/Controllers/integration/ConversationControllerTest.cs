// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Security.Cryptography;
// using System.Threading.Tasks;
// using API.Controllers;
// using API.DTO.Conversation;
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
// [TestSubject(typeof(ConversationController))]
// [TestCategory("integration")]
// public class ConversationControllerTest
// {
//
//     private Clothes2UDbContext _context;
//     private ConversationController _controller;
//     private IMapper _mapper;
//
//     private Conversation _default1, _default2;
//     private StatutConversation _statutNegociation, _statutAcceptation;
//     private Utilisateur _acheteur1, _acheteur2, _vendeur1;
//     private Annonce _annonce1, _annonce2;
//     private Conversation _conversation1, _conversation2, _conversation3;
//     private Achete _achete1, _achete2, _achete3;
//     private Vend _vend1, _vend2, _vend3;
//     private Message _message1_1, _message1_2, _message1_3;
//     private MessageTexte _messageTexte1_1, _messageTexte1_2, _messageTexte1_3;
//     private Message _message2_1, _message2_2, _message2_3, _message2_4;
//     private MessageTexte _messageTexte2_1, _messageTexte2_2, _messageTexte2_3, _messageTexte2_4;
//     private Message _message3_1, _message3_2;
//     private MessageTexte _messageTexte3_1, _messageTexte3_2;
//
//     [TestInitialize]
//     public void Setup()
//     {
//         var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
//
//         _context = new Clothes2UDbContext(builder.Options);
//
//         InitializeDefaultConversations();
//
//         var manager = new ConversationManager(_context);
//
//         var config = new MapperConfiguration(cfg => { cfg.AddProfile<GenericProfile>(); });
//         IMapper mapper = config.CreateMapper();
//         _mapper = config.CreateMapper();
//
//         _controller = new ConversationController(manager, _mapper);
//     }
//
//     public void InitializeDefaultConversations()
//     {
//         // Statuts de conversation
//         _statutNegociation = new StatutConversation
//         {
//             StatutConversationId = 1,
//             StatutConversationLibelle = "En négociation"
//         };
//
//         _statutAcceptation = new StatutConversation
//         {
//             StatutConversationId = 2,
//             StatutConversationLibelle = "Acceptation"
//         };
//     
//
//     // Utilisateurs
//         _vendeur1 = new Utilisateur
//         {
//             UtilisateurId = 1,
//             Login = "vendeur1",
//             Email = "vendeur1@test.com",
//             Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
//             Description = "Vendeur test 1",
//             Dateinscription = DateTime.UtcNow.AddMonths(-6),
//             StatutId = 1,
//             RoleId = 1
//         };
//
//         _acheteur1 = new Utilisateur
//         {
//             UtilisateurId = 2,
//             Login = "acheteur1",
//             Email = "acheteur1@test.com",
//             Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
//             Description = "Acheteur test 1",
//             Dateinscription = DateTime.UtcNow.AddMonths(-3),
//             StatutId = 1,
//             RoleId = 1
//         };
//
//         _acheteur2 = new Utilisateur
//         {
//             UtilisateurId = 3,
//             Login = "acheteur2",
//             Email = "acheteur2@test.com",
//             Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
//             Description = "Acheteur test 2",
//             Dateinscription = DateTime.UtcNow.AddMonths(-2),
//             StatutId = 1,
//             RoleId = 1
//         };
//
//         // Annonces
//         _annonce1 = new Annonce
//         {
//             AnnonceId = 1,
//             Title = "T-Shirt Nike Neuf",
//             Description = "Un T-shirt Nike neuf de taille M",
//             DateAnnonce = DateTime.UtcNow.AddDays(-10),
//             Negociable = true,
//             Prix = 25.00m,
//             UtilisateurId = 1,
//             EtatId = 1,
//             MarqueId = 1,
//             TailleId = 1,
//             SousCategorieId = 1,
//             CategorieId = 1,
//             StatutAnnonceId = 1
//         };
//
//         _annonce2 = new Annonce
//         {
//             AnnonceId = 2,
//             Title = "Baskets Adidas",
//             Description = "Baskets Adidas en très bon état",
//             DateAnnonce = DateTime.UtcNow.AddDays(-15),
//             Negociable = true,
//             Prix = 60.00m,
//             UtilisateurId = 1,
//             EtatId = 2,
//             MarqueId = 2,
//             TailleId = 10,
//             SousCategorieId = 15,
//             CategorieId = 3,
//             StatutAnnonceId = 1
//         };
//
//         // Conversation 1 : Avec 3 messages
//         _conversation1 = new Conversation
//         {
//             ConversationId = 1,
//             CreationDate = DateTime.UtcNow.AddDays(-5),
//             AnnonceId = 1,
//             StatutConversationId = 1
//         };
//
//         _achete1 = new Achete
//         {
//             UtilisateurAcheteurId = 2,
//             ConversationId = 1
//         };
//
//         _vend1 = new Vend
//         {
//             UtilisateurVendeurId = 1,
//             ConversationId = 1
//         };
//
//         _message1_1 = new Message
//         {
//             MessageId = 1,
//             MessageDate = DateTime.UtcNow.AddDays(-5).AddHours(-2),
//             MessageLu = true,
//             UtilisateurId = 2,
//             ConversationId = 1
//         };
//
//         _messageTexte1_1 = new MessageTexte
//         {
//             MessageTexteId = 1,
//             Content = "Bonjour, est-ce que le T-shirt est toujours disponible ?",
//             MessageId = 1
//         };
//
//         _message1_2 = new Message
//         {
//             MessageId = 2,
//             MessageDate = DateTime.UtcNow.AddDays(-5).AddHours(-1),
//             MessageLu = true,
//             UtilisateurId = 1,
//             ConversationId = 1
//         };
//
//         _messageTexte1_2 = new MessageTexte
//         {
//             MessageTexteId = 2,
//             Content = "Oui, il est toujours disponible !",
//             MessageId = 2
//         };
//
//         _message1_3 = new Message
//         {
//             MessageId = 3,
//             MessageDate = DateTime.UtcNow.AddDays(-4),
//             MessageLu = false,
//             UtilisateurId = 2,
//             ConversationId = 1
//         };
//
//         _messageTexte1_3 = new MessageTexte
//         {
//             MessageTexteId = 3,
//             Content = "Parfait ! Pourrais-je l'avoir pour 20€ ?",
//             MessageId = 3
//         };
//
//         // Conversation 2 : Avec 4 messages
//         _conversation2 = new Conversation
//         {
//             ConversationId = 2,
//             CreationDate = DateTime.UtcNow.AddDays(-8),
//             AnnonceId = 2,
//             StatutConversationId = 2
//         };
//
//         _achete2 = new Achete
//         {
//             UtilisateurAcheteurId = 3,
//             ConversationId = 2
//         };
//
//         _vend2 = new Vend
//         {
//             UtilisateurVendeurId = 1,
//             ConversationId = 2
//         };
//
//         _message2_1 = new Message
//         {
//             MessageId = 4,
//             MessageDate = DateTime.UtcNow.AddDays(-8),
//             MessageLu = true,
//             UtilisateurId = 3,
//             ConversationId = 2
//         };
//
//         _messageTexte2_1 = new MessageTexte
//         {
//             MessageTexteId = 4,
//             Content = "Bonjour, je suis intéressé par les baskets. Quelle est leur pointure ?",
//             MessageId = 4
//         };
//
//         _message2_2 = new Message
//         {
//             MessageId = 5,
//             MessageDate = DateTime.UtcNow.AddDays(-8).AddHours(2),
//             MessageLu = true,
//             UtilisateurId = 1,
//             ConversationId = 2
//         };
//
//         _messageTexte2_2 = new MessageTexte
//         {
//             MessageTexteId = 5,
//             Content = "Bonjour, c'est du 42. Elles sont en excellent état !",
//             MessageId = 5
//         };
//
//         _message2_3 = new Message
//         {
//             MessageId = 6,
//             MessageDate = DateTime.UtcNow.AddDays(-7),
//             MessageLu = true,
//             UtilisateurId = 3,
//             ConversationId = 2
//         };
//
//         _messageTexte2_3 = new MessageTexte
//         {
//             MessageTexteId = 6,
//             Content = "Parfait ! Je les prends à 60€. Comment procède-t-on ?",
//             MessageId = 6
//         };
//
//         _message2_4 = new Message
//         {
//             MessageId = 7,
//             MessageDate = DateTime.UtcNow.AddDays(-6),
//             MessageLu = true,
//             UtilisateurId = 1,
//             ConversationId = 2
//         };
//
//         _messageTexte2_4 = new MessageTexte
//         {
//             MessageTexteId = 7,
//             Content = "Super ! On peut se rencontrer cette semaine pour l'échange.",
//             MessageId = 7
//         };
//
//         // Conversation 3 : Avec 2 messages
//         _conversation3 = new Conversation
//         {
//             ConversationId = 3,
//             CreationDate = DateTime.UtcNow.AddDays(-2),
//             AnnonceId = 1,
//             StatutConversationId = 1
//         };
//
//         _achete3 = new Achete
//         {
//             UtilisateurAcheteurId = 3,
//             ConversationId = 3
//         };
//
//         _vend3 = new Vend
//         {
//             UtilisateurVendeurId = 1,
//             ConversationId = 3
//         };
//
//         _message3_1 = new Message
//         {
//             MessageId = 8,
//             MessageDate = DateTime.UtcNow.AddDays(-2),
//             MessageLu = true,
//             UtilisateurId = 3,
//             ConversationId = 3
//         };
//
//         _messageTexte3_1 = new MessageTexte
//         {
//             MessageTexteId = 8,
//             Content = "Salut ! Le T-shirt est de quelle couleur exactement ?",
//             MessageId = 8
//         };
//
//         _message3_2 = new Message
//         {
//             MessageId = 9,
//             MessageDate = DateTime.UtcNow.AddDays(-1),
//             MessageLu = false,
//             UtilisateurId = 1,
//             ConversationId = 3
//         };
//
//         _messageTexte3_2 = new MessageTexte
//         {
//             MessageTexteId = 9,
//             Content = "Il est noir avec le logo blanc Nike.",
//             MessageId = 9
//         };
//     }
//
//     private void SeedDatabase()
//     {
//         // Statuts de conversation
//         _context.StatutConversations.Add(_statutNegociation);
//         _context.StatutConversations.Add(_statutAcceptation);
//
//         // Utilisateurs
//         _context.Utilisateurs.Add(_vendeur1);
//         _context.Utilisateurs.Add(_acheteur1);
//         _context.Utilisateurs.Add(_acheteur2);
//
//         // Annonces
//         _context.Annonces.Add(_annonce1);
//         _context.Annonces.Add(_annonce2);
//
//         // Conversations
//         _context.Conversations.Add(_conversation1);
//         _context.Conversations.Add(_conversation2);
//         _context.Conversations.Add(_conversation3);
//         _context.SaveChanges();
//
//         // Relations Achete/Vend
//         _context.Achetes.Add(_achete1);
//         _context.Achetes.Add(_achete2);
//         _context.Achetes.Add(_achete3);
//         _context.Vends.Add(_vend1);
//         _context.Vends.Add(_vend2);
//         _context.Vends.Add(_vend3);
//
//         // Messages
//         _context.Messages.Add(_message1_1);
//         _context.Messages.Add(_message1_2);
//         _context.Messages.Add(_message1_3);
//         _context.Messages.Add(_message2_1);
//         _context.Messages.Add(_message2_2);
//         _context.Messages.Add(_message2_3);
//         _context.Messages.Add(_message2_4);
//         _context.Messages.Add(_message3_1);
//         _context.Messages.Add(_message3_2);
//
//         // Messages Texte
//         _context.MessageTextes.Add(_messageTexte1_1);
//         _context.MessageTextes.Add(_messageTexte1_2);
//         _context.MessageTextes.Add(_messageTexte1_3);
//         _context.MessageTextes.Add(_messageTexte2_1);
//         _context.MessageTextes.Add(_messageTexte2_2);
//         _context.MessageTextes.Add(_messageTexte2_3);
//         _context.MessageTextes.Add(_messageTexte2_4);
//         _context.MessageTextes.Add(_messageTexte3_1);
//         _context.MessageTextes.Add(_messageTexte3_2);
//         _context.SaveChanges();
//     }
//
//     [TestMethod]
//     public async Task ShouldGetConversationById()
//     {
//         //Arrange
//         //_context.Conversations.AddRange(new[] {_conversation1, _conversation2, _conversation3});
//         SeedDatabase();
//         
//         //Act
//         var result = _controller.GetById(_conversation1.ConversationId);
//         
//         //Assert
//         Assert.IsNotNull(result.Result);
//         Assert.IsInstanceOfType(result.Result, typeof(ActionResult<ConversationDetailDTO>));
//         var okObject = result.Result.Result as OkObjectResult;
//         Assert.IsNotNull(okObject);
//         Assert.IsInstanceOfType(okObject.Value, typeof(ConversationDetailDTO));
//         var resultDetailDto = okObject.Value as ConversationDetailDTO;
//         Assert.AreEqual(_conversation1.ConversationId, resultDetailDto.ConversationId);
//     }
//
//     [TestMethod]
//     public async Task ShouldNotGetConversationByIdCauseIdDoesNotExist()
//     {
//         //Arrange
//         
//         //Act
//         var result = await _controller.GetById(0);
//         
//         //Assert
//         Assert.IsNotNull(result.Result);
//         Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
//     }
//
//     [TestMethod]
//     public async Task ShouldGetConversationByUtilisateurIdAcheteur()
//     {
//         //Arrange
//         SeedDatabase();
//         
//         //Act
//         var result = _controller.GetByUtilisateurId(_acheteur1.UtilisateurId);
//         
//         //Assert
//         Assert.IsNotNull(result.Result);
//         Assert.IsInstanceOfType(result.Result, typeof(ActionResult<IEnumerable<ConversationDTO>>));
//         var actionResult = result.Result;
//         Assert.IsNotNull(actionResult.Result);
//         Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));
//         var okObjectResult = actionResult.Result as OkObjectResult;
//         Assert.IsNotNull(okObjectResult.Value);
//         Assert.IsInstanceOfType(okObjectResult.Value, typeof(IEnumerable<ConversationDTO>));
//         var conversations = okObjectResult.Value as IEnumerable<ConversationDTO>;
//         Assert.AreEqual(conversations.Count(), 1);
//     }
//     
//     [TestMethod]
//     public async Task ShouldGetConversationByUtilisateurIdVendeur()
//     {
//         //Arrange
//         SeedDatabase();
//         
//         //Act
//         var result = _controller.GetByUtilisateurId(_vendeur1.UtilisateurId);
//         
//         //Assert
//         Assert.IsNotNull(result.Result);
//         Assert.IsInstanceOfType(result.Result, typeof(ActionResult<IEnumerable<ConversationDTO>>));
//         var actionResult = result.Result;
//         Assert.IsNotNull(actionResult.Result);
//         Assert.IsInstanceOfType(actionResult.Result, typeof(OkObjectResult));
//         var okObjectResult = actionResult.Result as OkObjectResult;
//         Assert.IsNotNull(okObjectResult.Value);
//         Assert.IsInstanceOfType(okObjectResult.Value, typeof(IEnumerable<ConversationDTO>));
//         var conversations = okObjectResult.Value as IEnumerable<ConversationDTO>;
//         Assert.AreEqual(conversations.Count(), 3);
//
//     }
// }