namespace FrontBlazor.Models
{
    public abstract class SignalementCreate
    {
        public string SignalementMotif { get; set; } = null!;
        public int TypeSignalementId { get; set; }
    }

    public class SignalementAnnonceCreate : SignalementCreate
    {
        public int AnnonceSignaleeId { get; set; }
    }

    public class SignalementAvisCreate : SignalementCreate
    {
        public int AvisId { get; set; }
    }

    public class SignalementUtilisateurCreate : SignalementCreate
    {
        public int UtilisateurSignaleId { get; set; }
    }
}