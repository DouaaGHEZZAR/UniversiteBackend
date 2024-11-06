namespace UniversiteDomain.Exceptions.ParcoursException;

[Serializable]
public class DuplicateIdException : Exception
{
    public DuplicateIdException() : base() { }
    public DuplicateIdException(string message) : base(message) { }
    public DuplicateIdException(string message, Exception inner) : base(message, inner) { }
}