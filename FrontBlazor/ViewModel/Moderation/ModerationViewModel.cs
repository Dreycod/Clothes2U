using Shared.DTO.Utilisateur;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;


namespace FrontBlazor.Pages.Moderation;

public abstract class ModerationViewModel
{
    private readonly IAuthService _authService;
    private readonly NavigationManager _nav;
    public bool IsModerator { get; set; }
    
    public bool IsLoading { get; set; }

    public ModerationViewModel( IAuthService authService,  NavigationManager nav)
    {
        _authService = authService;
        _nav = nav;
    }

    public virtual async Task LoadAsync()
    {
        IsLoading = true;
        IsModerator = false;
        CurrentUtilisateurDTO user = (CurrentUtilisateurDTO)await _authService.GetCurrentUserAsync();
        if (user != null && (user.RoleUtilisateur == "Admin" || user.RoleUtilisateur == "Moderateur"))
        {
            IsModerator = true;
            IsLoading = false;
        }
        else
        {
            _nav.NavigateTo("/");
        }
    }
}