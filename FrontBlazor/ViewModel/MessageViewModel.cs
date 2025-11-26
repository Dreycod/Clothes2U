using FrontBlazor.Components.Pages;
using FrontBlazor.Models;
using FrontBlazor.Services;
using System.ComponentModel;
using System.Diagnostics;

namespace FrontBlazor.ViewModel;

public class MessageViewModel
{
    private readonly MessageService _service;

    public List<Message> Messages { get; set; } = new List<Message>();

    public MessageViewModel(MessageService service)
    {
        _service = service;
    }

    public async Task<List<Message>> GetMessagesByMessageId(int Id)
    {
        Messages = await _service.GetMessagesByConversationId(Id);
        return Messages;
    }
    public async Task<List<Message>> GetMessagesByUserId(int Id)
    {
        Messages = await _service.GetMessagesByUserId(Id);
        return Messages;
    }

    public async Task AddMessage( Message message)
    {
        try
        {
            await _service.AddAsync(message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erreur lors de l'ajout de la Message : {ex.Message}");
        }
    }

    public async Task UpdateMessage(Message message)
    {
        try
        {
            await _service.UpdateAsync(message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erreur lors de la mise à jour de la Message : {ex.Message}");
        }
    }

    public async Task DeleteMessageById(int Id)
    {
        try
        {
            await _service.DeleteAsync(Id);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erreur lors de la suppression de la Message Id:{Id} : {ex.Message}");
        }
    }
}
