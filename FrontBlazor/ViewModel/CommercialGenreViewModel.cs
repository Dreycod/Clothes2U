using Shared.DTO;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel;

public class CommercialGenresViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public GenreDTO currentGenre = new GenreDTO();
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

    public ListableViewModel<GenreDTO> VM_Genre { get; set; }
    public WritableService<GenreDTO> GenreService { get; set; }
    public event Action? OnStateChange;

    public CommercialGenresViewModel(ListableViewModel<GenreDTO> _GenreViewModel, WritableService<GenreDTO> genreService)
    {
        VM_Genre = _GenreViewModel;
        GenreService = genreService;
    }

    public async Task LoadAsync()
    {
        await VM_Genre.LoadWithDetailsAsync();
    }

    public void ShowAddModal()
    {
        isEditing = false;
        currentGenre = new GenreDTO();
        showModal = true;
    }

    public void ShowEditModal(GenreDTO genre)
    {
        isEditing = true;
        currentGenre = new GenreDTO
        {
            GenreId = genre.GenreId,
            NomGenre = genre.NomGenre
        };
        showModal = true;
    }

    public void ShowDeleteModal(GenreDTO genre)
    {
        currentGenre = genre;
        showDeleteModal = true;
    }

    public void CloseModal()
    {
        showModal = false;
        currentGenre = new GenreDTO();
        errorMessage = string.Empty;
    }

    public void CloseDeleteModal()
    {
        showDeleteModal = false;
        currentGenre = new GenreDTO();
    }

    public async Task SaveGenre()
    {
        if (string.IsNullOrWhiteSpace(currentGenre.NomGenre))
        {
            errorMessage = "Le nom du genre est requis";
            return;
        }

        try
        {
            GenreDTO genreToSave = new GenreDTO
            {
                GenreId = currentGenre.GenreId,
                NomGenre = currentGenre.NomGenre
            };

            if (isEditing)
            {
                await GenreService.UpdateAsync(genreToSave);
                successMessage = "Genre modifié avec succès";
            }
            else
            {
                await GenreService.AddAsync(genreToSave);
                successMessage = "Genre ajouté avec succès";
            }

            CloseModal();
            await VM_Genre.LoadAsync();
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

    public async Task DeleteGenre()
    {
        try
        {
            await GenreService.DeleteAsync(currentGenre.GenreId);
            successMessage = $"Genre {currentGenre.NomGenre} supprimé avec succès";
            CloseDeleteModal();
            await VM_Genre.LoadAsync();
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

    public int GetArticleCount(int genreId)
    {
        // TODO: Get actual article count from API
        return new Random(genreId).Next(10, 100);
    }
}