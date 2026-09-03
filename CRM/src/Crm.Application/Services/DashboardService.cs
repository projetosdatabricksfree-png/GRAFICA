using Crm.Application.DTOs;
using Crm.Application.Interfaces;
using Crm.Domain.Entities;

namespace Crm.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IPropostaRepository _propostaRepo;
    private readonly IClienteRepository _clienteRepo;

    public DashboardService(IPropostaRepository propostaRepo, IClienteRepository clienteRepo)
    {
        _propostaRepo = propostaRepo;
        _clienteRepo = clienteRepo;
    }

    public async Task<DashboardSummaryDto> ObterResumoAsync(CancellationToken ct = default)
    {
        var todas = await _propostaRepo.ListarComFiltrosAsync(null, null, null, 1, 50, ct);
        var clientes = await _clienteRepo.ListarAtivosAsync(null, ct);

        var total = await _propostaRepo.ContarAsync(null, null, null, ct);
        var abertas = todas.Count(p => p.StatusId == StatusProposta.Rascunho || p.StatusId == StatusProposta.Enviada);
        var aprovadas = todas.Count(p => p.StatusId == StatusProposta.Aprovada);

        var valorTotalOrcado = todas.Sum(p => p.ValorTotal);
        var valorTotalAprovado = todas.Where(p => p.StatusId == StatusProposta.Aprovada).Sum(p => p.ValorTotal);

        return new DashboardSummaryDto
        {
            TotalPropostas = total,
            PropostasAbertas = abertas,
            PropostasAprovadas = aprovadas,
            ValorTotalOrcado = valorTotalOrcado,
            ValorTotalAprovado = valorTotalAprovado,
            TotalClientes = clientes.Count,
            UltimasPropostas = todas.Take(5).Select(p => new PropostaDto
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Versao = p.Versao,
                ClienteNome = p.Cliente?.Nome ?? "CONSUMIDOR",
                RepresentanteNome = p.Representante?.Nome ?? string.Empty,
                StatusId = p.StatusId,
                StatusNome = p.Status?.Nome ?? "Rascunho",
                StatusCorHex = p.Status?.CorHex ?? "#757575",
                DataEmissao = p.DataEmissao,
                ValorTotal = p.ValorTotal
            }).ToList()
        };
    }
}
