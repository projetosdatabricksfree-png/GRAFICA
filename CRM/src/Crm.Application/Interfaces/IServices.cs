using Crm.Application.DTOs;

namespace Crm.Application.Interfaces;

public interface IPropostaService
{
    Task<PropostaDto?> ObterPorIdAsync(long id, CancellationToken ct = default);
    Task<List<PropostaDto>> ListarAsync(int? statusId, long? clienteId, string? busca, int pagina = 1, int tamanhoPagina = 20, CancellationToken ct = default);
    Task<int> ContarAsync(int? statusId, long? clienteId, string? busca, CancellationToken ct = default);
    Task<PropostaDto> CriarAsync(CriarPropostaRequest request, string? usuario = null, CancellationToken ct = default);
    Task<PropostaDto> AtualizarAsync(long id, AtualizarPropostaRequest request, string? usuario = null, CancellationToken ct = default);
    Task<PropostaDto> ClonarAsync(long id, string? usuario = null, CancellationToken ct = default);
    Task<bool> AlterarStatusAsync(long id, int novoStatusId, string? motivo = null, string? usuario = null, CancellationToken ct = default);
}

public interface IClienteService
{
    Task<List<ClienteDto>> ListarAsync(string? busca = null, CancellationToken ct = default);
    Task<ClienteDto?> ObterPorIdAsync(long id, CancellationToken ct = default);
    Task<ClienteDto> CriarAsync(CriarClienteRequest request, string? usuario = null, CancellationToken ct = default);
    Task<ClienteDto> AtualizarAsync(long id, CriarClienteRequest request, string? usuario = null, CancellationToken ct = default);
    Task<bool> ExcluirLogicoAsync(long id, string? usuario = null, CancellationToken ct = default);
}

public interface IPdfService
{
    Task<byte[]> GerarPropostaPdfAsync(long propostaId, CancellationToken ct = default);
    byte[] GerarPropostaPdf(PropostaDto proposta);
}

public interface IDashboardService
{
    Task<DashboardSummaryDto> ObterResumoAsync(CancellationToken ct = default);
}
