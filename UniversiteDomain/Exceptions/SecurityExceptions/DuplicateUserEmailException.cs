namespace UniversiteDomain.Exceptions.SecurityExceptions;

public class DuplicateUserEmailException : Exception
{
    public DuplicateUserEmailException() : base() { }
    public DuplicateUserEmailException(string message) : base(message) { }
    public DuplicateUserEmailException(string message, Exception inner) : base(message, inner) { }
}