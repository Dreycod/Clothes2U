using FrontBlazor;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel;
using FrontBlazor.ViewModel.Moderation;
using FrontBlazor.ViewModel.Moderation.Signalements;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Shared.DTO;
using Shared.DTO.Abonnement;
using Shared.DTO.Annonce;
using Shared.DTO.Categorie;
using Shared.DTO.Conversation;
using Shared.DTO.Couleur;
using Shared.DTO.Favoris;
using Shared.DTO.Message;
using Shared.DTO.Photo;
using Shared.DTO.NoteUtilisateur;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IStateService<AnnonceDTO>, AnnonceStateService>();
builder.Services.AddScoped(typeof(IListableService<>), typeof(ListableService<>));
builder.Services.AddScoped(typeof(IReadableService<>), typeof(ReadableService<>));
builder.Services.AddScoped(typeof(IWritableService<>), typeof(WritableService<>));

// Services
builder.Services.AddScoped<IAuthService, AuthWebService>();
builder.Services.AddScoped<INotificationService, NotificationWebService>();
builder.Services.AddScoped<ISignalementService, SignalementWebService>();
builder.Services.AddScoped<IBloqueService, BloqueWebService>();
builder.Services.AddScoped<ICategorieService<CategorieDTO>, CategorieWebService>();
builder.Services.AddScoped<IConversationService<ConversationDTO>, ConversationWebService>();
builder.Services.AddScoped<IMessageService<MessageDTO>, MessageWebService>();
builder.Services.AddScoped<ICouleurService<CouleurDTO>, CouleurWebService>();
builder.Services.AddScoped<IFavorisService<FavorisDTO>, FavorisWebService>();
builder.Services.AddScoped<IAnnonceService<AnnonceDTO>, AnnonceWebService>();
builder.Services.AddScoped<IUtilisateurService, UtilisateurWebService>();
builder.Services.AddScoped<IAbonnementService<AbonnementDTO>, AbonnementWebService>();
builder.Services.AddScoped<IMotsInterditsService, MotsInterditWebService>();
builder.Services.AddScoped<IMediasService<PhotoResponseDTO>, MediaWebService>();
builder.Services.AddScoped<VerificationService>();
builder.Services.AddScoped<ClipboardService>();
builder.Services.AddScoped<INotificationService>();
builder.Services.AddScoped<IVisualisationService, VisualisationWebService>();

// builder.Services.AddScoped<ISignalRService,ChatSignalRService>();
builder.Services.AddSingleton<ISignalRService>(sp =>
{
    return new SignalRWebService();
});

builder.Services.AddScoped<INoteUtilisateurService<NoteUtilisateurDetailDTO>, NoteUtilisateurWebService>();

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
builder.Services.AddScoped<VerificationViewModel>();
builder.Services.AddScoped<NavBarViewModel>();
builder.Services.AddScoped<SignalementViewModel>();
builder.Services.AddScoped<TraitementSignalementViewModel>();
// HttpClient AVEC CREDENTIALS (cookies)
builder.Services.AddScoped(sp => { return new HttpClient { BaseAddress = new Uri("http://localhost:5096/api/") }; });

await builder.Build().RunAsync();