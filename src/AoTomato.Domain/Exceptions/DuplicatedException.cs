namespace AoTomato.Domain.Exceptions;

public class DuplicatedException : Exception
{
    public DuplicatedException()
    { }

    public DuplicatedException(string? message) : base(message)
    {
    }
}