namespace Corelink.Application.Contracts;

public class Answer<T> : AnswerBase
{
    public T? Response { get; init; }

    private Answer(bool success, int statusCode, string message, T? response) : base(success, statusCode, message)
    {
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