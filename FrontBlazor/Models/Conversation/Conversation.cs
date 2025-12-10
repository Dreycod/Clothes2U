using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FrontBlazor.Models;
public class Conversation : IEntity, INotifyPropertyChanged
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
    public string? Interlocuteur { get; set; }
    public int? PhotoInterlocuteurId { get; set; }
    public ObservableCollection<Message>? ListMessages { get; set; } = new ObservableCollection<Message>();
    public string? TitreAnnonce { get; set; }
    public int? AnnonceId { get; set; }
    public int? PhotoAnnonceId { get; set; }
    public double? Prix { get; set; }
    private bool _hasNewMessages;
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

    // protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    // {
    //     if (EqualityComparer<T>.Default.Equals(field, value)) return false;
    //     field = value;
    //     OnPropertyChanged(propertyName);
    //     return true;
    // }
}
    