namespace FrontBlazor.Exceptions
{
    /// <summary>
    /// Exception levée quand un mot interdit est détecté
    /// </summary>
    public class MotInterditException : Exception
    {
        public MotInterditException()
            : base("Le contenu contient un mot interdit")
        {
        }

        public MotInterditException(string message)
            : base(message)
        {
        }

        public MotInterditException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception levée pour une erreur BadRequest générique
    /// </summary>
    public class BadRequestException : Exception
    {
        public int StatusCode { get; }

        public BadRequestException(string message, int statusCode = 400)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}