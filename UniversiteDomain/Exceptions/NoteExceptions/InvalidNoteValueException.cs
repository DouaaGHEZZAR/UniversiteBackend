namespace UniversiteDomain.Exceptions.NoteExceptions;

public class InvalidNoteValueException : Exception
{
    public InvalidNoteValueException(string message) : base(message) { }
}