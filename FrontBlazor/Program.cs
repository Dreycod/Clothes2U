using FrontBlazor;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.Services.Interfaces.GenericIServices;
using FrontBlazor.Services.WebService;
using FrontBlazor.ViewModel;
using FrontBlazor.ViewModel.Generic;
using FrontBlazor.ViewModel.Moderation;
using FrontBlazor.ViewModel.Moderation.Signalements;
using FrontBlazor.ViewModel.Moderation.Support;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Shared.DTO.Abonnement;
using Shared.DTO.Annonce;
using Shared.DTO.Categorie;
using Shared.DTO.Conversation;
using Shared.DTO.Couleur;
using Shared.DTO.Favoris;
using Shared.DTO.NoteUtilisateur;
using Shared.DTO.Photo;
using Shared.DTO.Recense;
using Shared.DTO.Tag;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(typeof(IListableService<>), typeof(ListableService<>));
builder.Services.AddScoped(typeof(IReadableService<>), typeof(ReadableService<>));
builder.Services.AddScoped(typeof(IWritableService<>), typeof(WritableService<>));

// Services
builder.Services.AddScoped<IAuthService, AuthWebService>();
builder.Services.AddScoped<ITailleService, TailleWebService>();
builder.Services.AddScoped<INotificationService, NotificationWebService>();
builder.Services.AddScoped<ActivityService>();
builder.Services.AddScoped<ISignalementService, SignalementWebService>();
builder.Services.AddScoped<IBloqueService, BloqueWebService>();
builder.Services.AddScoped<ICategorieService<CategorieDTO>, CategorieWebService>();
builder.Services.AddScoped<IConversationService<ConversationDTO>, ConversationWebService>();
builder.Services.AddScoped<IMessageService, MessageWebService>();
builder.Services.AddScoped<ICouleurService<CouleurDTO>, CouleurWebService>();
builder.Services.AddScoped<IFavorisService<FavorisDTO>, FavorisWebService>();
builder.Services.AddScoped<IAnnonceService, AnnonceWebService>();
builder.Services.AddScoped<IUtilisateurService, UtilisateurWebService>();
builder.Services.AddScoped<IAbonnementService, AbonnementWebService>();
builder.Services.AddScoped<IMotsInterditsService, MotsInterditWebService>();
builder.Services.AddScoped<IMediasService, MediaWebService>();
builder.Services.AddScoped<IStateService<AnnonceDTO>, AnnonceStateService>();
builder.Services.AddScoped<VerificationService>();
builder.Services.AddScoped<IModerationDashboardService, ModerationDashBoardWebService>();
builder.Services.AddScoped<ClipboardService>();
builder.Services.AddScoped<IVisualisationService, VisualisationWebService>();
builder.Services.AddScoped<IDecisionService, DecisionWebService>();
builder.Services.AddScoped<IDemandeRestaurationService, DemandeRestaurationWebService>();
builder.Services.AddScoped<IOrderService, OrderWebService>();
builder.Services.AddScoped<IPaymentService, PaymentWebService>();
builder.Services.AddScoped<IMarqueService, MarqueWebService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<StripeWebService>();
builder.Services.AddScoped<IRecenseService<RecenseDetailDTO>, RecenseWebService>();
builder.Services.AddScoped<ITagService<TagDTO>, TagWebService>();
builder.Services.AddScoped<PasswordResetWebService>();
builder.Services.AddScoped<SignalRHandlerWebService>();
builder.Services.AddScoped<SupportWebService>();
builder.Services.AddScoped<ISupportService, SupportWebService>();


//caracteristiques
builder.Services.AddScoped(typeof(ICaracteristiqueService<>), typeof(CaracteristiqueService<>));
// builder.Services.AddScoped<ISignalRService,ChatSignalRService>();
builder.Services.AddSingleton<ISignalRService>(sp =>
{
    return new SignalRWebService();
});

builder.Services.AddScoped<INoteUtilisateurService, NoteUtilisateurWebService>();
builder.Services.AddScoped(typeof(WritableService<>));
// ViewModels
builder.Services.AddScoped<LoginViewModel>();
builder.Services.AddScoped<SearchAnnonceViewModel>();
builder.Services.AddScoped<ProfilViewModel>();
builder.Services.AddScoped<HomeViewModel>();
builder.Services.AddScoped<MessagerieViewModel>();
builder.Services.AddScoped<DetailAnnonceViewModel>();
builder.Services.AddScoped<CommercialCategoriesViewModel>();
builder.Services.AddScoped<CommercialCouleursViewModel>();
builder.Services.AddScoped<CommercialMarquesViewModel>();
builder.Services.AddScoped<CommercialMessagesViewModel>();
builder.Services.AddScoped<CommercialSousCategoriesViewModel>();
builder.Services.AddScoped<CommercialTaillesViewModel>();
builder.Services.AddScoped<CommercialViewModel>();
builder.Services.AddScoped<ModerationBoardViewModel>();
builder.Services.AddScoped<MotsInterditsViewModel>();
builder.Services.AddScoped<VerificationViewModel>();
builder.Services.AddScoped<SignalementViewModel>();
builder.Services.AddScoped<TraitementSignalementViewModel>();
builder.Services.AddScoped<SanctionsViewModel>();
builder.Services.AddScoped<SanctionedUserViewModel>();
builder.Services.AddScoped<ImagesValidationViewModel>();
builder.Services.AddScoped<DemandesRestaurationViewModel>();
builder.Services.AddScoped<DemandeRestaurationDetailViewModel>();
builder.Services.AddScoped<HistoriqueTransactionViewModel>();
builder.Services.AddScoped<SettingsViewModel>();
builder.Services.AddScoped<UpdateAnnonceViewModel>();
builder.Services.AddScoped<CreationAnnonceViewModel>();
builder.Services.AddScoped<AcheterViewModel>();
builder.Services.AddScoped<ClientBaseViewModel>();
builder.Services.AddScoped<AddressViewModel>();
builder.Services.AddScoped<ForgotPasswordViewModel>();
builder.Services.AddScoped<ResetPasswordViewModel>();
builder.Services.AddScoped<OrderViewModel>();
builder.Services.AddScoped<CreateSupportTicketViewModel>();
builder.Services.AddScoped<TicketDetailViewModel>();
builder.Services.AddScoped<SupportTicketsViewModel>();

// AuthService doit déjà être enregistré
// (il contient les méthodes pour les adresses)

// HttpClient AVEC CREDENTIALS (cookies)
builder.Services.AddScoped(sp =>
{
    var baseAddress =
        builder.HostEnvironment.IsDevelopment()
            ? "http://localhost:5096/api/"
            : "https://apisae-anfegsddaabjavaa.francecentral-01.azurewebsites.net/api/";

    return new HttpClient
    {
        BaseAddress = new Uri(baseAddress)
    };
});

await builder.Build().RunAsync();