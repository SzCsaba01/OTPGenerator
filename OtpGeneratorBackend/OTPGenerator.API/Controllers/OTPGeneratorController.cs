using Microsoft.AspNetCore.Mvc;
using OTPGenerator.Data.Contracts.Helpers.DTO;
using OTPGenerator.Services.Contracts;

namespace OTPGenerator.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OTPGeneratorController : ControllerBase
{
    private readonly IOTPService _otpService;

    public OTPGeneratorController(IOTPService otpService)
    {
        _otpService = otpService;
    }

    [HttpGet("GenerateOTP")]
    public async Task<IActionResult> GenerateOTP()
    {
        var otp = await _otpService.GetGeneratedOTPAsync();

        return Ok(otp);
    }

    [HttpPut("ValidateOTP")]
    public async Task<IActionResult> ValidateOTP([FromBody] OTPDTO otpDTO)
    {
        await _otpService.ValidateOTPAsync(otpDTO);

        return Ok(new { message = "Successfully validated the one time password!"});
    }
}
