using System.ComponentModel.DataAnnotations;

namespace OTPGenerator.Data.Objects.Entities;
public class OTPEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required string OneTimePassword { get; set; }

    public required DateTime ExpirationDate { get; set; }

    public int TryCount { get; set; }

    [Required]
    public required string Salt { get; set; }
}
