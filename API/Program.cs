using System.Text;
using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository;
using Microsoft.EntityFrameworkCore;
using API.Models.Repository.Managers;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Clothes2UDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Clothes2UDb")));
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };
    });

builder.Services.AddAuthorization();
// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IDataRepository<Categorie, int>, CategorieManager>();
builder.Services.AddScoped<IDataRepository<SousCategorie, int>, SousCategorieManager>();
builder.Services.AddScoped<IDataRepository<StatutAnnonce, int>, StatutAnnonceManager>();
builder.Services.AddScoped<IDataRepository<Couleur, int>, CouleurManager>();
builder.Services.AddScoped<IDataRepository<Utilisateur, int>, UtilisateurManager>();
builder.Services.AddScoped<IDataRepository<Taille, int>, TailleManager>();

builder.Services.AddScoped<IAnnonceRepository<Annonce, int>, AnnonceManager>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
    
app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();