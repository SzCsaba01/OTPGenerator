using OTPGenerator.Data.Objects.Entities;

namespace OTPGenerator.Data.Contracts;
public interface IOTPRepository
{
    public Task<OTPEntity?> GetOTPByIdAsync(Guid OTPId);
    public Task<ICollection<OTPEntity>> GetExpiredOTPsAsync(); 
    public Task AddOTPAsync(OTPEntity OTPEntity);
    public Task DeleteOTPAsync(OTPEntity OTPEntity);
    public Task DeleteOTPsAsync(ICollection<OTPEntity> OTPEntities);
}
