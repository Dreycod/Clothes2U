using FrontBlazor.Components;
using FrontBlazor.Models;
using FrontBlazor.Models.StateServices;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.ViewModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HttpClient global (comme WASM)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5096/api/")
});

// State Services
builder.Services.AddScoped<IStateService<Utilisateur>, UserStateService>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAnnonceService<Annonce>, AnnonceService>();
builder.Services.AddScoped<ICategorieService<Categorie>, CategorieService>();
builder.Services.AddScoped<IConversationService<Conversation>, ConversationService>();
builder.Services.AddScoped<IMessageService<Message>, MessageService>();

// ViewModels
builder.Services.AddScoped<ConnexionViewModel>();
builder.Services.AddScoped<AnnoncesViewModel>();
builder.Services.AddScoped<ProfilViewModel>();
builder.Services.AddScoped<HomeViewModel>();
builder.Services.AddScoped<CategorieViewModel>();
builder.Services.AddScoped<ConversationViewModel>();
builder.Services.AddScoped<MessageViewModel>();

var app = builder.Build();

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