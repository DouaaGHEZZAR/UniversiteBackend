namespace UniversiteDomain.Exceptions.SecurityExceptions;

[Serializable]
public class UnauthorizedAccessException : Exception
{
    public UnauthorizedAccessException() : base("Accès non autorisé") { }
    public UnauthorizedAccessException(string message) : base(message) { }
    public UnauthorizedAccessException(string message, Exception inner) : base(message, inner) { }
}