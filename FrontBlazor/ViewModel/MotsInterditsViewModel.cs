using FrontBlazor.Models.Moderation;
using FrontBlazor.Services.Interfaces;

namespace FrontBlazor.ViewModel;

public class MotsInterditsViewModel
{
    public bool IsLoading { get; set; }
    public List<MotInterdit> Mots { get; set; }
    private readonly IMotsInterditsService  _motsInterditsService;
    public MotInterdit motToAdd { get; set; }

    public MotsInterditsViewModel(IMotsInterditsService motsInterditsService)
    {
        _motsInterditsService = motsInterditsService;
    }


    public async Task LoadAsync()
    {
        IsLoading = true;
        motToAdd = new MotInterdit();
        Mots = await _motsInterditsService.GetAllAsync();
        IsLoading = false;
    }

    public async Task DeleteMotAsync(int id)
    {
        try
        {
            await _motsInterditsService.DeleteAsync(id);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la suppression: {ex.Message}");
        }
    }

    public async Task AddMotInterditAsync()
    {
        try
        {
            await _motsInterditsService.AddAsync(motToAdd);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'ajout d'un message: {ex.Message}");
        }
    }
}