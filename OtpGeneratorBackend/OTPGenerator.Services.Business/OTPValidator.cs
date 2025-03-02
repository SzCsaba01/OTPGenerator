using Microsoft.Extensions.Logging;
using OTPGenerator.Data.Contracts.Helpers.DTO;
using OTPGenerator.Services.Business.Exceptions;
using OTPGenerator.Services.Contracts;
using System.Text.RegularExpressions;

namespace OTPGenerator.Services.Business;
public class OTPValidator : IValidator<OTPDTO>
{
    private readonly ILogger<OTPValidator> _logger;

    public OTPValidator(ILogger<OTPValidator> logger)
    {
        _logger = logger;
    }

    public void Validate(OTPDTO entity)
    {
        if (entity == null)
        {
            _logger.LogWarning("Validation failed: OTPDTO is null.");
            throw new InvalidPasswordException("The password is incorrect!");
        }

        if (entity.Id == Guid.Empty)
        {
            _logger.LogWarning("Validation failed: OTPDTO Id is empty.");
            throw new InvalidPasswordException("The password is incorrect!");
        }

        if (entity.OneTimePassword.Length != 6)
        {
            _logger.LogWarning("Validation failed: OTP length is incorrect.");
            throw new InvalidPasswordException("The password must be exactly 6 characters!");
        }

        if (!Regex.IsMatch(entity.OneTimePassword, "^[A-Z0-9]+$"))
        {
            _logger.LogWarning("Validation failed: OTP contains invalid characters.");
            throw new InvalidPasswordException("The password is incorrect!");
        }
    }
}
