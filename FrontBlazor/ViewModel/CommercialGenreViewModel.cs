using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel;

public class CommercialGenresViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public Genre currentGenre = new Genre();
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

    public ListableViewModel<Genre> VM_Genre { get; set; }
    public WritableService<Genre> GenreService { get; set; }
    public event Action? OnStateChange;

    public CommercialGenresViewModel(ListableViewModel<Genre> _GenreViewModel, WritableService<Genre> genreService)
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
        currentGenre = new Genre();
        showModal = true;
    }

    public void ShowEditModal(Genre genre)
    {
        isEditing = true;
        currentGenre = new Genre
        {
            GenreId = genre.GenreId,
            NomGenre = genre.NomGenre
        };
        showModal = true;
    }

    public void ShowDeleteModal(Genre genre)
    {
        currentGenre = genre;
        showDeleteModal = true;
    }

    public void CloseModal()
    {
        showModal = false;
        currentGenre = new Genre();
        errorMessage = string.Empty;
    }

    public void CloseDeleteModal()
    {
        showDeleteModal = false;
        currentGenre = new Genre();
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
            Genre genreToSave = new Genre
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