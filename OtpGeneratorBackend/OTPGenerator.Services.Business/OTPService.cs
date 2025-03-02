using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OTPGenerator.Data.Contracts;
using OTPGenerator.Data.Contracts.Helpers;
using OTPGenerator.Data.Contracts.Helpers.DTO;
using OTPGenerator.Data.Objects.Entities;
using OTPGenerator.Services.Business.Exceptions;
using OTPGenerator.Services.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace OTPGenerator.Services.Business;
public class OTPService : IOTPService
{
    private readonly IOTPRepository _OTPRepository;
    private readonly IConfiguration _configuration;
    private readonly string _encryptionKey;
    private readonly string _pepper;
    private readonly IMapper _mapper;
    private readonly IValidator<OTPDTO> _validator;
    private readonly ILogger<OTPService> _logger;

    public OTPService(
            IOTPRepository OTPRepository, 
            IConfiguration configuration, 
            IMapper mapper, 
            IValidator<OTPDTO> validator,
            ILogger<OTPService> logger
        )
    {
        _OTPRepository = OTPRepository;
        _configuration = configuration;
        _encryptionKey = _configuration["Encryption:Key"]!;
        _pepper = _configuration["Encryption:Pepper"]!;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<OTPDTO> GetGeneratedOTPAsync()
    {
        _logger.LogInformation("Starting OTP generation.");

        var otp = GenerateRandomOTP();
        var salt = GenerateSalt();

        _logger.LogInformation("Generated OTP: {OTP} and Salt: {Salt}", otp, salt);

        var saltAndPepperOTP = $"{salt}{otp}{_pepper}";

        var encryptedOTP = HashOTP(saltAndPepperOTP);

        _logger.LogInformation("Encrypted OTP.");

        var newOTPEntity = new OTPEntity
        {
            OneTimePassword = encryptedOTP,
            ExpirationDate = DateTime.UtcNow.AddSeconds(AppConstants.OTP_VALIDITY_IN_SECONDS),
            Salt = salt
        };

        await _OTPRepository.AddOTPAsync(newOTPEntity);

        _logger.LogInformation("OTP generated and stored in the database.");

        newOTPEntity.OneTimePassword = otp;

        return _mapper.Map<OTPDTO>(newOTPEntity);
    }

    public async Task ValidateOTPAsync(OTPDTO OTPDTO)
    {
        _logger.LogInformation("Validating OTP with ID: {OTPId}.", OTPDTO.Id);

        _validator.Validate(OTPDTO);

        var otpEntity = await _OTPRepository.GetOTPByIdAsync(OTPDTO.Id);

        if (otpEntity == null)
        {
            _logger.LogWarning("OTP with ID {OTPId} not found.", OTPDTO.Id);
            throw new InvalidPasswordException("The password is incorrect!");
        }

        var saltAndPepperOTP = $"{otpEntity.Salt}{OTPDTO.OneTimePassword}{_pepper}";

        var hashedOTP = HashOTP(saltAndPepperOTP);

        if (otpEntity.OneTimePassword != hashedOTP)
        {
            _logger.LogWarning("OTP validation failed for OTP ID: {OTPId}. Incorrect password.", OTPDTO.Id);
            throw new InvalidPasswordException("The password is incorrect!");
        }

        if (otpEntity.ExpirationDate < DateTime.UtcNow)
        {
            _logger.LogWarning("OTP ID: {OTPId} has expired.", OTPDTO.Id);
            throw new InvalidPasswordException("The password has expired! Please generate a new password!");
        }

        if (otpEntity.TryCount >= AppConstants.MAX_TRY_COUNT)
        {
            await _OTPRepository.DeleteOTPAsync(otpEntity);
            _logger.LogWarning("OTP validation failed for OTP ID: {OTPId}. Exceeded maximum attempt limit.", OTPDTO.Id);
            throw new InvalidPasswordException("The password is incorrect! You exceeded the attempt limit! Please generate a new password!");
        }

        await _OTPRepository.DeleteOTPAsync(otpEntity);

        _logger.LogInformation("OTP validation successful for OTP ID: {OTPId}.", OTPDTO.Id);
    }

    public async Task DeleteExpiredOTPsAsync()
    {
        _logger.LogInformation("DeleteExpiredOTPs job has started.");
        var expiredOTPs = await _OTPRepository.GetExpiredOTPsAsync();

        if (!expiredOTPs.Any())
        {
            _logger.LogInformation("No expired OTPs were found.");
            return;
        }

        await _OTPRepository.DeleteOTPsAsync(expiredOTPs);
        _logger.LogInformation("Successfully removed OTP-s");
    }

    private string GenerateRandomOTP()
    {
        var random = new Random();
        var otp = new char[AppConstants.OTP_LENGTH];

        for (int i = 0; i < AppConstants.OTP_LENGTH; i++)
        {
            otp[i] = AppConstants.VALID_CHARACTERS[random.Next(AppConstants.VALID_CHARACTERS.Length)];
        }

        return new string(otp);
    }

    private string HashOTP(string otp)
    {
        using (var hmac = SHA256.Create())
        {
            var hashedOTP = hmac.ComputeHash(Encoding.UTF8.GetBytes(otp));

            return Encoding.UTF8.GetString(hashedOTP);
        }
    }

    private string GenerateSalt()
    {
        var salt = new byte[16];
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
        {
            randomNumberGenerator.GetBytes(salt);
        }

        return Convert.ToBase64String(salt);
    }
}
