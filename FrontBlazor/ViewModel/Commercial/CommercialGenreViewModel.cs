using FrontBlazor.Services;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Commercial;
using Microsoft.AspNetCore.Components;
using Shared.DTO;

namespace FrontBlazor.ViewModel;

public class CommercialGenresViewModel: BaseCommercialViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public GenreDTO currentGenre = new GenreDTO();
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

   
    ICaracteristiqueService<GenreDTO> _genreService;
    public List<GenreDTO> Genres { get; set; }
    public event Action? OnStateChange;

    public CommercialGenresViewModel(ICaracteristiqueService<GenreDTO> genreService, IAuthService authService, NavigationManager nav) : base(authService, nav)
    {
        _genreService = genreService;
    }

    public async Task LoadAsync()
    {
        Genres = await _genreService.GetAllAsync();
        if (Genres != null)
        {
            Genres = Genres
                .OrderBy(c => c.GenreId)
                .ToList();
        }
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
                await _genreService.UpdateAsync(genreToSave);
                successMessage = "Genre modifié avec succès";
            }
            else
            {
                await _genreService.AddAsync(genreToSave);
                successMessage = "Genre ajouté avec succès";
            }

            CloseModal();
            Genres = await _genreService.GetAllAsync();
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
            await _genreService.DeleteAsync(currentGenre.GenreId);
            successMessage = $"Genre {currentGenre.NomGenre} supprimé avec succès";
            CloseDeleteModal();
            Genres = await _genreService.GetAllAsync();
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