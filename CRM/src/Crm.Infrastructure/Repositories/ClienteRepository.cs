using Crm.Application.Interfaces;
using Crm.Domain.Entities;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Crm.Infrastructure.Repositories;

public class ClienteRepository : Repository<Cliente>, IClienteRepository
{
    public ClienteRepository(CrmDbContext context) : base(context)
    {
    }

    public async Task<Cliente?> ObterComContatosAsync(long id, CancellationToken ct = default)
    {
        return await _context.Clientes
            .Include(c => c.Contatos)
            .Include(c => c.Propostas)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<List<Cliente>> ListarAtivosAsync(string? busca = null, CancellationToken ct = default)
    {
        var query = _context.Clientes
            .Include(c => c.Contatos)
            .Include(c => c.Propostas)
            .Where(c => c.Ativo)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim().ToLower();
            query = query.Where(c =>
                c.Nome.ToLower().Contains(termo) ||
                (c.CodigoCliente != null && c.CodigoCliente.ToLower().Contains(termo)) ||
                (c.Documento != null && c.Documento.Contains(termo)));
        }

        return await query.OrderBy(c => c.Nome).ToListAsync(ct);
    }
}

public class UnitOfWork : IUnitOfWork
{
    private readonly CrmDbContext _context;

    public UnitOfWork(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}
