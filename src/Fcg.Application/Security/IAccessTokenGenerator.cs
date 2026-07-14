using Fcg.Domain.Users;

namespace Fcg.Application.Security;

public interface IAccessTokenGenerator
{
    AccessToken Generate(User user);
}
