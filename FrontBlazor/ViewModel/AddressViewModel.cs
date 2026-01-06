using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Shared.DTO;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.ViewModel;

public class AddressViewModel : IDisposable
{
    private readonly IAuthService _authService;
    private readonly NavigationManager _nav;

    public ObservableCollection<AdresseDTO> Addresses { get; private set; } = new();
    public AdresseDTO? SelectedAddress { get; private set; }
    public UtilisateurDTO? CurrentUser { get; private set; }
    
    // Mode édition ou création
    public bool IsEditMode { get; private set; }
    public bool IsFormVisible { get; private set; }
    
    // Champs du formulaire
    public string Rue { get; set; } = "";
    public string Ville { get; set; } = "";
    public string CodePostal { get; set; } = "";
    public string Pays { get; set; } = "France";
    
    // États
    public bool IsLoading { get; private set; }
    public bool IsSaving { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? SuccessMessage { get; private set; }
    
    // Validation
    public Dictionary<string, string> ValidationErrors { get; private set; } = new();

    public event Action? OnChange;

    public AddressViewModel(IAuthService authService, NavigationManager nav)
    {
        _authService = authService;
        _nav = nav;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            CurrentUser = await _authService.GetCurrentUserAsync();
            if (CurrentUser == null)
            {
                _nav.NavigateTo("/login");
                return;
            }

            await LoadAddressesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AddressVM] ❌ Error loading: {ex.Message}");
            ErrorMessage = "Une erreur est survenue lors du chargement";
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    private async Task LoadAddressesAsync()
    {
        var addresses = await _authService.GetUserAddressesAsync();
        Addresses = new ObservableCollection<AdresseDTO>(addresses ?? new List<AdresseDTO>());
    }

    public void ShowCreateForm()
    {
        IsEditMode = false;
        IsFormVisible = true;
        SelectedAddress = null;
        ClearForm();
        ClearMessages();
        NotifyStateChanged();
    }

    public void ShowEditForm(AdresseDTO address)
    {
        IsEditMode = true;
        IsFormVisible = true;
        SelectedAddress = address;
        
        // Remplir le formulaire
        Rue = address.AdresseRue;
        Ville = address.AdresseVille;
        CodePostal = address.AdresseCodePostal;
        Pays = address.AdressePays;
        
        ClearMessages();
        NotifyStateChanged();
    }

    public void HideForm()
    {
        IsFormVisible = false;
        SelectedAddress = null;
        ClearForm();
        ClearMessages();
        NotifyStateChanged();
    }

    public async Task<bool> SaveAddressAsync()
    {
        if (!ValidateForm())
        {
            return false;
        }

        IsSaving = true;
        ErrorMessage = null;
        SuccessMessage = null;
        NotifyStateChanged();

        try
        {
            if (IsEditMode && SelectedAddress != null)
            {
                // Modifier l'adresse existante
                var updateDto = new UpdateAdresseDTO
                {
                    AdresseRue = Rue.Trim(),
                    AdresseVille = Ville.Trim(),
                    AdresseCodePostal = CodePostal.Trim(),
                    AdressePays = Pays.Trim()
                };

                var success = await _authService.UpdateAddressAsync(SelectedAddress.AdresseId, updateDto);
                
                if (success)
                {
                    SuccessMessage = "Adresse modifiée avec succès";
                    await LoadAddressesAsync();
                    HideForm();
                    return true;
                }
                else
                {
                    ErrorMessage = "Impossible de modifier l'adresse";
                    return false;
                }
            }
            else
            {
                // Créer une nouvelle adresse
                var createDto = new CreateAdresseDTO
                {
                    AdresseRue = Rue.Trim(),
                    AdresseVille = Ville.Trim(),
                    AdresseCodePostal = CodePostal.Trim(),
                    AdressePays = Pays.Trim(),
                    UtilisateurId = CurrentUser!.UtilisateurId
                };

                var newAddress = await _authService.AddAddressAsync(createDto);
                
                if (newAddress != null)
                {
                    SuccessMessage = "Adresse ajoutée avec succès";
                    await LoadAddressesAsync();
                    HideForm();
                    return true;
                }
                else
                {
                    ErrorMessage = "Impossible d'ajouter l'adresse";
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AddressVM] ❌ Error saving address: {ex.Message}");
            ErrorMessage = "Une erreur est survenue lors de l'enregistrement";
            return false;
        }
        finally
        {
            IsSaving = false;
            NotifyStateChanged();
        }
    }

    public async Task<bool> DeleteAddressAsync(AdresseDTO address)
    {
        if (Addresses.Count == 1)
        {
            ErrorMessage = "Vous devez avoir au moins une adresse";
            NotifyStateChanged();
            return false;
        }

        try
        {
            var success = await _authService.DeleteAddressAsync(address.AdresseId);
            
            if (success)
            {
                SuccessMessage = "Adresse supprimée avec succès";
                await LoadAddressesAsync();
                return true;
            }
            else
            {
                ErrorMessage = "Impossible de supprimer l'adresse";
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AddressVM] ❌ Error deleting address: {ex.Message}");
            ErrorMessage = "Une erreur est survenue lors de la suppression";
            return false;
        }
        finally
        {
            NotifyStateChanged();
        }
    }

    public async Task<bool> SetDefaultAddressAsync(AdresseDTO address)
    {
        try
        {
            var success = await _authService.SetDefaultAddressAsync(address.AdresseId);
            
            if (success)
            {
                SuccessMessage = "Adresse par défaut mise à jour";
                await LoadAddressesAsync();
                return true;
            }
            else
            {
                ErrorMessage = "Impossible de modifier l'adresse par défaut";
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AddressVM] ❌ Error setting default address: {ex.Message}");
            ErrorMessage = "Une erreur est survenue";
            return false;
        }
        finally
        {
            NotifyStateChanged();
        }
    }

    private bool ValidateForm()
    {
        ValidationErrors.Clear();

        if (string.IsNullOrWhiteSpace(Rue))
        {
            ValidationErrors["Rue"] = "La rue est obligatoire";
        }
        else if (Rue.Length < 5)
        {
            ValidationErrors["Rue"] = "La rue doit contenir au moins 5 caractères";
        }

        if (string.IsNullOrWhiteSpace(Ville))
        {
            ValidationErrors["Ville"] = "La ville est obligatoire";
        }
        else if (Ville.Length < 2)
        {
            ValidationErrors["Ville"] = "La ville doit contenir au moins 2 caractères";
        }

        if (string.IsNullOrWhiteSpace(CodePostal))
        {
            ValidationErrors["CodePostal"] = "Le code postal est obligatoire";
        }
        else if (!System.Text.RegularExpressions.Regex.IsMatch(CodePostal, @"^\d{5}$"))
        {
            ValidationErrors["CodePostal"] = "Le code postal doit contenir 5 chiffres";
        }

        if (string.IsNullOrWhiteSpace(Pays))
        {
            ValidationErrors["Pays"] = "Le pays est obligatoire";
        }

        if (ValidationErrors.Any())
        {
            ErrorMessage = "Veuillez corriger les erreurs du formulaire";
            NotifyStateChanged();
            return false;
        }

        return true;
    }

    private void ClearForm()
    {
        Rue = "";
        Ville = "";
        CodePostal = "";
        Pays = "France";
        ValidationErrors.Clear();
    }

    private void ClearMessages()
    {
        ErrorMessage = null;
        SuccessMessage = null;
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void Dispose()
    {
        // Cleanup si nécessaire
    }
}