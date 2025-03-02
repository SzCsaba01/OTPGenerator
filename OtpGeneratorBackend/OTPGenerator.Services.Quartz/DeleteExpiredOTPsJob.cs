using OTPGenerator.Services.Contracts;
using Quartz;

namespace OTPGenerator.Services.Quartz;
public class DeleteExpiredOTPsJob : IJob
{
    private readonly IOTPService _otpService;

    public DeleteExpiredOTPsJob(IOTPService otpService)
    {
        _otpService = otpService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _otpService.DeleteExpiredOTPsAsync();
    }
}
