namespace UniversiteDomain.Exceptions.ParcoursException;

[Serializable]
public class DuplicateAnneeFormationException : Exception
{
    public DuplicateAnneeFormationException() : base() { }
    public DuplicateAnneeFormationException(string message) : base(message) { }
    public DuplicateAnneeFormationException(string message, Exception inner) : base(message, inner) { }
}