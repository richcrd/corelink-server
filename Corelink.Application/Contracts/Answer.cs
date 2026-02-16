namespace Corelink.Application.Contracts;

public class Answer<T>
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string Message { get; init; }
    public T? Response { get; init; }

    private Answer(bool success, int statusCode, string message, T? response)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
        Response = response;
    }

    public static Answer<T> Ok(T response, string message = "Success")
    {
        return new Answer<T>(true, 200, message, response);
    }
    
    public static Answer<T> BadRequest(string message = "Bad Request")
    {
        return new Answer<T>(false, 400, message, default);
    }
    
    public static Answer<T> NotFound(string message)
    {
        return new Answer<T>(false, 404, message, default);
    }
    
    public static Answer<T> Error(string message = "Internal Server Error")
    {
        return new Answer<T>(false, 500, message, default);
    }
}