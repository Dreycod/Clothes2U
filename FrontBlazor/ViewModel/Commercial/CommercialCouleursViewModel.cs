using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using Shared.DTO;
using Shared.DTO.Couleur;

namespace FrontBlazor.ViewModel;
public class CommercialCouleursViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public CouleurDTO currentCouleur = new CouleurDTO();
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

    public ListableViewModel<CouleurDTO> VM_Couleur { get; set; }
    public WritableService<CouleurDTO> CouleurService { get; set; }
    public event Action? OnStateChange;

    public CommercialCouleursViewModel(ListableViewModel<CouleurDTO> _CouleurViewModel, WritableService<CouleurDTO> couleurService)
    {
        VM_Couleur = _CouleurViewModel;
        CouleurService = couleurService;
    }
    public async Task LoadAsync()
    {
        await VM_Couleur.LoadAsync();
        if (VM_Couleur.Items != null)
        {
            VM_Couleur.Items = VM_Couleur.Items
                .OrderBy(c => c.CouleurId)
                .ToList();
        }
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
                await CouleurService.UpdateAsync(currentCouleur);
                successMessage = "Couleur modifiée avec succès";
            }
            else
            {
                await CouleurService.AddAsync(currentCouleur);
                successMessage = "Couleur ajoutée avec succès";
            }

            CloseModal();
            await VM_Couleur.LoadAsync();
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
            await CouleurService.DeleteAsync(currentCouleur.CouleurId);
            successMessage = $"Couleur {currentCouleur.Nom} supprimée avec succès";
            CloseDeleteModal();
            await VM_Couleur.LoadAsync();
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
