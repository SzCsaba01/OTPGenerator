using System.Net;

namespace OTPGenerator.Services.Business.Exceptions;
public class InvalidPasswordException : ApiExceptionBase
{
    public InvalidPasswordException(string message) : base(HttpStatusCode.BadRequest, message, "Invalid password") {}
}
