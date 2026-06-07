
namespace AoTomato.Domain.Exceptions;

public class InvalidPayloadException : Exception
{
    public InvalidPayloadException()
    {
    }

    public InvalidPayloadException(string? message) : base(message)
    {
    }
    
    public InvalidPayloadException(List<string> invalidFields) : base(string.Join(";", invalidFields))
    {
    }
}