using System;
using System.Collections.Generic;
using System.Linq;
using API.Controllers;
using API.Mapper;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository.Managers;
using API.Services;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared.DTO.Signalement;
using Shared.DTO.Utilisateur;

namespace API.Tests.Controllers.integration;


[TestClass]
[TestSubject(typeof(SignalementController))]
[TestCategory("integration")]
public class SignalementControllerTest
{
    private Clothes2UDbContext _context;
    private SignalementController _controller;
    private IMapper _mapper;
    private ICurrentUserService _currentUserService;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private const int TEST_USER_ID = 1;
    
    [TestInitialize]
    public void Setup()
    {
        var builder = new DbContextOptionsBuilder<Clothes2UDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        _context = new Clothes2UDbContext(builder.Options);

        CleanupDatabase();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SignalementMappingProfile>();
        });
        _mapper = config.CreateMapper();

        var Manager = new SignalementManager(_context);

        var mockCurrentUserService = new Mock<ICurrentUserService>();
        mockCurrentUserService
            .Setup(s => s.GetUserIdOrThrow())
            .ReturnsAsync(TEST_USER_ID);

        _controller = new SignalementController(
            Manager,
            _mapper,
            mockCurrentUserService.Object
        );
        InitializeDefaultObjects();
    }
    private void CleanupDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    private const int TEST_USER_ID_1 = 1;
    private const int TEST_USER_ID_2 = 2;
    private const int TEST_USER_ID_3 = 3;
    private const int TEST_ANNONCE_ID_1 = 1;
    private const int TEST_ANNONCE_ID_2 = 2;
    private const int TEST_AVIS_ID_1 = 1;
    private const int TEST_MESSAGE_ID_1 = 1;
    private const int TEST_CONVERSATION_ID_1 = 1;
    
    public RoleUtilisateur DefaultRole { get; private set; }
    public StatutUtilisateur DefaultStatut { get; private set; }
    public TypeSignalement TypeSignalementAnnonce { get; private set; }
    public TypeSignalement TypeSignalementUtilisateur { get; private set; }
    public TypeSignalement TypeSignalementAvis { get; private set; }
    public TypeSignalement TypeSignalementMessage { get; private set; }
    
    public Utilisateur UtilisateurSignaleur { get; private set; }
    public Utilisateur UtilisateurSignale { get; private set; }
    public Utilisateur UtilisateurVendeur { get; private set; }
    
    public Annonce AnnonceSignalee { get; private set; }
    public NoteUtilisateur AvisSignale { get; private set; }
    public Conversation Conversation { get; private set; }
    public Message MessageSignale { get; private set; }
    
    public Signalement SignalementAnnonceTest { get; private set; }
    public Signalement SignalementUtilisateurTest { get; private set; }
    public Signalement SignalementAvisTest { get; private set; }
    public Signalement SignalementMessageTest { get; private set; }
    
    public SignalementAnnonce SignalementAnnonceRelation { get; private set; }
    public SignalementUtilisateur SignalementUtilisateurRelation { get; private set; }
    public SignalementAvis SignalementAvisRelation { get; private set; }
    public SignalementMessage SignalementMessageRelation { get; private set; }
    
    public StatutAnnonce StatutAnnonceActif { get; private set; }
    public Categorie DefaultCategorie { get; private set; }
    public SousCategorie DefaultSousCategorie { get; private set; }
    public Marque DefaultMarque { get; private set; }
    public Taille DefaultTaille { get; private set; }
    public EtatArticle DefaultEtat { get; private set; }
    public Genre DefaultGenre { get; private set; }

    private void InitializeDefaultObjects()
    {
       DefaultRole = new RoleUtilisateur
        {
            RoleUtilisateurId = 1,
            RoleUtilisateurLibelle = "User"
        };

        DefaultStatut = new StatutUtilisateur
        {
            StatutUtilisateurId = 1,
            StatutLibelle = "Actif"
        };
        TypeSignalementAnnonce = new TypeSignalement
        {
            SignalementTypeId = 1,
            SignalementTypeLibelle = "Annonce inappropriée"
        };

        TypeSignalementUtilisateur = new TypeSignalement
        {
            SignalementTypeId = 2,
            SignalementTypeLibelle = "Comportement abusif"
        };

        TypeSignalementAvis = new TypeSignalement
        {
            SignalementTypeId = 3,
            SignalementTypeLibelle = "Avis frauduleux"
        };

        TypeSignalementMessage = new TypeSignalement
        {
            SignalementTypeId = 4,
            SignalementTypeLibelle = "Message inapproprié"
        };
        UtilisateurSignaleur = new Utilisateur
        {
            UtilisateurId = TEST_USER_ID_1,
            Login = "SignaleurTest",
            Email = "signaleur@test.com",
            Password = "password123",
            Description = "Utilisateur qui signale",
            Dateinscription = DateTime.Now.AddMonths(-6),
            ValidEmail = true,
            ValidTelephone = false,
            PreferenceCookies = true,
            PreferenceTheme = false,
            PreferenceNotifMail = true,
            RoleId = DefaultRole.RoleUtilisateurId,
            StatutId = DefaultStatut.StatutUtilisateurId,
            Role = DefaultRole,
            Statut = DefaultStatut
        };

        UtilisateurSignale = new Utilisateur
        {
            UtilisateurId = TEST_USER_ID_2,
            Login = "UtilisateurSignale",
            Email = "signale@test.com",
            Password = "password123",
            Description = "Utilisateur signalé",
            Dateinscription = DateTime.Now.AddMonths(-3),
            ValidEmail = true,
            ValidTelephone = false,
            PreferenceCookies = true,
            PreferenceTheme = false,
            PreferenceNotifMail = true,
            RoleId = DefaultRole.RoleUtilisateurId,
            StatutId = DefaultStatut.StatutUtilisateurId,
            Role = DefaultRole,
            Statut = DefaultStatut
        };

        UtilisateurVendeur = new Utilisateur
        {
            UtilisateurId = TEST_USER_ID_3,
            Login = "VendeurTest",
            Email = "vendeur@test.com",
            Password = "password123",
            Description = "Vendeur d'annonces",
            Dateinscription = DateTime.Now.AddYears(-1),
            ValidEmail = true,
            ValidTelephone = true,
            PreferenceCookies = true,
            PreferenceTheme = false,
            PreferenceNotifMail = true,
            RoleId = DefaultRole.RoleUtilisateurId,
            StatutId = DefaultStatut.StatutUtilisateurId,
            Role = DefaultRole,
            Statut = DefaultStatut
        };
        StatutAnnonceActif = new StatutAnnonce
        {
            StatutAnnonceId = 1,
            StatutLibelle = "Actif"
        };

        DefaultCategorie = new Categorie
        {
            CategorieId = 1,
            LibelleCategorie = "Vêtements"
        };

        DefaultSousCategorie = new SousCategorie
        {
            SousCategorieId = 1,
            LibelleSousCategorie = "T-shirts",
            CategorieId = DefaultCategorie.CategorieId,
            Categorie = DefaultCategorie
        };

        DefaultMarque = new Marque
        {
            MarqueId = 1,
            NomMarque = "Nike"
        };

        DefaultTaille = new Taille
        {
            TailleId = 1,
            Libelletaille = "M"
        };

        DefaultEtat = new EtatArticle
        {
            EtatArticleId = 1,
            NomEtat = "Bon état"
        };

        DefaultGenre = new Genre
        {
            GenreId = 1,
            NomGenre = "Mixte"
        };

        // 5. Annonce signalée
        AnnonceSignalee = new Annonce
        {
            AnnonceId = TEST_ANNONCE_ID_1,
            Title = "Annonce contenu inapproprié",
            Description = "Cette annonce contient du contenu inapproprié",
            DateAnnonce = DateTime.Now.AddDays(-5),
            Negociable = true,
            Prix = 50.00m,
            UtilisateurId = UtilisateurVendeur.UtilisateurId,
            EtatId = DefaultEtat.EtatArticleId,
            MarqueId = DefaultMarque.MarqueId,
            TailleId = DefaultTaille.TailleId,
            SousCategorieId = DefaultSousCategorie.SousCategorieId,
            CategorieId = DefaultCategorie.CategorieId,
            StatutAnnonceId = StatutAnnonceActif.StatutAnnonceId,
            GenreId = DefaultGenre.GenreId,
            Utilisateur = UtilisateurVendeur,
            Etat = DefaultEtat,
            Marque = DefaultMarque,
            Taille = DefaultTaille,
            SousCategorie = DefaultSousCategorie,
            Categorie = DefaultCategorie,
            Statut = StatutAnnonceActif,
            GenreAnnonce = DefaultGenre
        };

        // 6. Avis signalé
        AvisSignale = new NoteUtilisateur
        {
            NoteUtilisateurId = TEST_AVIS_ID_1,
            Note = 1,
            Commentaire = "Commentaire frauduleux ou inapproprié",
            DatePublication = DateTime.Now.AddDays(-2),
            Statut = true,
            AuteurId = UtilisateurSignale.UtilisateurId,
            CibleId = UtilisateurVendeur.UtilisateurId,
            Auteur = UtilisateurSignale,
            Cible = UtilisateurVendeur
        };

        // 7. Conversation et message signalé
        Conversation = new Conversation
        {
            ConversationId = TEST_CONVERSATION_ID_1,
            AnnonceId = AnnonceSignalee.AnnonceId,
            LAnnonce = AnnonceSignalee
        };

        MessageSignale = new Message
        {
            MessageId = TEST_MESSAGE_ID_1,
            MessageDate = DateTime.Now.AddDays(-1),
            MessageLu = true,
            MessageStatut = true,
            UtilisateurId = UtilisateurSignale.UtilisateurId,
            ConversationId = Conversation.ConversationId,
            Utilisateur = UtilisateurSignale,
            Conversation = Conversation
        };

        // 8. Signalements de base
        SignalementAnnonceTest = new Signalement
        {
            SignalementId = 1,
            SignalementDate = DateTime.Now,
            SignalementMotif = "Cette annonce contient des images inappropriées",
            SignalementTypeId = TypeSignalementAnnonce.SignalementTypeId,
            UtilisateurId = UtilisateurSignaleur.UtilisateurId,
            TypeSignalement = TypeSignalementAnnonce,
            Utilisateur = UtilisateurSignaleur
        };

        SignalementUtilisateurTest = new Signalement
        {
            SignalementId = 2,
            SignalementDate = DateTime.Now,
            SignalementMotif = "Cet utilisateur a un comportement harcelant",
            SignalementTypeId = TypeSignalementUtilisateur.SignalementTypeId,
            UtilisateurId = UtilisateurSignaleur.UtilisateurId,
            TypeSignalement = TypeSignalementUtilisateur,
            Utilisateur = UtilisateurSignaleur
        };

        SignalementAvisTest = new Signalement
        {
            SignalementId = 3,
            SignalementDate = DateTime.Now,
            SignalementMotif = "Cet avis semble être frauduleux",
            SignalementTypeId = TypeSignalementAvis.SignalementTypeId,
            UtilisateurId = UtilisateurSignaleur.UtilisateurId,
            TypeSignalement = TypeSignalementAvis,
            Utilisateur = UtilisateurSignaleur
        };

        SignalementMessageTest = new Signalement
        {
            SignalementId = 4,
            SignalementDate = DateTime.Now,
            SignalementMotif = "Ce message contient du langage offensant",
            SignalementTypeId = TypeSignalementMessage.SignalementTypeId,
            UtilisateurId = UtilisateurSignaleur.UtilisateurId,
            TypeSignalement = TypeSignalementMessage,
            Utilisateur = UtilisateurSignaleur
        };
        SignalementAnnonceRelation = new SignalementAnnonce
        {
            SignalementAnnonceId = 1,
            AnnonceSignaleeId = AnnonceSignalee.AnnonceId,
            SignalementId = SignalementAnnonceTest.SignalementId,
            Annonce = AnnonceSignalee,
            Signalement = SignalementAnnonceTest
        };

        SignalementUtilisateurRelation = new SignalementUtilisateur
        {
            SignalementUtilisateurId = 1,
            UtilisateurSignaleId = UtilisateurSignale.UtilisateurId,
            SignalementId = SignalementUtilisateurTest.SignalementId,
            UtilisateurSignale = UtilisateurSignale,
            Signalement = SignalementUtilisateurTest
        };

        SignalementAvisRelation = new SignalementAvis
        {
            SignalementAvisId = 1,
            AvisId = AvisSignale.NoteUtilisateurId,
            SignalementId = SignalementAvisTest.SignalementId,
            Avis = AvisSignale,
            Signalement = SignalementAvisTest
        };

        SignalementMessageRelation = new SignalementMessage
        {
            SignalementMessageId = 1,
            MessageId = MessageSignale.MessageId,
            SignalementId = SignalementMessageTest.SignalementId,
            Message = MessageSignale,
            Signalement = SignalementMessageTest
        };
        SignalementAnnonceTest.SignalementsAnnonce = SignalementAnnonceRelation;
        SignalementUtilisateurTest.SignalementsUtilisateur = SignalementUtilisateurRelation;
        SignalementAvisTest.SignalementsAvis = SignalementAvisRelation;
        SignalementMessageTest.SignalementsMessage = SignalementMessageRelation;
        
        _context.Set<RoleUtilisateur>().Add(DefaultRole);
        _context.Set<StatutUtilisateur>().Add(DefaultStatut);
        _context.Set<TypeSignalement>().AddRange(
            TypeSignalementAnnonce, 
            TypeSignalementUtilisateur, 
            TypeSignalementAvis,
            TypeSignalementMessage
        );
        
        // 2. Données pour annonces
        _context.Set<StatutAnnonce>().Add(StatutAnnonceActif);
        _context.Set<Categorie>().Add(DefaultCategorie);
        _context.Set<SousCategorie>().Add(DefaultSousCategorie);
        _context.Set<Marque>().Add(DefaultMarque);
        _context.Set<Taille>().Add(DefaultTaille);
        _context.Set<EtatArticle>().Add(DefaultEtat);
        _context.Set<Genre>().Add(DefaultGenre);
        
        // 3. Utilisateurs
        _context.Set<Utilisateur>().AddRange(
            UtilisateurSignaleur, 
            UtilisateurSignale, 
            UtilisateurVendeur
        );
        
        // 4. Annonce, Avis, Conversation, Message
        _context.Set<Annonce>().Add(AnnonceSignalee);
        _context.Set<NoteUtilisateur>().Add(AvisSignale);
        _context.Set<Conversation>().Add(Conversation);
        _context.Set<Message>().Add(MessageSignale);
        
        _context.Set<Signalement>().AddRange(
            SignalementAnnonceTest,
            SignalementUtilisateurTest,
            SignalementAvisTest,
            SignalementMessageTest
        );
        
        // 6. Relations de signalement
        _context.Set<SignalementAnnonce>().Add(SignalementAnnonceRelation);
        _context.Set<SignalementUtilisateur>().Add(SignalementUtilisateurRelation);
        _context.Set<SignalementAvis>().Add(SignalementAvisRelation);
        _context.Set<SignalementMessage>().Add(SignalementMessageRelation);
        
        _context.SaveChanges();
    }
    [TestMethod]
    public void ShouldGetSignalementReturnNotFound()
    {
        //Given
        int nonExistingId = 999;
        
        //Act
        IActionResult action = _controller.GetById(nonExistingId).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    [TestMethod]
    public void ShouldGetSignalement()
    {
        //Given
        int elementId = SignalementAnnonceTest.SignalementId;
        
        //Act
        IActionResult action = _controller.GetById(elementId).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(ObjectResult));
        var okResult = action as ObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(SignalementDetailsDTO));
        var returnElement = okResult.Value as SignalementDetailsDTO;
        Assert.IsNotNull(returnElement);
        Assert.AreEqual(SignalementAnnonceTest.GetId(), returnElement.SignalementId);
    }

    [TestMethod]
    public void ShouldGetSignalements()
    {
        //Given : Les elements sont déjà en base
        
        //Act
        ActionResult<IEnumerable<SignalementDTO>> action = _controller.GetAll().GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<SignalementDTO>));
        var returnelements = okResult.Value as IEnumerable<SignalementDTO>;
        Assert.IsNotNull(returnelements);
        Assert.AreEqual(returnelements.Count(), 4);
        Assert.IsTrue(returnelements.Any(b => b.SignalementId == SignalementAnnonceTest.GetId()));
        Assert.IsTrue(returnelements.Any(b => b.SignalementId == SignalementUtilisateurTest.GetId()));
        Assert.IsTrue(returnelements.Any(b => b.SignalementId == SignalementAvisTest.GetId()));
        Assert.IsTrue(returnelements.Any(b => b.SignalementId == SignalementMessageTest.GetId()));
    }

    [TestMethod]
    public void ShouldGetSignalementByType()
    {
        //Given
        int typeId = TypeSignalementAnnonce.SignalementTypeId;
        
        //Act
        ActionResult<IEnumerable<SignalementDTO>> action = _controller.GetByType(typeId).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        var okResult = action.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<SignalementDTO>));
        var returnelements = okResult.Value as IEnumerable<SignalementDTO>;
        Assert.IsNotNull(returnelements);
        Assert.AreEqual(returnelements.Count(), 1);
    }
}   
