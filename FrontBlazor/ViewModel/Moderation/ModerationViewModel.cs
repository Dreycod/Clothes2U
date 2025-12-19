using Shared.DTO.Utilisateur;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;


namespace FrontBlazor.Pages.Moderation;

public abstract class ModerationViewModel
{
    private readonly IAuthService _authService;
    private readonly NavigationManager _nav;
    
    public bool IsLoading { get; set; }

    public ModerationViewModel( IAuthService authService,  NavigationManager nav)
    {
        _authService = authService;
        _nav = nav;
    }

    public virtual async Task LoadAsync()
    {
        IsLoading = true;
        UtilisateurViewDTO user = (UtilisateurViewDTO)await _authService.GetCurrentUserAsync();
        if (user == null || user.RoleUtilisateur != "Admin" && user.RoleUtilisateur != "Modérateur")
        {
            _nav.NavigateTo("/");
        }
        IsLoading = false;
    }
}