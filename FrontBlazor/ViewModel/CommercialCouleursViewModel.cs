using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel;
public class CommercialCouleursViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public Couleur currentCouleur = new Couleur();
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

    public ListableViewModel<Couleur> VM_Couleur { get; set; }
    public event Action? OnStateChange;

    public CommercialCouleursViewModel(ListableViewModel<Couleur> _CouleurViewModel)
    {
        VM_Couleur = _CouleurViewModel;
    }
    public async void LoadAsync()
    {
        await VM_Couleur.LoadAsync();
    }

    public void ShowAddModal()
    {
        isEditing = false;
        currentCouleur = new Couleur();
        showModal = true;
    }

    public void ShowEditModal(Couleur couleur)
    {
        isEditing = true;
        currentCouleur = new Couleur
        {
            CouleurId = couleur.CouleurId,
            Nom = couleur.Nom
        };
        showModal = true;
    }

    public void ShowDeleteModal(Couleur couleur)
    {
        currentCouleur = couleur;
        showDeleteModal = true;
    }

    public void CloseModal()
    {
        showModal = false;
        currentCouleur = new Couleur();
        errorMessage = string.Empty;
    }

    public void CloseDeleteModal()
    {
        showDeleteModal = false;
        currentCouleur = new Couleur();
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
                // TODO: Update couleur via API
                successMessage = "Couleur modifiée avec succès";
            }
            else
            {
                // TODO: Create couleur via API
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
            // TODO: Delete couleur via API
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

    public int GetArticleCount(int couleurId)
    {
        // TODO: Get actual article count from API
        return new Random(couleurId).Next(20, 150);
    }

    public string GetColorHex(string colorName)
    {
        // Simple color mapping - you might want to store hex values in the database
        var colorMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Rouge", "#ef4444" },
                { "Bleu", "#3b82f6" },
                { "Vert", "#10b981" },
                { "Jaune", "#eab308" },
                { "Orange", "#f97316" },
                { "Violet", "#a855f7" },
                { "Rose", "#ec4899" },
                { "Noir", "#1f2937" },
                { "Blanc", "#f9fafb" },
                { "Gris", "#6b7280" },
                { "Marron", "#92400e" },
                { "Beige", "#d6c9b0" }
            };

        return colorMap.TryGetValue(colorName ?? "", out var hex) ? hex : "#9ca3af";
    }
}
