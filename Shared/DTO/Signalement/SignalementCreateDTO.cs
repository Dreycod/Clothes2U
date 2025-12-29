using System.Text.Json.Serialization;

namespace Shared.DTO.Signalement;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(SignalementAnnonceCreateDTO), "annonce")]
[JsonDerivedType(typeof(SignalementAvisCreateDTO), "avis")]
[JsonDerivedType(typeof(SignalementUtilisateurCreateDTO), "utilisateur")]
[JsonDerivedType(typeof(SignalementMessageCreateDTO), "message")]
public abstract class SignalementCreateDTO
{
    public string SignalementMotif { get; set; } = null!;
}

public class SignalementAnnonceCreateDTO : SignalementCreateDTO
{
    public int AnnonceSignaleeId { get; set; }
}

public class SignalementAvisCreateDTO : SignalementCreateDTO
{
    public int AvisId { get; set; }
}

public class SignalementUtilisateurCreateDTO : SignalementCreateDTO
{
    public int UtilisateurSignaleId { get; set; }
}
public class SignalementMessageCreateDTO : SignalementCreateDTO
{
    public int MessageId { get; set; }
}