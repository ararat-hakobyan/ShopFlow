using Microsoft.AspNetCore.Mvc;
using ShopFlow.Common;

namespace ShopFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected CancellationToken RequestAborted => HttpContext.RequestAborted;

    protected IActionResult FromResult(Result result)
        => result.IsSuccess
            ? Ok(ApiResponse.Ok(result.Message))
            : StatusCode(ToStatusCode(result.ErrorType), ApiResponse.Fail(result.Message));

    protected IActionResult FromResult<TValue>(Result<TValue> result)
        => result.IsSuccess && result.Value is not null
            ? Ok(result.Value)
            : StatusCode(ToStatusCode(result.ErrorType), ApiResponse.Fail(result.Message));

    protected static int ToStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status400BadRequest
    };
}
