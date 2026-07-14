namespace Fcg.Application.Security;

public sealed class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException(string message) : base(message)
    {
    }
}
