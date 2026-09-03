using Crm.Domain.Entities;
using System.Linq.Expressions;

namespace Crm.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> ObterPorIdAsync(long id, CancellationToken ct = default);
    Task<List<T>> ListarTodosAsync(CancellationToken ct = default);
    Task<List<T>> BuscarAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task AdicionarAsync(T entity, CancellationToken ct = default);
    void Atualizar(T entity);
    void Remover(T entity);
}

public interface IPropostaRepository : IRepository<Proposta>
{
    Task<Proposta?> ObterCompletaPorIdAsync(long id, CancellationToken ct = default);
    Task<List<Proposta>> ListarComFiltrosAsync(int? statusId, long? clienteId, string? busca, int pagina = 1, int tamanhoPagina = 20, CancellationToken ct = default);
    Task<int> ContarAsync(int? statusId, long? clienteId, string? busca, CancellationToken ct = default);
    Task<long> ObterProximoCodigoAsync(CancellationToken ct = default);
}

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> ObterComContatosAsync(long id, CancellationToken ct = default);
    Task<List<Cliente>> ListarAtivosAsync(string? busca = null, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
