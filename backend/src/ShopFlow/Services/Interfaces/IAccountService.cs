using ShopFlow.Common;
using ShopFlow.DTOModels;

namespace ShopFlow.Services.Interfaces;

public interface IAccountService
{
    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthenticatedUser>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
