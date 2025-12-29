using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.DTO;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.ViewModel;

public class SanctionedUserViewModel
{
    private readonly IAuthService _authService;
    private readonly IDemandeRestaurationService _demandeRestaurationService;
    private readonly NavigationManager _navigationManager;

    public SanctionedUserViewModel(
        IAuthService authService, 
        IDemandeRestaurationService demandeRestaurationService,
        NavigationManager navigationManager)
    {
        _authService = authService;
        _demandeRestaurationService = demandeRestaurationService;
        _navigationManager = navigationManager;
    }

    public bool IsLoading { get; set; }
    public bool IsSubmitting { get; set; }
    public bool ShowDemandeForm { get; set; }
    public bool DemandeSubmitted { get; set; }
    public string MessageDemande { get; set; } = "";
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public UtilisateurDTO? CurrentUser { get; set; }
    
    public async Task LoadAsync()
    {
        IsLoading = true;
        NotifyStateChanged();
        
        CurrentUser = await _authService.GetCurrentUserAsync();
        if (CurrentUser == null || (CurrentUser.StatutId != 2 && CurrentUser.StatutId != 3))
        {
            _navigationManager.NavigateTo("/");
            return;
        }
        
        IsLoading = false;
        NotifyStateChanged();
    }

    public void ToggleDemandeForm()
    {
        ShowDemandeForm = !ShowDemandeForm;
        ErrorMessage = null;
        MessageDemande = "";
        NotifyStateChanged();
    }

    public async Task<bool> SoumettreDemandeAsync()
    {
        if (string.IsNullOrWhiteSpace(MessageDemande))
        {
            ErrorMessage = "Veuillez saisir un message pour votre demande.";
            NotifyStateChanged();
            return false;
        }

        if (MessageDemande.Length < 20)
        {
            ErrorMessage = "Votre message doit contenir au moins 20 caractères.";
            NotifyStateChanged();
            return false;
        }
        IsSubmitting = true;
        ErrorMessage = null;
        SuccessMessage = null;
        NotifyStateChanged();
        var response = await _demandeRestaurationService.AddDemandeRestauration(MessageDemande);
        IsSubmitting = false;
        if (!response.Success)
        {
            ErrorMessage = response.ErrorMessage;
            NotifyStateChanged();
            return false;
        }
        DemandeSubmitted = true;
        ShowDemandeForm = false;
        SuccessMessage = "Votre demande de restauration a été envoyée avec succès. Notre équipe de modération l'examinera dans les plus brefs délais.";
        MessageDemande = "";
        NotifyStateChanged();
        return true;
    }

    public async Task HandleLogout()
    {
        await _authService.LogoutAsync();
        _navigationManager.NavigateTo("/", true);
    }

    public event Action? OnStateChanged;
    
    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }
}