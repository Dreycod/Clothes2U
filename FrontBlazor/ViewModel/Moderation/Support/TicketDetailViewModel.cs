using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.Services.WebService;
using Microsoft.AspNetCore.Components;
using Shared.DTO.SupportTicket;

namespace FrontBlazor.ViewModel.Moderation.Support;

public class TicketDetailViewModel : ModerationViewModel
{
    private readonly ISupportService _service;
    private readonly NavigationManager _nav;
    
    public TicketDetailViewDTO TicketDetail { get; set; }
    public string ResponseMessage { get; set; }
    public int Id { get; set; }
    public string ErrorMessage { get; set; }
    
    public TicketDetailViewModel(
        ISupportService service,
        IAuthService authService,
        NavigationManager nav)
        : base(authService, nav)
    {
        _nav = nav;
        _service = service;
    }
    
    public override async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        
        try
        {
            await base.LoadAsync();
            TicketDetail = await _service.GetTicketById(Id);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors du chargement du ticket: {ex.Message}";
            Console.WriteLine($"Erreur: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    public void NavigateToSupport()
    {
        _nav.NavigateTo("/moderation/support");
    }
    
    public async Task SendResponse()
    {
        try
        {
            IsLoading = true;
            SupportTicketReplyDTO reply = new SupportTicketReplyDTO()
            {
                Message = ResponseMessage,
                TicketId = Id
            };
            await _service.ReplyAsync(reply);
            await LoadAsync();
            Console.WriteLine("Envoie de la réponse : " + reply.Message);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors de l'envoi de la réponse: {ex.Message}";
        }
        finally
        {
            NavigateToSupport();
        }
    }
    
    public async Task CloseTicket()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null; 
        
            await _service.CloseTicket(Id);
            NavigateToSupport();
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = ex.Message;
            Console.WriteLine($"Erreur HTTP: {ex}");
            IsLoading = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors de la clôture du ticket: {ex.Message}";
            Console.WriteLine($"Erreur: {ex}");
            IsLoading = false;
        }
    }
}

