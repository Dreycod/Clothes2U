using Shared.DTO.Photo;

namespace Shared.DTO.Message;

public abstract class MessagePostDTO
{
    public int UtilisateurId { get; set; }
    public int ConversationId { get; set; }
}

// DTOs spécialisés
public class MessageTextePostDTO : MessagePostDTO
{
    public string Content { get; set; }
    public List<PhotoUploadDTO>? Photos { get; set; }
}

public class MessageDemandePostDTO : MessagePostDTO
{
    public int? DemandeId { get; set; }
    public double PrixPropose { get; set; }
}

public class MessageValidationPostDTO : MessagePostDTO
{
    public int MessageDemandeId { get; set; }
}