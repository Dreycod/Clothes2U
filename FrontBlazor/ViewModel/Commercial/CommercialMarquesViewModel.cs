using Shared.DTO;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using Shared.DTO.Marque;

namespace FrontBlazor.ViewModel;
public class CommercialMarquesViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public MarqueDTO currentMarque = new MarqueDTO();
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

    public ListableViewModel<MarqueDTO> VM_Marque { get; set; }
    public WritableService<MarqueDTO> MarqueService { get; set; }  
    public event Action? OnStateChange;


    public CommercialMarquesViewModel(ListableViewModel<MarqueDTO> _MarqueViewModel, WritableService<MarqueDTO> marqueService)
    {
        VM_Marque = _MarqueViewModel;
        MarqueService = marqueService;
    }

    public async Task LoadAsync()
    {
        await VM_Marque.LoadAsync();
    }

    public void ShowAddModal()
    {
        isEditing = false;
        currentMarque = new MarqueDTO();
        showModal = true;
    }

    public void ShowEditModal(MarqueDTO marque)
    {
        isEditing = true;
        currentMarque = new MarqueDTO
        {
            MarqueID = marque.MarqueID,
            NomMarque = marque.NomMarque
        };
        showModal = true;
    }

    public void ShowDeleteModal(MarqueDTO marque)
    {
        currentMarque = marque;
        showDeleteModal = true;
    }

    public void CloseModal()
    {
        showModal = false;
        currentMarque = new MarqueDTO();
        errorMessage = string.Empty;
    }

    public void CloseDeleteModal()
    {
        showDeleteModal = false;
        currentMarque = new MarqueDTO();
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
            MarqueDTO marqueToSave = new MarqueDTO
            {
                MarqueID = currentMarque.MarqueID,
                NomMarque = currentMarque.NomMarque
            };

            if (isEditing)
            {

                await MarqueService.UpdateAsync(marqueToSave);
                successMessage = "Marque modifiée avec succès";
            }
            else
            {
                await MarqueService.AddAsync(marqueToSave);
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
            await MarqueService.DeleteAsync(currentMarque.MarqueID);
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