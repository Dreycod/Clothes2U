using FrontBlazor.Models;
using FrontBlazor.Services;
using System.ComponentModel;
using System.Diagnostics;

namespace FrontBlazor.ViewModel;

public class ConversationViewModel
{
    private readonly ConversationService _service;

    public List<Conversation> Conversations { get; set; } = new List<Conversation>();

    public ConversationViewModel(ConversationService service)
    {
        _service = service;
    }

    public async Task<List<Conversation>> GetConversationsByUserId(int Id)
    {
        Conversations = await _service.GetConversationsByUserId(Id);
        return Conversations;
    }
    public async Task<List<Conversation>> GetConversationDetailById(int Id)
    {
        Conversations = await _service.GetConversationDetailById(Id);
        return Conversations;
    }

    public async Task AddConversation(Conversation conversation)
    {
        try
        {
            await _service.AddAsync(conversation);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erreur lors de l'ajout de la conversation : {ex.Message}");
        }
    }

    public async Task UpdateConversation(Conversation conversation)
    {
        try
        {
            await _service.UpdateAsync(conversation);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erreur lors de la mise à jour de la conversation : {ex.Message}");
        }
    }

    public async Task DeleteConversationById(int Id)
    {
        try
        {
            await _service.DeleteAsync(Id);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erreur lors de la suppression de la conversation Id:{Id} : {ex.Message}");
        }
    }
}
