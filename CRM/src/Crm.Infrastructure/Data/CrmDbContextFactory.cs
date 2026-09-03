using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Crm.Infrastructure.Data;

public class CrmDbContextFactory : IDesignTimeDbContextFactory<CrmDbContext>
{
    public CrmDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CrmDbContext>();
        
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=crm_db;Username=ags_user;Password=ags_pass";

        optionsBuilder.UseNpgsql(connectionString, b =>
        {
            b.MigrationsAssembly(typeof(CrmDbContext).Assembly.FullName);
        });

        return new CrmDbContext(optionsBuilder.Options);
    }
}
