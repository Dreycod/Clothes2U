using FrontBlazor.Components.Moderation;
using FrontBlazor.Models;
using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel.Moderation;

public class SignalementViewModel : ModerationViewModel
{
    private readonly ISignalementService _signalementService;
    private readonly IMediasService<Photo> _mediaService;
    private readonly NavigationManager _nav;
    
    public SignalementViewModel(ISignalementService signalementService,
        IAuthService authService,
        IMediasService<Photo> mediasService,
        NavigationManager nav) : base(authService, nav)
    {
        _signalementService = signalementService;
    }
    
    public List<Signalement> Signalements { get; set; } = new();
    
    public int SelectedCategoryId { get; set; } = 0; 

    public async Task LoadSignalements()
    {
        IsLoading = true;
        try
        {
            if (SelectedCategoryId == 0)
            {
                Signalements = await _signalementService.GetAllAsync();
            }
            else
            {
                Signalements = await _signalementService.GetAllByType(SelectedCategoryId);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task ChangeCategory(int categoryId)
    {
        SelectedCategoryId = categoryId;
        await LoadSignalements();
    }

    public async Task VoirSignalement(int id)
    {
        _nav.NavigateTo($"/moderation/signalements/{id}");
    }
}