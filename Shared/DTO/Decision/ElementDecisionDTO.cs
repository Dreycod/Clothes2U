using System.Text.Json.Serialization;

namespace Shared.DTO.Decision;



[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ElementDecisionAnnonceDTO), "annonce")]
[JsonDerivedType(typeof(ElementDecisionMessageDTO), "message")]
[JsonDerivedType(typeof(ElementAvisDTO), "avis")]
[JsonDerivedType(typeof(ElementUtilisateurDTO), "utilisateur")]
public abstract class ElementDecisionDTO { }

public class ElementDecisionAnnonceDTO : ElementDecisionDTO
{
    public int AnnonceId { get; set; }
}

public class ElementDecisionMessageDTO : ElementDecisionDTO
{
    public int  MessageId { get; set; }
}

public class ElementAvisDTO : ElementDecisionDTO
{
    public int AvisId { get; set; }
}
public class ElementUtilisateurDTO : ElementDecisionDTO
{
    public int UtilisateurId { get; set; }
}