using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FrontBlazor;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using FrontBlazor.Models;
using FrontBlazor.Models.StateServices;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel;
using Microsoft.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IStateService<Annonce>, AnnonceStateService>();
builder.Services.AddScoped(typeof(IListableService<>), typeof(ListableService<>));
builder.Services.AddScoped(typeof(IReadableService<>), typeof(ReadableService<>));
builder.Services.AddScoped(typeof(IWritableService<>), typeof(WritableService<>));

// Services
builder.Services.AddScoped<IAuthService, AuthWebService>();
builder.Services.AddScoped<ISignalementService, SignalementWebService>();
builder.Services.AddScoped<ICategorieService<Categorie>, CategorieWebService>();
builder.Services.AddScoped<IConversationService<Conversation>, ConversationWebService>();
builder.Services.AddScoped<IMessageService<Message>, MessageWebService>();
builder.Services.AddScoped<ICouleurService<Couleur>, CouleurWebService>();
builder.Services.AddScoped<IFavorisService<Favoris>, FavorisWebService>();
builder.Services.AddScoped<IAnnonceService<Annonce>, AnnonceWebService>();
builder.Services.AddScoped<IReadableService<UtilisateurView>, UtilisateurWebService>();
builder.Services.AddScoped<IAbonnementService<Abonnement>, AbonnementWebService>();
builder.Services.AddScoped<IMotsInterditsService, MotsInterditWebService>();
// builder.Services.AddScoped<ISignalRService,ChatSignalRService>();
builder.Services.AddSingleton<ISignalRService>(sp =>
{
    return new SignalRWebService();
});

builder.Services.AddScoped<INoteUtilisateurService<NoteUtilisateur>, NoteUtilisateurWebService>();

builder.Services.AddScoped(typeof(ListableViewModel<>));
builder.Services.AddScoped(typeof(WritableService<>));
// ViewModels
builder.Services.AddScoped<LoginViewModel>();
builder.Services.AddScoped<SearchAnnonceViewModel>();
builder.Services.AddScoped<ProfilViewModel>();
builder.Services.AddScoped<HomeViewModel>();
builder.Services.AddScoped<MessagerieViewModel>();
builder.Services.AddScoped<DetailAnnonceViewModel>();
builder.Services.AddScoped<CreationAnnonceViewModel>();
builder.Services.AddScoped<CommercialCategoriesViewModel>();
builder.Services.AddScoped<CommercialCouleursViewModel>();
builder.Services.AddScoped<CommercialMarquesViewModel>();
builder.Services.AddScoped<CommercialSousCategoriesViewModel>();
builder.Services.AddScoped<CommercialTaillesViewModel>();
builder.Services.AddScoped<CommercialViewModel>();
builder.Services.AddScoped<ModerationBoardViewModel>();
builder.Services.AddScoped<MotsInterditsViewModel>();


// HttpClient AVEC CREDENTIALS (cookies)
builder.Services.AddScoped(sp => { return new HttpClient { BaseAddress = new Uri("http://localhost:5096/api/") }; });

await builder.Build().RunAsync();