using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using OTPGenerator.Data.Contracts;
using OTPGenerator.Data.Contracts.Helpers;
using OTPGenerator.Data.Contracts.Helpers.DTO;
using OTPGenerator.Data.Objects.Entities;
using OTPGenerator.Services.Business;
using OTPGenerator.Services.Business.Exceptions;
using OTPGenerator.Services.Contracts;
using System.Reflection;

namespace OTPGenerator.Tests;
public class ServiceTests
{
    private readonly Mock<IOTPRepository> _mockRepository;
    private readonly Mock<IOTPService> _mockOTPService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<OTPDTO>> _mockValidator;
    private readonly Mock<ILogger<OTPService>> _mockLogger;

    private readonly OTPService _otpService;

    public ServiceTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockValidator = new Mock<IValidator<OTPDTO>>();
        _mockLogger = new Mock<ILogger<OTPService>>();
        _mockRepository = new Mock<IOTPRepository>();
        _mockOTPService = new Mock<IOTPService>();

        _otpService = new OTPService(
        _mockRepository.Object,
                new ConfigurationBuilder().Build(),  
                _mockMapper.Object,
                _mockValidator.Object,
                _mockLogger.Object
            );
    }

    [Fact]
    public async Task GetGeneratedOTPAsync_ShouldReturnOTPDTO()
    {
        // Arrange
        var otpEntity = CreateOtpEntity();

        _mockRepository.Setup(r => r.AddOTPAsync(It.IsAny<OTPEntity>())).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<OTPDTO>(It.IsAny<OTPEntity>())).Returns(new OTPDTO { Id = Guid.NewGuid(), OneTimePassword = "123456" });

        // Act
        var result = await _otpService.GetGeneratedOTPAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.OneTimePassword.Length); 
    }

    [Fact]
    public async Task ValidateOTPAsync_ShouldThrowException_WhenOTPNotFound()
    {
        // Arrange
        var otpDto = CreateOtpDTO();

        _mockValidator.Setup(v => v.Validate(It.IsAny<OTPDTO>()));
        _mockRepository.Setup(r => r.GetOTPByIdAsync(It.IsAny<Guid>())).ReturnsAsync((OTPEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPasswordException>(() => _otpService.ValidateOTPAsync(otpDto));
    }

    [Fact]
    public async Task ValidateOTPAsync_ShouldValidateSuccessfully_WhenOTPIsCorrect()
    {
        // Arrange
        var otpDto = CreateOtpDTO();
        var otpEntity = CreateOtpEntity(id: otpDto.Id);

        var otpService = new OTPService(
            _mockRepository.Object,
            new ConfigurationBuilder().Build(),
            _mockMapper.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );

        var hashOTPMethod = typeof(OTPService).GetMethod("HashOTP", BindingFlags.NonPublic | BindingFlags.Instance);

        var saltAndPepperOTP = $"{otpEntity.Salt}{otpEntity.OneTimePassword}";
        var hashedOtp = (string)hashOTPMethod.Invoke(otpService, new object[] { saltAndPepperOTP });

        otpEntity.OneTimePassword = hashedOtp;

        _mockValidator.Setup(v => v.Validate(It.IsAny<OTPDTO>()));
        _mockRepository.Setup(r => r.GetOTPByIdAsync(It.IsAny<Guid>())).ReturnsAsync(otpEntity);
        _mockRepository.Setup(r => r.DeleteOTPAsync(It.IsAny<OTPEntity>())).Returns(Task.CompletedTask);

        // Act
        await _otpService.ValidateOTPAsync(otpDto);

        // Assert
        _mockRepository.Verify(r => r.DeleteOTPAsync(It.IsAny<OTPEntity>()), Times.Once);
    }

    [Fact]
    public async Task DeleteExpiredOTPsAsync_ShouldDeleteExpiredOTPs()
    {
        // Arrange
        var expiredOTPs = new List<OTPEntity>
        {
            CreateOtpEntity(expirationDate: DateTime.UtcNow.AddSeconds(-1))
        };

        _mockRepository.Setup(r => r.GetExpiredOTPsAsync()).ReturnsAsync(expiredOTPs);
        _mockRepository.Setup(r => r.DeleteOTPsAsync(It.IsAny<ICollection<OTPEntity>>())).Returns(Task.CompletedTask);

        // Act
        await _otpService.DeleteExpiredOTPsAsync();

        // Assert
        _mockRepository.Verify(r => r.DeleteOTPsAsync(It.IsAny<ICollection<OTPEntity>>()), Times.Once);
    }

    [Fact]
    public void GenerateRandomOTP_ShouldReturnOTPOfCorrectLength()
    {
        // Arrange
        var methodInfo = typeof(OTPService).GetMethod("GenerateRandomOTP", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = (string)methodInfo.Invoke(_otpService, null);

        // Assert
        Assert.Equal(6, result.Length); 
    }

    [Fact]
    public void HashOTP_ShouldReturnHashedString()
    {
        // Arrange
        var otp = "123456";
        var methodInfo = typeof(OTPService).GetMethod("HashOTP", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = (string)methodInfo.Invoke(_otpService, new object[] { otp });

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(otp, result); 
    }

    [Fact]
    public void GenerateSalt_ShouldReturnValidSalt()
    {
        // Arrange
        var methodInfo = typeof(OTPService).GetMethod("GenerateSalt", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var result = (string)methodInfo.Invoke(_otpService, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(24, result.Length); 
    }

    private OTPEntity CreateOtpEntity(Guid? id = null, string otp = "ASD456", string salt = "salt", int tryCount = 0, DateTime? expirationDate = null)
    {
        return new OTPEntity
        {
            Id = id ?? Guid.NewGuid(),
            OneTimePassword = otp,
            ExpirationDate = expirationDate ?? DateTime.UtcNow.AddSeconds(AppConstants.OTP_VALIDITY_IN_SECONDS),
            Salt = salt,
            TryCount = tryCount
        };
    }

    private OTPDTO CreateOtpDTO(Guid? id = null, string otp = "ASD456")
    {
        return new OTPDTO
        {
            Id = id ?? Guid.NewGuid(),
            OneTimePassword = otp
        };
    }

}
