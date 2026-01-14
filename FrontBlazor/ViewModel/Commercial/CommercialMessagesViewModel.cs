using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Commercial;
using Microsoft.AspNetCore.Components;
using Shared.DTO.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace FrontBlazor.ViewModel;

public class CommercialMessagesViewModel : BaseCommercialViewModel
{
    // Services (à injecter via DI)
    private readonly IMessageService _messageService;
    private readonly INotificationService _notificationService;
    private readonly NavigationManager _navigationManager;

    // Events
    public event Action? OnStateChange;

    // Properties
    public bool IsLoading { get; set; }
    public string successMessage { get; set; } = string.Empty;
    public string errorMessage { get; set; } = string.Empty;

    //// Message data
    public NotificationCommercialCreateDTO notificationCommercial = new NotificationCommercialCreateDTO();

    // Modal states
    public bool showEditModal { get; set; }
    public bool showDeleteModal { get; set; }

    // Constructor
    public CommercialMessagesViewModel(
        IMessageService messageService,
        INotificationService notificationService,
        IAuthService authService, NavigationManager navigationManager) : base(authService, navigationManager)
    {
        _messageService = messageService;
        _notificationService = notificationService;
        _navigationManager = navigationManager;
    }

    // Load messages history
    public async Task LoadAsync()
    {
        try
        {
            IsLoading = true;
            await base.LoadAsync();
            ClearMessages();
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            errorMessage = $"Erreur lors du chargement de l'historique : {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    // Send new message
    public async Task SendMessage()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(notificationCommercial.CommercialTitle) ||
                string.IsNullOrWhiteSpace(notificationCommercial.CommercialText))
            {
                errorMessage = "Veuillez remplir tous les champs obligatoires.";
                NotifyStateChanged();
                return;
            }

            ClearMessages();
            IsLoading = true;
            NotifyStateChanged();
            

            await _notificationService.PostCommercialNotification(notificationCommercial);
            successMessage = "Message envoyé avec succès à tous les utilisateurs !";
            notificationCommercial = new NotificationCommercialCreateDTO();
        }
        catch (Exception ex)
        {
            errorMessage = $"Erreur lors de l'envoi du message : {ex.Message}";
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    // Clear form
    public void ClearForm()
    {
        notificationCommercial = new NotificationCommercialCreateDTO();
        ClearMessages();
        NotifyStateChanged();
    }

    // Helper methods
    private void ClearMessages()
    {
        successMessage = string.Empty;
        errorMessage = string.Empty;
    }
    public void OnCommercialTextChanged(ChangeEventArgs e)
    {
        notificationCommercial.CommercialText = e.Value?.ToString() ?? "";
        NotifyStateChanged();
    }

    private void NotifyStateChanged()
    {
        OnStateChange?.Invoke();
    }
    public string CommercialText
    {
        get => notificationCommercial.CommercialText;
        set
        {
            if (notificationCommercial.CommercialText == value) return;
            notificationCommercial.CommercialText = value;
            NotifyStateChanged();
        }
    }

}
