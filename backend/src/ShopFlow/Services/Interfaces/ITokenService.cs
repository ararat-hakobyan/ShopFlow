using ShopFlow.DTOModels;

namespace ShopFlow.Services.Interfaces;

public interface ITokenService
{
    AccessToken Create(AuthenticatedUser user);
}
