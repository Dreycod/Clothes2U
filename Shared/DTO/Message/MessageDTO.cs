using System.Text.Json.Serialization;
using Shared.Interfaces;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "TypeMessage")]
[JsonDerivedType(typeof(MessageTextDTO), "Texte")]
[JsonDerivedType(typeof(MessagePropositionDTO), "Proposition")]
[JsonDerivedType(typeof(MessageValidationDTO), "Validation")]
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

public class MessagePropositionDTO : MessageDTO
{
    public override string TypeMessage => "Proposition";
    // public int SenderId { get; set; }
    // public string SenderName { get; set; }
    public int? OffreParenteId { get; set; }
    public double PrixProposer { get; set; }
}

public class MessageValidationDTO : MessageDTO
{
    public override string TypeMessage => "Validation";
    public double PrixValide { get; set; }
}