using Shared.Interfaces;
using Shared.DTO.Utilisateur;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Shared.DTO.Message;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "TypeMessage")]
[JsonDerivedType(typeof(MessageTextDTO), "Texte")]
[JsonDerivedType(typeof(MessagePropositionDTO), "Proposition")]
[JsonDerivedType(typeof(MessageValidationDTO), "Validation")]
public abstract class MessageDTO : IEntity
{
    public int MessageId { get; set; }
    public int? SenderId { get; set; }
    public string? SenderName { get; set; }
    public bool? SentByCurrentUser { get; set; }
    public int? UtilisateurId { get; set; }
    public DateTime? Date { get; set; }
    public ObservableCollection<int> ImagesId { get; set; } = new();
    public bool? Lu { get; set; }
    public bool? IsRead { get; set; }
    public string? Content { get; set; }
    public UtilisateurDTO? Utilisateur { get; set; }
    public abstract string TypeMessage { get; }
    public int ConversationId { get; set; }

    public int GetId() => MessageId;
}

public class MessageTextDTO : MessageDTO
{
    public override string TypeMessage => "Texte";
}

public class MessagePropositionDTO : MessageDTO
{
    public override string TypeMessage => "Proposition";
    public int? OffreParenteId { get; set; }
    public double PrixProposer { get; set; }
}

public class MessageValidationDTO : MessageDTO
{
    public override string TypeMessage => "Validation";
    public double PrixValide { get; set; }
}