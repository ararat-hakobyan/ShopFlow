using Microsoft.Extensions.Logging;
using ShopFlow.Common;
using ShopFlow.DTOModels;
using ShopFlow.Models;
using ShopFlow.Enums;
using ShopFlow.Services.Interfaces;
using ShopFlow.DAL.Repositories.Interfaces;

namespace ShopFlow.Services;

public sealed class AccountService : IAccountService
{
    private const string InvalidCredentialsMessage = "Email or password is incorrect.";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ILogger<AccountService> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var alreadyExists = await _unitOfWork.Users.ExistsAsync(
            request.Username,
            request.Email,
            cancellationToken);

        if (alreadyExists)
        {
            return Result.Conflict("An account with this username or email address already exists.");
        }

        var user = new User
        {
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            Role = UserRole.Customer,
            PasswordHash = _passwordHasher.Hash(request.Password)
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("New customer account created for {Username}.", user.Username);

        return Result.Success("Your account has been created. You can sign in now.");
    }

    public async Task<Result<AuthenticatedUser>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);

        // The same message is returned for an unknown email and for a wrong password,
        // so the form cannot be used to find out which accounts exist.
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            _logger.LogWarning("Failed sign-in attempt for {Email}.", request.Email);
            return Result<AuthenticatedUser>.Failure(InvalidCredentialsMessage);
        }

        int? courierId = null;

        if (user.Role == UserRole.Courier)
        {
            var courier = await _unitOfWork.Couriers.GetByIdAsync(user.UserID, cancellationToken);
            courierId = courier?.CourierID;
        }

        var authenticated = new AuthenticatedUser
        {
            UserId = user.UserID,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            CourierId = courierId
        };

        return Result<AuthenticatedUser>.Success(authenticated);
    }
}
