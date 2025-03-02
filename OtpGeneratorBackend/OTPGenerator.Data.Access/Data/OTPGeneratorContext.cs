using Microsoft.EntityFrameworkCore;
using OTPGenerator.Data.Objects.Entities;

namespace OTPGenerator.Data.Access.Data;
public class OTPGeneratorContext : DbContext
{
    public OTPGeneratorContext(DbContextOptions<OTPGeneratorContext> options) : base(options) {}

    public DbSet<OTPEntity> OTPs { get; set; }
}
