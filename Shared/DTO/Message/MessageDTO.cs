using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Shared.Interfaces;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "TypeMessage")]
[JsonDerivedType(typeof(MessageTextDTO), "Texte")]
[JsonDerivedType(typeof(MessageDemandeDTO), "Demande")]
[JsonDerivedType(typeof(MessageEstPayeeDTO), "Payee")]
public abstract class MessageDTO : IEntity
{
    public int? MessageId { get; set; }
    public DateTime? Date { get; set; }
    public bool? Lu { get; set; }
    public abstract string TypeMessage { get; }
    public bool SentByCurrentUser { get; set; }
    public int SenderId { get; set; }
    public string? SenderName { get; set; }
    public int ConversationId { get; set; }
    
    public int GetId()
    {
        return MessageId ?? 0;
    }
}

public class MessageTextDTO : MessageDTO
{
    public override string TypeMessage => "Texte";
    //public string SenderName { get; set; }
    public string Content { get; set; }
    public List<int> Photos { get; set; }
}

public class MessageDemandeDTO : MessageDTO
{
    public override string TypeMessage => "Demande";
    // public int SenderId { get; set; }
    // public string SenderName { get; set; }
    public int? DemandeId { get; set; }
    public decimal PrixPropose { get; set; }
    public bool EstAcceptee { get; set; } = false;
    public bool EstRepondue { get; set; } = false;
}

public class MessageEstPayeeDTO : MessageDTO
{
    public override string TypeMessage => "Payee";
    public int MessageEstPayeeId { get; set; }
    public bool EstEnvoye { get; set; }
    public bool EstAnnule { get; set; }
}