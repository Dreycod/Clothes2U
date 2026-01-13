using API.Hubs;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services;
using API.Services.VerificationSrvceV2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shared.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using API.Controllers;
using API.Models.Repository.Interfaces;
using Shared.DTO.Photo;
using Shared.DTO.Tag;
using API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Clothes2UDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Clothes2UDb")));

// Configuration JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

Console.WriteLine($"🔑 Configuration JWT:");
Console.WriteLine($"   Key length: {jwtKey?.Length ?? 0} caractères");
Console.WriteLine($"   Issuer: {jwtIssuer}");
Console.WriteLine($"   Audience: {jwtAudience}");

/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,           // ← DÉSACTIVÉ
            ValidateAudience = true,          // Gardé activé car présent
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("authToken", out var token))
                {
                    token = token.Trim();
            
                    // NE PAS utiliser context.Token - ça ne marche pas
                    // À la place, injecter dans le header Authorization
                    context.Request.Headers.Remove("Authorization");
                    context.Request.Headers.Append("Authorization", $"Bearer {token}");
            
                    Console.WriteLine($"✅ [OnMessageReceived] Token ajouté au header Authorization");
                    Console.WriteLine($"   Token complet: {token}");
                }
                else
                {
                    Console.WriteLine("❌ [OnMessageReceived] Cookie 'authToken' absent");
                }
        
                return Task.CompletedTask;
            },
    
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"❌ [OnAuthenticationFailed] {context.Exception.Message}");
        
                // Déboguer ce que le middleware reçoit réellement
                var authHeader = context.Request.Headers["Authorization"].ToString();
                Console.WriteLine($"❌ Header Authorization au moment de l'échec: '{authHeader}'");
        
                return Task.CompletedTask;
            },
    
            OnTokenValidated = context =>
            {
                var userId = context.Principal?.FindFirst("userId")?.Value;
                var email = context.Principal?.FindFirst("sub")?.Value;
                Console.WriteLine($"✅✅✅ [OnTokenValidated] Email: {email}, UserId: {userId}");
                return Task.CompletedTask;
            }
        };
    });*/
// Configuration de l'authentification (JWT + Google OAuth)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // ⬅️ GARDEZ cette ligne
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.TryGetValue("authToken", out var token))
            {
                token = token.Trim();
                context.Request.Headers.Remove("Authorization");
                context.Request.Headers.Append("Authorization", $"Bearer {token}");
                Console.WriteLine($"✅ [OnMessageReceived] Token ajouté au header Authorization");
            }
            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"❌ [OnAuthenticationFailed] {context.Exception.Message}");
            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
            var userId = context.Principal?.FindFirst("userId")?.Value;
            var email = context.Principal?.FindFirst("sub")?.Value;
            Console.WriteLine($"✅✅✅ [OnTokenValidated] Email: {email}, UserId: {userId}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Clothes2U API", Version = "v1" });

    // 🔐 Déclaration Bearer JWT
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "⚠️ Entrez votre JWT sous la forme: Bearer {token}"
    });

    // 🔐 Application globale du Bearer
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorDev", policy =>
        policy.WithOrigins("http://localhost:5281") // URL exacte de votre Blazor
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()); // ✅ Pas de SetIsOriginAllowed avec AllowCredentials
});
builder.Services.AddAutoMapper(cfg => {
    cfg.AllowNullCollections = true;
}, Assembly.GetExecutingAssembly());
builder.Services.AddScoped<ICaracteristiquesRepository<Categorie>, CategorieManager>();
builder.Services.AddScoped<ICaracteristiquesRepository<Genre>, GenreManager>();
builder.Services.AddScoped<ICaracteristiquesRepository<Couleur>, CouleurManager>();
builder.Services.AddScoped<IDataRepository<SousCategorie, int>, SousCategorieManager>();
builder.Services.AddScoped<IDataRepository<StatutAnnonce, int>, StatutAnnonceManager>();
builder.Services.AddScoped<IDataRepository<StatutConversation, int>, StatutConversationManager>();
builder.Services.AddScoped<IFavorisRepository, FavorisManager>();
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurManager>();
builder.Services.AddScoped<IPhotoRepository, PhotoManager>();
builder.Services.AddScoped<IllustreAnnonceRepository<Illustre_Annonce, int>, IllustreAnnonceManager>();
builder.Services.AddScoped<IAnnonceRepository<Annonce, int, FilterDTO>, AnnonceManager>();
builder.Services.AddScoped<IConversationRepository<Conversation, int>, ConversationManager>();
builder.Services.AddScoped<IMessageRepository, MessageManager>();
builder.Services.AddScoped<IDataRepository<MessageTexte, int>, MessageTexteManager>();
builder.Services.AddScoped<IMessageDemandeRepository, MessageDemandeManager>();
builder.Services.AddScoped<IDataRepository<MessageEstPayee, int>, MessageEstPayeeManager>();
builder.Services.AddScoped<IDataRepository<MessageEnvoieColis, int>, MessageEnvoieColisManager>();
builder.Services.AddScoped<IDataRepository<MessageContientImage, int>, MessageContientImageManager>();
builder.Services.AddScoped<IDataRepository<MessageEstRecu, int>, MessageEstRecuManager>();
builder.Services.AddScoped<IBloqueRepository<Bloque, int>, BloqueManager>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<INoteUtilisateurRepository,  NoteUtilisateurManager>();
builder.Services.AddScoped<IAbonnementRepository<Abonnement, int>, AbonnementManager>();
builder.Services.AddScoped<IVisualisationRepository<Visualisation, int>, VisualisationManager>();
builder.Services.AddScoped<IRecenseRepository<Recense, int>, RecenseManager>();
builder.Services.AddScoped<ITailleRepository, TailleManager>();
builder.Services.AddScoped<IDataRepository<EtatArticle, int>, EtatArticleManager>();
builder.Services.AddScoped<ICaracteristiquesRepository<Marque>, MarqueManager>(); 
builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeManager>();
builder.Services.AddScoped<IMotInterditRepository, MotInterditManager>();
builder.Services.AddScoped<ISignalementRepository,  SignalementManager>();
builder.Services.AddScoped<IPhotoRepository, PhotoManager>();
builder.Services.AddScoped<IDataRepository<Annonce, int>, AnnonceManager>();
builder.Services.AddScoped<IDataRepository<Utilisateur, int>, UtilisateurManager>();
builder.Services.AddScoped<IDecisionRepository,  DecisionManager>();
builder.Services.AddScoped<IDemandeRestaurationRepository<DemandeRestauration, int>,  DemandeRestaurationManager>();
builder.Services.AddScoped<ITransactionRepository<Transaction, int>, TransactionManager>();
builder.Services.AddScoped<IPasswordResetRepository<PasswordResetToken, int>, PasswordResetManager>();
builder.Services.AddScoped<ICaracteristiquesRepository<Mesure>, MesureManager>();
builder.Services.AddScoped<IClusterRepository, ClusterManager>(); 
builder.Services.AddScoped<IMarqueRepository, MarqueManager>();
builder.Services.AddScoped<ITagRepository<Tag, int>, TagManager>();
builder.Services.AddScoped<IEstDeCouleurRepository<Est_De_Couleur, int>, EstDeCouleurManager>();
builder.Services.AddScoped<IDataRepository<SupportTicket, int>, TicketSupportManager>();

//services
builder.Services.AddHttpClient();
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, TwilioSmsService>();
builder.Services.AddScoped<ICurrentUserService,  CurrentUserService>();
builder.Services.AddScoped<IMotInterditService,  MotInterditService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<ISuggestionService, SuggestionService>();
builder.Services.AddScoped<IDetectionService, DetectionService>();

builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IOrderRepository, OrderManager>();
builder.Services.AddScoped<IModerationDashboardService, ModerationDashboardService>();
builder.Services.AddScoped<IAnnonceExtensionService, AnnonceExtensionService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<INotificationMailService, NotificationMailService>();
builder.Services.AddScoped<PasswordResetService>();
builder.Services.AddScoped<IUserDeletionService, UserDeletionService>();
builder.Services.AddScoped<ISupportService, SupportService>();

//notification
builder.Services.AddScoped<INotificationRepository, NotificationManager>();
builder.Services.AddScoped<IDataRepository<NotificationMessage, int>, NotificationMessageManager>();
builder.Services.AddScoped<IDataRepository<NotificationAvertissement, int>,  NotificationAvertissementManager>();
builder.Services.AddScoped<IDataRepository<NotificationNouvelleAnnonce, int>, NotificationNouvelleAnnonceManager>();
builder.Services.AddScoped<IDataRepository<NotificationModificationAnnonce, int>, NotificationModificationAnnonceManager>();
builder.Services.AddScoped<IDataRepository<NotificationProposition, int>, NotificationPropositionManager>();
builder.Services.AddScoped<IDataRepository<NotificationAchatAnnonce, int>, NotificationAchatManager>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationHubService, NotificationHubService>();

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));


var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/Login/google-callback"))
    {
        Console.WriteLine("\n🔍 ========== GOOGLE CALLBACK DEBUG ==========");
        Console.WriteLine($"📍 Path: {context.Request.Path}");
        Console.WriteLine($"🔗 Query: {context.Request.QueryString}");
        Console.WriteLine($"🍪 Cookies reçus:");
        foreach (var cookie in context.Request.Cookies)
        {
            Console.WriteLine($"   - {cookie.Key}: {cookie.Value.Substring(0, Math.Min(50, cookie.Value.Length))}...");
        }
        Console.WriteLine("============================================\n");
    }

    await next();
});



// 1. Middleware de diagnostic (le vôtre)
app.Use(async (context, next) =>
{
    Console.WriteLine($"\n🌐 ========== NOUVELLE REQUÊTE ==========");
    Console.WriteLine($"🎯 {context.Request.Method} {context.Request.Path}");
    // ... vos logs
    await next();
    Console.WriteLine($"📤 Réponse: {context.Response.StatusCode}");
    Console.WriteLine($"==========================================\n");
});


// Middleware de diagnostic
app.Use(async (context, next) =>
{
    Console.WriteLine($"\n🌐 ========== NOUVELLE REQUÊTE ==========");
    Console.WriteLine($"🎯 {context.Request.Method} {context.Request.Path}");
    Console.WriteLine($"🌍 Origin: {context.Request.Headers["Origin"]}");
    Console.WriteLine($"🍪 Cookies: {context.Request.Cookies.Count}");
    
    foreach (var cookie in context.Request.Cookies)
    {
        var preview = cookie.Value.Length > 50 ? cookie.Value.Substring(0, 50) + "..." : cookie.Value;
        Console.WriteLine($"   🍪 {cookie.Key}: {preview}");
    }
    
    await next();
    
    Console.WriteLine($"📤 Réponse: {context.Response.StatusCode}");
    Console.WriteLine($"==========================================\n");
});

// CORS AVANT Authentication
app.UseCors("AllowBlazorDev");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ordre CRITIQUE des middlewares
app.UseRouting();
app.UseSession();
app.UseAuthentication();  // DOIT être avant UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");
app.MapHub<NotificationHub>("/notificationHub");
app.Run();