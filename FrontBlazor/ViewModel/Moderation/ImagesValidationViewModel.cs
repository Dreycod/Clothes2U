using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.DTO.DemandeRestauration;
using Shared.DTO.Moderation;

namespace FrontBlazor.ViewModel;

public class ImagesValidationViewModel : ModerationViewModel
{
    private readonly NavigationManager _nav;
    private readonly IMediasService _mediasService;
    public List<(IBrowserFile File, string PreviewBase64)> SelectedFilePreviews { get; set; } = new();

    public ImagesValidationViewModel(
        IAuthService authService,
        IMediasService mediasService,
        NavigationManager nav
        ) : base(authService, nav)
    {
        _mediasService = mediasService;
        _nav = nav;
    }

    public bool IsLoading { get; set; }
    public List<DemandeRestaurationDTO> DemandeRestaurations { get; set; } = new();

    public override async Task LoadAsync()
    {
        IsLoading = true;
        base.LoadAsync();
        IsLoading = false;
    }
    public event Action? OnStateChanged;

    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }

    public void NavigateTo(string url)
    {
        _nav.NavigateTo(url);
    }
    public void ApproveImage(int imageId)
    {
        var result = _mediasService.ValidationImageAsync(true,imageId);
        if (result != null)
        {
            // Logique supplémentaire si nécessaire
        }
    }

    public void RejectImage(int imageId)
    {
        var result = _mediasService.ValidationImageAsync(false, imageId);
        if (result != null)
        {
            // Logique supplémentaire si nécessaire
        }
    }
}