namespace Fcg.Application.Common;

public sealed class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string message) : base(message)
    {
    }
}
