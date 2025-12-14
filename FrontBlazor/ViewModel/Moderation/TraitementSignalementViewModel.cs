using FrontBlazor.Models;
using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel.Moderation.Signalements;

public class TraitementSignalementViewModel : ModerationViewModel
{
    private readonly ISignalementService _signalementService;
    private readonly IReadableService<UtilisateurView> _utilisateurService;
    private readonly IAnnonceService<Annonce>  _annonceService;
    private readonly INoteUtilisateurService<NoteUtilisateur> _noteUtilisateurService;
    private readonly INotificationService _notificationService;
    

    public TraitementSignalementViewModel(
        ISignalementService signalementService,
        IReadableService<UtilisateurView> utilisateurService,
        IAnnonceService<Annonce> annonceService,
        INoteUtilisateurService<NoteUtilisateur> noteUtilisateurService,
        INotificationService notificationService,
        IAuthService authService,
        NavigationManager nav)
        : base(authService, nav)
    {
        _utilisateurService =  utilisateurService;
        _notificationService = notificationService;
        _annonceService = annonceService;
        _noteUtilisateurService = noteUtilisateurService;
        _signalementService = signalementService;
    }
    
    public SignalementDetails Signalement { get; set; }
    public NoteUtilisateur avis { get; set; }
    public Annonce annonce { get; set; }
    public UtilisateurView utilisateurSignale { get; set; }


    public async Task LoadSignalementAsync(int id)
    {
        await base.LoadAsync();
        Signalement =  await _signalementService.GetSignalementByIdAsync(id);
        switch (Signalement)
        {
            case SignalementAnnonce sa:
                annonce = await _annonceService.GetAnnonceDetailById(sa.AnnonceSignaleeId);
                break;
            
            case SignalementAvis sav:
                avis = await _noteUtilisateurService.GetByIdAsync(sav.AvisId);
                break;
        }
        utilisateurSignale = await _utilisateurService.GetByIdAsync(Signalement.UtilisateurSignaleId);
    }

    public async Task SendWarning(string messageAvertissement)
    {
        try 
        {
            await _notificationService.CreateNotificationAvertissement(
                messageAvertissement, 
                utilisateurSignale.UtilisateurId 
            );
        
            Console.WriteLine("Notification envoyée avec succès");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'envoi : {ex.Message}");
        }
    }

    public async Task ShowSuspendModal()
    {
        throw new NotImplementedException();
    }

    public async Task ShowBanModal()
    {
        throw new NotImplementedException();
    }

    public async Task DismissReport()
    {
        throw new NotImplementedException();
    }
}