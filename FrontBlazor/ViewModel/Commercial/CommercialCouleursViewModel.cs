using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Commercial;
using Microsoft.AspNetCore.Components;
using Shared.DTO;
using Shared.DTO.Couleur;

namespace FrontBlazor.ViewModel;
public class CommercialCouleursViewModel: BaseCommercialViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public CouleurDTO currentCouleur = new CouleurDTO();
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

    public ICaracteristiqueService<CouleurDTO> _couleurService { get; set; }
    public event Action? OnStateChange;
    public bool IsLoading { get; set; }

    public List<CouleurDTO> Couleurs { get; set; } 
    public CommercialCouleursViewModel(ICaracteristiqueService<CouleurDTO> couleurViewModel, IAuthService authService, NavigationManager nav) : base(authService, nav)
    {
        _couleurService =  couleurViewModel;
    }
    public async Task LoadAsync()
    {
        IsLoading = true;
        await base.LoadAsync();
        Couleurs = await _couleurService.GetAllAsync();
        if (Couleurs != null)
        {
            Couleurs= Couleurs
                .OrderBy(c => c.CouleurId)
                .ToList();
        }
        IsLoading = false;
    }

    public void ShowAddModal()
    {
        isEditing = false;
        currentCouleur = new CouleurDTO();
        showModal = true;
    }

    public void ShowEditModal(CouleurDTO couleur)
    {
        isEditing = true;
        currentCouleur = new CouleurDTO
        {
            CouleurId = couleur.CouleurId,
            Nom = couleur.Nom
        };
        showModal = true;
    }

    public void ShowDeleteModal(CouleurDTO couleur)
    {
        currentCouleur = couleur;
        showDeleteModal = true;
    }

    public void CloseModal()
    {
        showModal = false;
        currentCouleur = new CouleurDTO();
        errorMessage = string.Empty;
    }

    public void CloseDeleteModal()
    {
        showDeleteModal = false;
        currentCouleur = new CouleurDTO();
    }

    public async Task SaveCouleur()
    {
        if (string.IsNullOrWhiteSpace(currentCouleur.Nom))
        {
            errorMessage = "Le nom de la couleur est requis";
            return;
        }

        try
        {
            if (isEditing)
            {
                await _couleurService.UpdateAsync(currentCouleur);
                successMessage = "Couleur modifiée avec succès";
            }
            else
            {
                await _couleurService.AddAsync(currentCouleur);
                successMessage = "Couleur ajoutée avec succès";
            }

            CloseModal();
            Couleurs = await _couleurService.GetAllAsync();
            OnStateChange?.Invoke();

            // Clear success message after 3 seconds
            await Task.Delay(3000);
            successMessage = string.Empty; 
            OnStateChange?.Invoke();

        }
        catch (Exception ex)
        {
            errorMessage = $"Erreur: {ex.Message}";
        }
    }

    public async Task DeleteCouleur()
    {
        try
        {
            await _couleurService.DeleteAsync(currentCouleur.CouleurId);
            successMessage = $"Couleur {currentCouleur.Nom} supprimée avec succès";
            CloseDeleteModal();
            await _couleurService.GetAllAsync();
            OnStateChange?.Invoke();

            // Clear success message after 3 seconds
            await Task.Delay(3000);
            successMessage = string.Empty;
            OnStateChange?.Invoke();
        }
        catch (Exception ex)
        {
            errorMessage = $"Erreur: {ex.Message}";
            CloseDeleteModal();
        }
    }
}
