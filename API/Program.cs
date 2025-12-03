using System.IdentityModel.Tokens.Jwt;
using System.Text;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using System.Text.Json.Serialization;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
//  DATABASE
// -----------------------------
builder.Services.AddDbContext<Clothes2UDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Clothes2UDb")));

// -----------------------------
//  AUTHENTICATION (JWT + COOKIE)
// -----------------------------
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// ✅ Récupération de la clé JWT
var jwtKey = builder.Configuration["Jwt:Key"];
Console.WriteLine($"[Program.cs] 🔑 Clé JWT configurée (longueur: {jwtKey?.Length ?? 0})");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromMinutes(5),

        // IMPORTANT pour que User.FindFirst("uid") et roles fonctionnent
        NameClaimType = "uid",
        RoleClaimType = "role"
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Lire le cookie "authToken" si présent
            if (context.Request.Cookies.TryGetValue("authToken", out var cookieToken) &&
                !string.IsNullOrEmpty(cookieToken))
            {
                context.Token = cookieToken;
            }

            // Debug (facultatif)
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader))
            {
                Console.WriteLine($"📩 Header Authorization: {authHeader.Substring(0, Math.Min(50, authHeader.Length))}...");
            }
            else if (!string.IsNullOrEmpty(cookieToken))
            {
                Console.WriteLine($"📩 Token lu depuis cookie (length {cookieToken.Length})");
            }
            else
            {
                Console.WriteLine("⚠️ Aucun token dans Authorization header ni dans cookie");
            }

            return Task.CompletedTask;
        },

        OnTokenValidated = ctx =>
        {
            Console.WriteLine("✅ Token VALIDÉ avec succès !");
            foreach (var claim in ctx.Principal.Claims)
                Console.WriteLine($"   Claim: {claim.Type} = {claim.Value}");
            return Task.CompletedTask;
        },

        OnAuthenticationFailed = ctx =>
        {
            Console.WriteLine($"❌ Échec de l'authentification JWT : {ctx.Exception?.Message}");
            return Task.CompletedTask;
        }
    };
});



builder.Services.AddAuthorization(config =>
{
    config.AddPolicy(Policies.Authorized, Policies.Logged());
});

// -----------------------------
//  JSON & CONTROLLERS
// -----------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// -----------------------------
//  SWAGGER
// -----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -----------------------------
//  CORS (Blazor WebAssembly + cookies)
// -----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorDev", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7188",
                "http://localhost:5094",  
                "http://localhost:5084",  
                "https://localhost:7214"  
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// -----------------------------
//  DEPENDENCY INJECTION
// -----------------------------
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IDataRepository<Categorie, int>, CategorieManager>();
builder.Services.AddScoped<IDataRepository<SousCategorie, int>, SousCategorieManager>();
builder.Services.AddScoped<IDataRepository<StatutAnnonce, int>, StatutAnnonceManager>();
builder.Services.AddScoped<IDataRepository<Couleur, int>, CouleurManager>();
builder.Services.AddScoped<IFavorisRepository, FavorisManager>();
builder.Services.AddScoped<IDataRepository<Utilisateur, int>, UtilisateurManager>();
builder.Services.AddScoped<ITailleRepository, TailleManager>();
builder.Services.AddScoped<IPhotoRepository<Photo, int>, PhotoManager>();
builder.Services.AddScoped<IDataRepository<Illustre_Annonce, int>, IllustreAnnonceManager>();
builder.Services.AddScoped<IAnnonceRepository<Annonce, int>, AnnonceManager>();
builder.Services.AddScoped<IConversationRepository<Conversation, int>, ConversationManager>();
builder.Services.AddScoped<INotificationRepository<Notification>, NotificationManager>();
builder.Services.AddScoped<IDataRepository<Message, int>, MessageManager>();
builder.Services.AddScoped<IDataRepository<MessageTexte, int>, MessageTexteManager>();
builder.Services.AddScoped<IDataRepository<MessageDemande, int>, MessageDemandeManager>();
builder.Services.AddScoped<IDataRepository<MessageValidation, int>, MessageValidationManager>();
builder.Services.AddScoped<IDataRepository<Marque, int>, MarqueManager>();
builder.Services.AddScoped<IBloqueRepository<Bloque, int>, BloqueManager>();
builder.Services.AddScoped<IRecenseRepository<Recense, int>, RecenseManager>();

// Services
builder.Services.AddScoped<IPhotoService, PhotoService>();

// -----------------------------
//  APP
// -----------------------------
var app = builder.Build();

// -----------------------------
//  MIDDLEWARES
// -----------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ ORDRE IMPORTANT : CORS avant Authentication
app.UseCors("AllowBlazorDev"); 

// ✅ Authentification & Autorisation
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

Console.WriteLine("🚀 Application démarrée et prête à accepter des connexions");

// Run
app.Run();