using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.DTO;
using Shared.DTO.Annonce;
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
    private readonly IAnnonceService _annonceService;
    public List<(int PhotoID, string PreviewBase64, string Username, string AnnonceID)> SelectedFilePreviews { get; set; } = new();
    public List<string> _ValidationMessage { get; set; } = new();
    public bool _IsSuccess;

    public ImagesValidationViewModel(
        IAuthService authService,
        IMediasService mediasService,
        IAnnonceService annonceService,
        NavigationManager nav
        ) : base(authService, nav)
    {
        _mediasService = mediasService;
        _nav = nav;
        _annonceService = annonceService;
    }

    public bool IsLoading { get; set; }
    public List<DemandeRestaurationDTO> DemandeRestaurations { get; set; } = new();

    public override async Task LoadAsync()
    {
        IsLoading = true;
        base.LoadAsync();

        List<PhotoDTO> photoDTOs = await _mediasService.GetAllPhotosValidation();
       
        Console.WriteLine($"✅ Photos à valider chargées: {photoDTOs.Count}");

        if (photoDTOs.Count == 0)
        {
            await ShowMessage("Aucune image en attente de validation.", true);
        }
        else
        {
            IEnumerable<AnnonceDTO> annonces = await _annonceService.GetAnnoncesByPhotoIDs(
                photoDTOs.Select(p => p.PhotoId).ToList()
            );

            var annoncesByPhotoId = new Dictionary<int, AnnonceDTO>();

            foreach (var annonce in annonces)
            {
                foreach (var photoId in annonce.Photos) 
                {
                    if (!annoncesByPhotoId.ContainsKey(photoId))
                        annoncesByPhotoId[photoId] = annonce;
                }
            }

            SelectedFilePreviews = photoDTOs.Select(p =>
            {
                if (annoncesByPhotoId.TryGetValue(p.PhotoId, out var annonce))
                {
                    return (
                        PhotoID: p.PhotoId,
                        PreviewBase64: $"data:image/jpeg;base64,{Convert.ToBase64String(p.Image)}",
                        Username: annonce.NomAuteur,
                        AnnonceID: annonce.AnnonceId.ToString()
                    );
                }
                else
                {
                    // Fallback if no annonce exists for this photo
                    return (
                        PhotoID: p.PhotoId,
                        PreviewBase64: $"data:image/jpeg;base64,{Convert.ToBase64String(p.Image)}",
                        Username: "Unknown",
                        AnnonceID: "0"
                    );
                }
            }).ToList();

        }
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

            await ShowMessage($"Image ID: {imageId} approuvée ✅", true);
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

            await ShowMessage($"Image ID: {imageId} refusée ❌", true);
        }
        else
        {
            await ShowMessage("Erreur lors du refus ⚠️", false);
        }
    }

    private async Task ShowMessage(string message, bool success)
    {
        _IsSuccess = success;

        _ValidationMessage ??= new List<string>();
        _ValidationMessage.Add(message);
        NotifyStateChanged();

        await Task.Delay(10000);

        _ValidationMessage.Remove(message);
        NotifyStateChanged();
    }




}