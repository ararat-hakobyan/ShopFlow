namespace ShopFlow.Common;

public sealed class ApiResponse
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public IDictionary<string, string[]>? Errors { get; init; }

    public static ApiResponse Ok(string message = "") => new() { Success = true, Message = message };

    public static ApiResponse Fail(string message, IDictionary<string, string[]>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}
