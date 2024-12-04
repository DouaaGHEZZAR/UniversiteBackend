namespace UniversiteDomain.Exceptions.NoteExceptions;

public class InvalidUeForEtudiantException : Exception
{
    public InvalidUeForEtudiantException(string message) : base(message) { }
}