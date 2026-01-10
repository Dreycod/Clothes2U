using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.ViewModel.Commercial;

public abstract class BaseCommercialViewModel
{
    private readonly IAuthService _authService;
    private readonly NavigationManager _nav;
    public bool IsCommercial { get; set; }

    public bool IsLoading { get; set; }

    public BaseCommercialViewModel(IAuthService authService, NavigationManager nav)
    {
        _authService = authService;
        _nav = nav;
    }

    public virtual async Task LoadAsync()
    {
        IsLoading = true;
        IsCommercial = false;

        CurrentUtilisateurDTO user = (CurrentUtilisateurDTO)await _authService.GetCurrentUserAsync();
        if (user != null && user.RoleUtilisateur == "Admin" || user.RoleUtilisateur == "Commercial")
        {
            IsCommercial = true;
        }
        else
        {
            _nav.NavigateTo("/");
        }
        IsLoading = false;
    }
}
