using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.DTO;
using Shared.DTO.DemandeRestauration;
using Shared.DTO.Moderation;
using Shared.DTO.Photo;
using System.Diagnostics;
using System.Net.Mime;

namespace FrontBlazor.ViewModel;

public class ImagesValidationViewModel : ModerationViewModel
{
    private readonly NavigationManager _nav;
    private readonly IMediasService _mediasService;
    public List<(int PhotoID, string PreviewBase64)> SelectedFilePreviews { get; set; } = new();
    public List<string> _ValidationMessage;
    public bool _IsSuccess;



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
        Console.WriteLine($"test");

        IsLoading = true;
        base.LoadAsync();

        List<PhotoDTO> photoDTOs = await _mediasService.GetAllPhotosValidation();

        Console.WriteLine($"✅ Photos à valider chargées: {photoDTOs.Count}");

        SelectedFilePreviews = photoDTOs
            .Select(p => (
                PhotoID: p.PhotoId,
                PreviewBase64: $"data:image/jpeg;base64,{Convert.ToBase64String(p.Image)}"
            ))
            .ToList();

        IsLoading = false;

        NotifyStateChanged();

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
    public async Task ApproveImage(int imageId)
    {
        var result = await _mediasService.ValidationImageAsync(true, imageId);

        if (result != null)
        {
            SelectedFilePreviews
                .RemoveAll(p => p.PhotoID == imageId);
            NotifyStateChanged();

            await ShowMessage("Image approuvée ✅", true);
        }
        else
        {
            await ShowMessage("Erreur lors de l’approbation ❌", false);
        }
    }

    public async Task RejectImage(int imageId)
    {
        var result = await _mediasService.ValidationImageAsync(false, imageId);

        if (result != null)
        {
            SelectedFilePreviews
                .RemoveAll(p => p.PhotoID == imageId);
            NotifyStateChanged();

            await ShowMessage("Image refusée ❌", true);
        }
        else
        {
            await ShowMessage("Erreur lors du refus ⚠️", false);
        }
    }

    private async Task ShowMessage(string message, bool success)
    {
        _IsSuccess = success;

        _ValidationMessage ??= new List<string>(); // extra safety
        _ValidationMessage.Add(message);
        NotifyStateChanged();

        await Task.Delay(10000);

        _ValidationMessage.Remove(message);
        NotifyStateChanged();
    }




}