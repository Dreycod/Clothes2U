using Shared.DTO.MotInterdit;
using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;

public class MotsInterditsViewModel : ModerationViewModel
{
    public List<MotInterditDTO> Mots { get; set; } = new List<MotInterditDTO>(); 
    private readonly IMotsInterditsService _motsInterditsService;
    public MotInterditDTO motToAdd { get; set; } = new MotInterditDTO(); 
    public string? ErrorMessage { get; set; } 

    public MotsInterditsViewModel(
        IMotsInterditsService motsInterditsService,
        IAuthService authService,
        NavigationManager nav) : base(authService, nav)
    {
        _motsInterditsService = motsInterditsService;
    }
    
    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();

    public override async Task LoadAsync()
    {
        await base.LoadAsync(); 
        IsLoading = true;
        motToAdd = new MotInterditDTO();
        
        try
        {
            Mots = await _motsInterditsService.GetAllAsync();
            Console.WriteLine($"Mots récupérés: {Mots?.Count ?? 0}");
            
            if (Mots == null)
            {
                Mots = new List<MotInterditDTO>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement: {ex.Message}");
            Mots = new List<MotInterditDTO>();
        }
        
        IsLoading = false;
        NotifyStateChanged();
    }

    public async Task DeleteMotAsync(int id)
    {
        IsLoading = true;
        try
        {
            await _motsInterditsService.DeleteAsync(id);
            await LoadAsync();
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la suppression: {ex.Message}");
        }
        IsLoading = false;
    }

    public async Task<bool> AddMotInterditAsync()
    {
        if (string.IsNullOrWhiteSpace(motToAdd.LibelleMot))
        {
            ErrorMessage = "Le mot ne peut pas être vide.";
            NotifyStateChanged();
            return false;
        }
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var (addedMot, error) = await _motsInterditsService.AddAsync(motToAdd);

            if (error != null)
            {
                ErrorMessage = error;
                NotifyStateChanged();
                IsLoading = false;
                return false;
            }

            if (addedMot != null)
            {
                Mots.Add(addedMot);
                motToAdd = new MotInterditDTO();
                NotifyStateChanged();
                IsLoading = false;
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors de l'ajout : {ex.Message}";
            Console.WriteLine($"Erreur lors de l'ajout d'un mot interdit : {ex.Message}");
            NotifyStateChanged();
            IsLoading = false;
            return false;
        }
    }
}