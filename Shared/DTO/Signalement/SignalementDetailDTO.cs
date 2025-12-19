using System.Text.Json.Serialization;

namespace Shared.DTO.Signalement;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(SignalementAnnonceDTO), "annonce")]
[JsonDerivedType(typeof(SignalementAvisDTO), "avis")]
[JsonDerivedType(typeof(SignalementUtilisateurDTO), "utilisateur")]
public abstract class SignalementDetailsDTO
{
    public int SignalementId { get; set; }
    public DateTime SignalementDate { get; set; }
    public string SignalementMotif { get; set; } = null!;
    public int UtilisateurSignaleId { get; set; }

}

public class SignalementAnnonceDTO : SignalementDetailsDTO
{
    public int AnnonceSignaleeId { get; set; }
}

public class SignalementAvisDTO : SignalementDetailsDTO
{
    public int AvisId { get; set; }
}

public class SignalementUtilisateurDTO : SignalementDetailsDTO { }