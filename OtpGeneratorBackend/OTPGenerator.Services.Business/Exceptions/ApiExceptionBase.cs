using System.Net;

namespace OTPGenerator.Services.Business.Exceptions;
public class ApiExceptionBase : Exception
{
    public readonly HttpStatusCode Code;
    public string Title { get; }

    public ApiExceptionBase(HttpStatusCode statusCode, string message, string title) : base(message)
    {
        Code = statusCode;
        Title = title;
    }
}
