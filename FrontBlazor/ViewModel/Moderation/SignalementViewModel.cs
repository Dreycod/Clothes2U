using FrontBlazor.Components.Moderation;
using Shared.DTO;
using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.DTO.Photo;
using Shared.DTO.Signalement;

namespace FrontBlazor.ViewModel.Moderation;

public class SignalementViewModel : ModerationViewModel
{
    private readonly ISignalementService _signalementService;
    private readonly IMediasService _mediaService;
    private readonly NavigationManager _nav;
    
    public SignalementViewModel(ISignalementService signalementService,
        IAuthService authService,
        IMediasService mediasService,
        NavigationManager nav) : base(authService, nav)
    {
        _signalementService = signalementService;
        _nav = nav;
    }
    
    public List<SignalementDTO> Signalements { get; set; } = new();
    
    public int SelectedCategoryId { get; set; } = 0;

    public async override Task LoadAsync()
    {
        base.LoadAsync();
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
        await LoadAsync();
    }

    public async Task VoirSignalement(int id)
    {
        _nav.NavigateTo($"/moderation/signalements/{id}");
    }
}