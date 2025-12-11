using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel;

public class DetailAnnonceViewModel
{
    private readonly IAnnonceService<Annonce> _annonceService;

    public Annonce? AnnonceDetail { get; set; }
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }

    public DetailAnnonceViewModel(IAnnonceService<Annonce> annonceService, IFavorisService<Favoris> favorisService, IAuthService authService)
    {
        _annonceService = annonceService;
    }

    public async Task LoadAnnonceDetailAsync(int id)
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            AnnonceDetail = await _annonceService.GetAnnonceDetailById(id);
            if (AnnonceDetail == null)
            {
                ErrorMessage = "Annonce introuvable";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur lors du chargement de l'annonce";
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
