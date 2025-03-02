using Microsoft.EntityFrameworkCore;
using OTPGenerator.Data.Access.Data;
using OTPGenerator.Data.Contracts;
using OTPGenerator.Data.Objects.Entities;

namespace OTPGenerator.Data.Access;
public class OTPRepository : IOTPRepository
{
    private readonly OTPGeneratorContext _context;

    public OTPRepository(OTPGeneratorContext context)
    {
        _context = context;
    }

    public async Task<OTPEntity?> GetOTPByIdAsync(Guid OTPId)
    {
        return await _context.OTPs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == OTPId);
    }

    public async Task<ICollection<OTPEntity>> GetExpiredOTPsAsync()
    {
        return await _context.OTPs
            .Where(x => x.ExpirationDate < DateTime.UtcNow)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task AddOTPAsync(OTPEntity OTPEntity)
    {
        await _context.OTPs.AddAsync(OTPEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOTPAsync(OTPEntity OTPEntity)
    {
       _context.OTPs.Remove(OTPEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOTPsAsync(ICollection<OTPEntity> OTPEntities)
    {
        _context.OTPs.RemoveRange(OTPEntities);
        await _context.SaveChangesAsync();
    }
}
