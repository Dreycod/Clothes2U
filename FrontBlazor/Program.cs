using FrontBlazor.Components;
using FrontBlazor.Models;
using FrontBlazor.Models.StateServices;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.ViewModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configuration HttpClient avec cookies pour Blazor Server
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5096"); // ou https si tu utilises https
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseCookies = true,
        CookieContainer = new System.Net.CookieContainer()
    });

// HttpClient pour les autres services
builder.Services.AddHttpClient<AnnonceService>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5096");
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseCookies = true,
        CookieContainer = new System.Net.CookieContainer()
    });

// State Services
builder.Services.AddScoped<IStateService<Utilisateur>, UserStateService>();

// Services
builder.Services.AddScoped<IAnnonceService<Annonce>, AnnonceService>();

// ViewModels
builder.Services.AddScoped<ConnexionViewModel>();
builder.Services.AddScoped<AnnoncesViewModel>();
builder.Services.AddScoped<ProfilViewModel>();
builder.Services.AddScoped<HomeViewModel>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();