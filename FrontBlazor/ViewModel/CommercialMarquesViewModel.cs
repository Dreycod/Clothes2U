using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel;
public class CommercialMarquesViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public Marque currentMarque = new Marque();
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

    public ListableViewModel<Marque> VM_Marque { get; set; }
    public event Action? OnStateChange;

    public CommercialMarquesViewModel(ListableViewModel<Marque> _MarqueViewModel)
    {
        VM_Marque = _MarqueViewModel;
    }

    public async Task LoadAsync()
    {
        await VM_Marque.LoadAsync();
    }

    public void ShowAddModal()
    {
        isEditing = false;
        currentMarque = new Marque();
        showModal = true;
    }

    public void ShowEditModal(Marque marque)
    {
        isEditing = true;
        currentMarque = new Marque
        {
            MarqueId = marque.MarqueId,
            NomMarque = marque.NomMarque
        };
        showModal = true;
    }

    public void ShowDeleteModal(Marque marque)
    {
        currentMarque = marque;
        showDeleteModal = true;
    }

    public void CloseModal()
    {
        showModal = false;
        currentMarque = new Marque();
        errorMessage = string.Empty;
    }

    public void CloseDeleteModal()
    {
        showDeleteModal = false;
        currentMarque = new Marque();
    }

    public async Task SaveMarque()
    {
        if (string.IsNullOrWhiteSpace(currentMarque.NomMarque))
        {
            errorMessage = "Le nom de la marque est requis";
            return;
        }

        try
        {
            if (isEditing)
            {
                // TODO: Update marque via API
                successMessage = "Marque modifiée avec succès";
            }
            else
            {
                // TODO: Create marque via API
                successMessage = "Marque ajoutée avec succès";
            }

            CloseModal();
            await VM_Marque.LoadAsync();
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

    public async Task DeleteMarque()
    {
        try
        {
            // TODO: Delete marque via API
            successMessage = $"Marque {currentMarque.NomMarque} supprimée avec succès";
            CloseDeleteModal();
            await VM_Marque.LoadAsync();
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

    public int GetArticleCount(int marqueId)
    {
        // TODO: Get actual article count from API
        return new Random(marqueId).Next(10, 100);
    }
}