using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopFlow.Common;
using ShopFlow.DTOModels;
using ShopFlow.Services.Interfaces;

namespace ShopFlow.Controllers;

public sealed class AccountController : ApiControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ITokenService _tokenService;

    public AccountController(IAccountService accountService, ITokenService tokenService)
    {
        _accountService = accountService;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _accountService.RegisterAsync(request, RequestAborted);

        return FromResult(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _accountService.LoginAsync(request, RequestAborted);

        if (result.IsFailure || result.Value is null)
        {
            return Unauthorized(ApiResponse.Fail(result.Message));
        }

        var token = _tokenService.Create(result.Value);

        return Ok(new LoginResponse
        {
            Token = token.Token,
            ExpiresAtUtc = token.ExpiresAtUtc,
            User = result.Value
        });
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public IActionResult Logout() => Ok(ApiResponse.Ok("You have been signed out."));

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me([FromServices] ICurrentUser currentUser)
        => Ok(new AuthenticatedUser
        {
            UserId = currentUser.UserId,
            Username = currentUser.Username,
            Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty,
            Role = currentUser.Role
        });
}
