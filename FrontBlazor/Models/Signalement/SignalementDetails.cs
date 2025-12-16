using System.Text.Json.Serialization;

namespace FrontBlazor.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(SignalementAnnonce), "annonce")]
[JsonDerivedType(typeof(SignalementAvis), "avis")]
[JsonDerivedType(typeof(SignalementUtilisateur), "utilisateur")]
public abstract class SignalementDetails
{
    public int SignalementId { get; set; }
    public DateTime SignalementDate { get; set; }
    public string SignalementMotif { get; set; } = null!;
    public int UtilisateurSignaleId { get; set; }
}

public  class SignalementAnnonce :  SignalementDetails
{
    public int AnnonceSignaleeId { get; set; }
}

public class SignalementAvis : SignalementDetails
{
    public int AvisId { get; set; }
}

public class SignalementUtilisateur : SignalementDetails { }
