using Microsoft.EntityFrameworkCore;
using OTPGenerator.Data.Access;
using OTPGenerator.Data.Access.Data;
using OTPGenerator.Data.Objects.Entities;

namespace OTPGenerator.Tests;

public class RepositoryTests
{
    private readonly DbContextOptions<OTPGeneratorContext> _contextOptions;

    public RepositoryTests()
    {
        _contextOptions = new DbContextOptionsBuilder<OTPGeneratorContext>()
            .UseInMemoryDatabase(databaseName: "OTPGenerator")
            .Options;
    }

    [Fact]
    public async Task GetOTPByIdAsync_ShouldReturnCorrectOTP()
    {
        //Arrange
        var repository = GetRepository();
        var otpEntity = CreateValidOTP();

        
        await repository.AddOTPAsync(otpEntity);

        //Act
        var result = await repository.GetOTPByIdAsync(otpEntity.Id);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(otpEntity.OneTimePassword, result?.OneTimePassword);
        Assert.Equal(otpEntity.Salt, result?.Salt);
    }

    [Fact]
    public async Task GetExpiredOTPsAsync_ShouldReturnExpiredOTPs()
    {
        //Arrange
        var repository = GetRepository();
        var expiredOTP = CreateExpiredOTP();

        var validOTP = CreateValidOTP();

        await repository.AddOTPAsync(expiredOTP);
        await repository.AddOTPAsync(validOTP);

        //Act
        var expiredOTPs = await repository.GetExpiredOTPsAsync();

        Assert.Single(expiredOTPs);
        Assert.Contains(expiredOTPs, otp => otp.Id == expiredOTP.Id);
        Assert.DoesNotContain(expiredOTPs, otp => otp.Id == validOTP.Id);
    }

    [Fact]
    public async Task AddOTPAsync_ShouldAddOTP()
    {
        //Arrange
        var repository = GetRepository();
        var otpEntity = CreateValidOTP();

        await repository.AddOTPAsync(otpEntity);

        //Act
        var result = await repository.GetOTPByIdAsync(otpEntity.Id);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(otpEntity.OneTimePassword, result?.OneTimePassword);
        Assert.Equal(otpEntity.Salt, result?.Salt);
        Assert.Equal(otpEntity.ExpirationDate, result?.ExpirationDate);
    }

    [Fact]
    public async Task DeleteOTPAsync_ShouldDeleteOTP()
    {
        //Arrange
        var repository = GetRepository();
        var otpEntity = CreateValidOTP();
        await repository.AddOTPAsync(otpEntity);

        //Act
        await repository.DeleteOTPAsync(otpEntity);

        //Assert
        var result = await repository.GetOTPByIdAsync(otpEntity.Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteOTPsAsync_ShouldDeleteMultipleOTPs()
    {
        //Arrange
        var repository = GetRepository();
        var otpEntities = new List<OTPEntity>
            {
               CreateExpiredOTP(),
               CreateExpiredOTP()
            };

        foreach (var otpEntity in otpEntities)
        {
            await repository.AddOTPAsync(otpEntity);
        }

        //Act
        await repository.DeleteOTPsAsync(otpEntities);

        //Assert
        var result = await repository.GetOTPByIdAsync(otpEntities[0].Id);
        var result2 = await repository.GetOTPByIdAsync(otpEntities[1].Id);
        Assert.Null(result);
        Assert.Null(result2);
    }

    private OTPRepository GetRepository()
    {
        var context = new OTPGeneratorContext(_contextOptions);
        return new OTPRepository(context);
    }

    private OTPEntity CreateValidOTP()
    {
        return new OTPEntity
        {
            Id = Guid.NewGuid(),
            OneTimePassword = "ASD456",
            ExpirationDate = DateTime.UtcNow.AddMinutes(5),
            Salt = "randomSalt",
            TryCount = 0
        };
    }

    private OTPEntity CreateExpiredOTP()
    {
        return new OTPEntity
        {
            Id = Guid.NewGuid(),
            OneTimePassword = "ASD456",
            ExpirationDate = DateTime.UtcNow.AddMinutes(-1), // Expired OTP
            Salt = "expiredSalt",
            TryCount = 0
        };
    }
}