using Crm.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Crm.Infrastructure.Data.Interceptors;

public class AuditoriaSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AtualizarAuditoria(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AtualizarAuditoria(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void AtualizarAuditoria(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<IAuditableEntity>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CriadoEm = now;
                if (string.IsNullOrWhiteSpace(entry.Entity.CriadoPor))
                    entry.Entity.CriadoPor = "Sistema";
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.AtualizadoEm = now;
                if (string.IsNullOrWhiteSpace(entry.Entity.AtualizadoPor))
                    entry.Entity.AtualizadoPor = "Sistema";
            }
        }
    }
}
