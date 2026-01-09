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
    public decimal PrixPropose { get; set; }
}

public class MessageEstPayeePostDTO : MessagePostDTO
{
}

public class MessageEnvoisColisPostDTO : MessagePostDTO
{
    public PhotoUploadDTO Photo { get; set; }
    public int MessageEstPayeeId { get; set; }
}