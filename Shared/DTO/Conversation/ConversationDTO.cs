using Shared.Interfaces;
using Shared.DTO.Message;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shared.DTO.Conversation;

public class ConversationDTO : IEntity, INotifyPropertyChanged
{
    public int ConversationId { get; set; }
    private string? _lastMessage;
    public string? LastMessage
    {
        get => _lastMessage;
        set
        {
            if (_lastMessage != value)
            {
                _lastMessage = value;
                OnPropertyChanged();
            }
        }
    }
    public DateTime? LastMessageDate { get; set; }
    public int StatusConversationId { get; set; }
    public string? StatusConversation { get; set; }
    public int VendeurId { get; set; }
    public string? Interlocuteur { get; set; }
    public int? PhotoInterlocuteurId { get; set; }
    public ObservableCollection<MessageDTO>? ListMessages { get; set; } = new ObservableCollection<MessageDTO>();
    public string? TitreAnnonce { get; set; }
    public int? AnnonceId { get; set; }
    public int? PhotoAnnonceId { get; set; }
    public decimal Prix { get; set; }
    public decimal PrixAnnonce { get; set; }
    private bool _hasNewMessages;
    public bool Negociable { get; set; }
    public bool HasNewMessages
    {
        get => _hasNewMessages;
        set
        {
            if (_hasNewMessages != value)
            {
                Console.WriteLine($"[Conversation] HasNewMessages changing: {_hasNewMessages} → {value}");
                _hasNewMessages = value;
                OnPropertyChanged();
            }
        }
    }
    public int GetId()
    {
        return ConversationId;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}