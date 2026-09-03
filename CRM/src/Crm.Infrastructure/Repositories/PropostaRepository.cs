using Crm.Application.Interfaces;
using Crm.Domain.Entities;
using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Crm.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly CrmDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(CrmDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> ObterPorIdAsync(long id, CancellationToken ct = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, ct);
    }

    public async Task<List<T>> ListarTodosAsync(CancellationToken ct = default)
    {
        return await _dbSet.ToListAsync(ct);
    }

    public async Task<List<T>> BuscarAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(ct);
    }

    public async Task AdicionarAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
    }

    public void Atualizar(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Remover(T entity)
    {
        _dbSet.Remove(entity);
    }
}

public class PropostaRepository : Repository<Proposta>, IPropostaRepository
{
    public PropostaRepository(CrmDbContext context) : base(context)
    {
    }

    public async Task<Proposta?> ObterCompletaPorIdAsync(long id, CancellationToken ct = default)
    {
        return await _context.Propostas
            .Include(p => p.EmpresaEmissora)
            .Include(p => p.Cliente)
            .Include(p => p.Contato)
            .Include(p => p.Representante)
            .Include(p => p.Status)
            .Include(p => p.FormaPagamento)
            .Include(p => p.Itens)
            .Include(p => p.Historico)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<List<Proposta>> ListarComFiltrosAsync(int? statusId, long? clienteId, string? busca, int pagina = 1, int tamanhoPagina = 20, CancellationToken ct = default)
    {
        var query = _context.Propostas
            .Include(p => p.Cliente)
            .Include(p => p.Representante)
            .Include(p => p.Status)
            .Include(p => p.FormaPagamento)
            .AsNoTracking()
            .AsQueryable();

        if (statusId.HasValue && statusId.Value > 0)
            query = query.Where(p => p.StatusId == statusId.Value);

        if (clienteId.HasValue && clienteId.Value > 0)
            query = query.Where(p => p.ClienteId == clienteId.Value);

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim().ToLower();
            query = query.Where(p =>
                p.Codigo.ToString().Contains(termo) ||
                (p.Cliente != null && p.Cliente.Nome.ToLower().Contains(termo)) ||
                (p.Representante != null && p.Representante.Nome.ToLower().Contains(termo)));
        }

        return await query
            .OrderByDescending(p => p.DataEmissao)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(ct);
    }

    public async Task<int> ContarAsync(int? statusId, long? clienteId, string? busca, CancellationToken ct = default)
    {
        var query = _context.Propostas.AsQueryable();

        if (statusId.HasValue && statusId.Value > 0)
            query = query.Where(p => p.StatusId == statusId.Value);

        if (clienteId.HasValue && clienteId.Value > 0)
            query = query.Where(p => p.ClienteId == clienteId.Value);

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim().ToLower();
            query = query.Where(p =>
                p.Codigo.ToString().Contains(termo) ||
                (p.Cliente != null && p.Cliente.Nome.ToLower().Contains(termo)));
        }

        return await query.CountAsync(ct);
    }

    public async Task<long> ObterProximoCodigoAsync(CancellationToken ct = default)
    {
        // Usa sequence oficial do Postgres ou MAX + 1 se em memória/fallback
        try
        {
            var connection = _context.Database.GetDbConnection();
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT nextval('proposta_codigo_seq');";
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync(ct);
            var result = await cmd.ExecuteScalarAsync(ct);
            if (result != null && long.TryParse(result.ToString(), out var seq))
                return seq;
        }
        catch
        {
            // Fallback
        }

        var max = await _context.Propostas.MaxAsync(p => (long?)p.Codigo, ct) ?? 62631;
        return max + 1;
    }
}
