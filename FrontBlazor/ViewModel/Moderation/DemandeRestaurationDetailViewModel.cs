using System.ComponentModel;
using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.DTO.Annonce;
using Shared.DTO.Conversation;
using Shared.DTO.Decision;
using Shared.DTO.DemandeRestauration;
using Shared.DTO.Message;
using Shared.DTO.NoteUtilisateur;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.ViewModel.Moderation;

public class DemandeRestaurationDetailViewModel : ModerationViewModel, INotifyPropertyChanged
{
    private readonly IUtilisateurService _utilisateurService;
    private readonly IDemandeRestaurationService _demandeRestaurationService;
    private readonly IDecisionService _decisionService;
    private readonly IAnnonceService _annonceService;
    private readonly INoteUtilisateurService _noteUtilisateurService;
    private readonly IConversationService<ConversationDTO> _conversationService;
    private readonly NavigationManager _nav;
    private readonly IMediasService _mediasService;
    public event PropertyChangedEventHandler PropertyChanged;
    public event Action OnStateChanged;

    public DemandeRestaurationDetailViewModel(
        IUtilisateurService utilisateurService,
        IDemandeRestaurationService demandeRestaurationService,
        IDecisionService decisionService,
        IConversationService<ConversationDTO> conversationService,
        IAnnonceService annonceService,
        IMediasService mediasService,
        INoteUtilisateurService noteUtilisateurService,
        IAuthService authService,
        NavigationManager nav)
        : base(authService, nav)
    {
        _utilisateurService = utilisateurService;
        _annonceService = annonceService;
        _conversationService = conversationService;
        _decisionService =  decisionService;
        _noteUtilisateurService = noteUtilisateurService;
        _mediasService = mediasService;
        _nav = nav;
    }
    public bool IsLoading { get; set; }
    public DecisionDetailDTO Decision { get; set; }
    public DemandeRestaurationDetailDTO Demande{get; set; }
    public UtilisateurViewDTO Utilisateur { get; set; }
    public NoteUtilisateurDetailDTO Avis { get; set; }
    public AnnonceDetailDTO Annonce { get; set; }
    public MessageSignalementDTO Message { get; set; }
    public UtilisateurViewDTO UtilisateurSignale { get; set; }
    public string? PhotoProfilUrl { get; set; }
    public List<string> PhotosUrl { get; set; } = new();

    public async Task LoadDemandeAsync(int id)
    {
        IsLoading = true;
        await base.LoadAsync();
        Demande = await _demandeRestaurationService.GetDemandeRestaurationDetail(id);
        Decision = await _decisionService.GetDecisionDetailAsync(Demande.DecisionId);
        Utilisateur = await _utilisateurService.GetUserById(Demande.DecisionId);
        switch (Decision.ElementDecision)
        {
            case ElementDecisionAnnonceDTO ea:
                Annonce = await _annonceService.GetAnnonceDetailById(ea.AnnonceId);
                PhotosUrl = await GetPhotosUrl(Annonce.Photos);
                break;
            case ElementDecisionMessageDTO em:
                Message = await _conversationService.GetMessageById(em.MessageId);
                PhotosUrl = await GetPhotosUrl(Message.Photos);
                break;
            case ElementAvisDTO eavis :
                Avis = await _noteUtilisateurService.GetByIdAsync(eavis.AvisId);
                break;
        }
        Utilisateur = await _utilisateurService.GetUserById(Demande.UtilisateurId);
        PhotoProfilUrl = await GetPhotoProfilUrl(); 
        IsLoading = false;
    }
    
    
    
    private async Task<List<string>> GetPhotosUrl(List<int> photosId)
    {
        List<string> photoUrls = new List<string>();
        foreach (int photoId in photosId)
        {
            photoUrls.Add(_mediasService.GetPhotoUrl(photoId));
        }
        return photoUrls;
    }

    private async Task<string?> GetPhotoProfilUrl()
    {
        if (Utilisateur.PhotoProfilId == null)
        {
            return null;
        }
        return _mediasService.GetPhotoUrl(Utilisateur.PhotoProfilId);
    }

}