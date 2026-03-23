namespace Corelink.Application.Contracts;

public class AnswerBase
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string Message { get; init; }

    protected AnswerBase(bool success, int statusCode, string message)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
    }

    public static AnswerBase Ok(string message = "Success")
        => new(true, 200, message);

    public static AnswerBase BadRequest(string message = "Bad Request")
        => new(false, 400, message);

    public static AnswerBase NotFound(string message)
        => new(false, 404, message);

    public static AnswerBase Error(string message = "Internal Server Error")
        => new(false, 500, message);
}