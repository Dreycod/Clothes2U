using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FrontBlazor;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using FrontBlazor.Models;
using FrontBlazor.Models.StateServices;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.ViewModel;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IStateService<Annonce>, AnnonceStateService>();

// Services
builder.Services.AddScoped<ICategorieService<Categorie>, CategorieService>();
builder.Services.AddScoped<IConversationService<Conversation>, ConversationService>();
builder.Services.AddScoped<IMessageService<Message>, MessageService>();
builder.Services.AddScoped<ITailleService<Taille>, TailleService>();
builder.Services.AddScoped<IMarqueService<Marque>, MarqueService>();
builder.Services.AddScoped<ICouleurService<Couleur>, CouleurService>();
builder.Services.AddScoped<IAnnonceService<Annonce>, AnnonceService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ViewModels
builder.Services.AddScoped<ConnexionViewModel>();
builder.Services.AddScoped<AnnoncesViewModel>();
builder.Services.AddScoped<ProfilViewModel>();
builder.Services.AddScoped<HomeViewModel>();
builder.Services.AddScoped<CategorieViewModel>();
builder.Services.AddScoped<ConversationViewModel>();
builder.Services.AddScoped<MessageViewModel>();
builder.Services.AddScoped<TailleViewModel>();
builder.Services.AddScoped<MarqueViewModel>();
builder.Services.AddScoped<CouleurViewModel>();



builder.Services.AddScoped(sp =>
{
    return new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7188/api/")
    };
});


await builder.Build().RunAsync();