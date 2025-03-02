namespace OTPGenerator.Data.Contracts.Helpers.DTO;
public class OTPDTO
{
    public Guid Id { get; set; }
    public required string OneTimePassword { get; set; }
    public DateTime? ExpirationDate { get; set; }
}
