using Shared.DTO;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using Shared.DTO.Taille;
using Shared.DTO.Categorie;

namespace FrontBlazor.ViewModel;

public class CommercialTaillesViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public TailleDTO currentTaille = new TailleDTO();
    public int selectedCategorieId = 0;
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

    private ListableViewModel<TailleDTO> VM_Taille;
    private WritableService<TailleDTO> TailleService;
    private ListableViewModel<CategorieDTO> VM_Categorie;
    public event Action? OnStateChange;

    public CommercialTaillesViewModel(ListableViewModel<TailleDTO> tailleService, ListableViewModel<CategorieDTO> categorieService, WritableService<TailleDTO> _tailleService )
    {
        VM_Taille = tailleService;
        VM_Categorie = categorieService;
        TailleService = _tailleService;
    }
    public async Task LoadAsync()
    {
        await VM_Taille.LoadAsync();
        if (VM_Taille.Items != null)
        {
            VM_Taille.Items = VM_Taille.Items
                .OrderBy(c => c.TailleId)
                .ToList();
        }
        await VM_Categorie.LoadAsync();
    }

    public void ShowAddModal()
    {
        isEditing = false;
        currentTaille = new TailleDTO();
        selectedCategorieId = 0;
        showModal = true;
    }

    public void ShowEditModal(TailleDTO taille)
    {
        isEditing = true;
        currentTaille = new TailleDTO
        {
        //    TailleId = taille.TailleId,
        //    Libelletaille = taille.Libelletaille,
        //    CategorieId = taille.CategorieId
        };
        //selectedCategorieId = taille.CategorieId;
        showModal = true;
    }

    public void ShowDeleteModal(TailleDTO taille)
    {
        currentTaille = taille;
        showDeleteModal = true;
    }

    public void CloseModal()
    {
        showModal = false;
        currentTaille = new TailleDTO();
        selectedCategorieId = 0;
        errorMessage = string.Empty;
    }

    public void CloseDeleteModal()
    {
        showDeleteModal = false;
        currentTaille = new TailleDTO();
    }

    public async Task SaveTaille()
    {
        if (string.IsNullOrWhiteSpace(currentTaille.Libelletaille))
        {
            errorMessage = "Le libellé de la taille est requis";
            return;
        }

        if (selectedCategorieId == 0)
        {
            errorMessage = "Veuillez sélectionner une catégorie";
            return;
        }

        try
        {
            //currentTaille.CategorieId = selectedCategorieId;

            if (isEditing)
            {
                await TailleService.UpdateAsync(currentTaille);
                successMessage = "Taille modifiée avec succès";
            }
            else
            {
                await TailleService.AddAsync(currentTaille);
                successMessage = "Taille ajoutée avec succès";
            }

            CloseModal();
            await VM_Taille.LoadAsync();
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

    public async Task DeleteTaille()
    {
        try
        {
            await TailleService.DeleteAsync(currentTaille.TailleId);
            successMessage = $"Taille {currentTaille.Libelletaille} supprimée avec succès";
            CloseDeleteModal();
            await VM_Taille.LoadAsync();
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

    public string GetCategoriesNumber(int idTaille)
    {
        int? categoryNumber = VM_Taille.Items?.FirstOrDefault(c => c.TailleId == idTaille).Mesures.Count();
        if (categoryNumber == null)
            return "0";

        return categoryNumber.ToString();
    }
}

