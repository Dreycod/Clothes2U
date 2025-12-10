using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository;
using Microsoft.EntityFrameworkCore;
using API.Models.Repository.Managers;
using System.Text.Json.Serialization;
using API.DTO;
using API.Hubs;
using API.Services;
using API.Services.Notifications;
using API.Services.Notifications.Observers;
using API.Services.VerificationSrvceV2;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
        policy.WithOrigins("http://localhost:5281")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true));
});
builder.Services.AddAutoMapper(cfg => {
    cfg.AllowNullCollections = true;
}, Assembly.GetExecutingAssembly());
builder.Services.AddScoped<ICaracteristiquesRepository<Categorie>, CategorieManager>();
builder.Services.AddScoped<IDataRepository<SousCategorie, int>, SousCategorieManager>();
builder.Services.AddScoped<IDataRepository<StatutAnnonce, int>, StatutAnnonceManager>();
builder.Services.AddScoped<IDataRepository<Couleur, int>, CouleurManager>();
builder.Services.AddScoped<IFavorisRepository, FavorisManager>();
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurManager>();
builder.Services.AddScoped<IDataRepository<Taille, int>, TailleManager>();
builder.Services.AddScoped<IPhotoRepository<Photo, int>, PhotoManager>();
builder.Services.AddScoped<IDataRepository<Illustre_Annonce, int>, IllustreAnnonceManager>();
builder.Services.AddScoped<IAnnonceRepository<Annonce, int, FilterDTO>, AnnonceManager>();
builder.Services.AddScoped<IConversationRepository<Conversation, int>, ConversationManager>();
builder.Services.AddScoped<IDataRepository<Message, int>, MessageManager>();
builder.Services.AddScoped<IDataRepository<MessageTexte, int>, MessageTexteManager>();
builder.Services.AddScoped<IDataRepository<MessageDemande, int>, MessageDemandeManager>();
builder.Services.AddScoped<IDataRepository<MessageValidation, int>, MessageValidationManager>();
builder.Services.AddScoped<IBloqueRepository<Bloque, int>, BloqueManager>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<INoteUtilisateurRepository,  NoteUtilisateurManager>();
builder.Services.AddScoped<IAbonnementRepository<Abonnement, int>, AbonnementManager>();
builder.Services.AddScoped<IVisualisationRepository<Visualisation, int>, VisualisationManager>();
builder.Services.AddScoped<IRecenseRepository<Recense, int>, RecenseManager>();
builder.Services.AddScoped<ITailleRepository, TailleManager>();
builder.Services.AddScoped<IEtatArticleRepository, EtatArticleManager>();
builder.Services.AddScoped<ICaracteristiquesRepository<Marque>, MarqueManager>(); 
builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeManager>();
builder.Services.AddScoped<IMotInterditRepository, MotInterditManager>();


//services
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, TwilioSmsService>();
builder.Services.AddScoped<ICurrentUserService,  CurrentUserService>();
builder.Services.AddScoped<IMotInterditService,  MotInterditService>();
builder.Services.AddHttpContextAccessor();



//notification
builder.Services.AddScoped<INotificationRepository, NotificationManager>();
builder.Services.AddScoped<INotificationMessageRepository, NotificationMessageManager>();
builder.Services.AddScoped<INotificationNouvelleAnnonceRepository, NotificationNouvelleAnnonceManager>();
builder.Services.AddScoped<INotificationModificationAnnonceRepository, NotificationModificationAnnonceManager>();

// Service de notification - Singleton (mais utilise IServiceProvider pour créer des scopes)
builder.Services.AddSingleton<INotificationService, NotificationService>();

// Observers - Scoped (IMPORTANT: ne plus les enregistrer comme INotificationObserver)
builder.Services.AddScoped<MessageNotificationObserver>();
builder.Services.AddScoped<NouvelleAnnonceNotificationObserver>();
builder.Services.AddScoped<ModificationAnnonceNotificationObserver>();
builder.Services.AddSignalR();



var app = builder.Build();


var notificationService = app.Services.GetRequiredService<INotificationService>();
notificationService.Subscribe<MessageNotificationObserver>();
notificationService.Subscribe<NouvelleAnnonceNotificationObserver>();
notificationService.Subscribe<ModificationAnnonceNotificationObserver>();


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
app.UseAuthentication();  // DOIT être avant UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub");

app.Run();