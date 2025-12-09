namespace API.DTO.Message;

public abstract class MessagePostDTO
{
    public int UtilisateurId { get; set; }
    public int ConversationId { get; set; }
}

// DTOs spécialisés
public class MessageTextePostDTO : MessagePostDTO
{
    public string Content { get; set; }
    public List<int>? ImageId { get; set; }
}

public class MessageDemandePostDTO : MessagePostDTO
{
    public int? DemandeId { get; set; }
}

public class MessageValidationPostDTO : MessagePostDTO
{
    // Propriétés spécifiques si nécessaire
}