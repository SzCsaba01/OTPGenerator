using OTPGenerator.Data.Contracts.Helpers.DTO;

namespace OTPGenerator.Services.Contracts;
public interface IOTPService
{
    public Task<OTPDTO> GetGeneratedOTPAsync();
    public Task ValidateOTPAsync(OTPDTO OTPDTO);
    public Task DeleteExpiredOTPsAsync();
}
