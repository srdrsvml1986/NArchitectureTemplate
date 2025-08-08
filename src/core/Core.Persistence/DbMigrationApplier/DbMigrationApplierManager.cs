using Microsoft.EntityFrameworkCore;

namespace NArchitecture.Core.Persistence.DbMigrationApplier;

public class DbMigrationApplierService<TDbContext> : IDbMigrationApplierService<TDbContext>
    where TDbContext : DbContext
{
    private readonly TDbContext _context;

    public DbMigrationApplierService(TDbContext context)
    {
        _context = context;
    }

    public void Initialize()
    {
        _context.Database.EnsureDbApplied();
    }
}
