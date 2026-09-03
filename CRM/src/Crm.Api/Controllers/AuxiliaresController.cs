using Crm.Application.DTOs;
using Crm.Application.Interfaces;
using Crm.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class AuxiliaresController : ControllerBase
{
    private readonly IRepository<ProdutoServico> _produtoRepo;
    private readonly IRepository<Representante> _representanteRepo;
    private readonly IRepository<EmpresaEmissora> _empresaRepo;
    private readonly IRepository<FormaPagamento> _formaPagamentoRepo;
    private readonly IRepository<StatusProposta> _statusRepo;
    private readonly IDashboardService _dashboardService;

    public AuxiliaresController(
        IRepository<ProdutoServico> produtoRepo,
        IRepository<Representante> representanteRepo,
        IRepository<EmpresaEmissora> empresaRepo,
        IRepository<FormaPagamento> formaPagamentoRepo,
        IRepository<StatusProposta> statusRepo,
        IDashboardService dashboardService)
    {
        _produtoRepo = produtoRepo;
        _representanteRepo = representanteRepo;
        _empresaRepo = empresaRepo;
        _formaPagamentoRepo = formaPagamentoRepo;
        _statusRepo = statusRepo;
        _dashboardService = dashboardService;
    }

    [HttpGet("produtos")]
    public async Task<ActionResult<List<ProdutoServicoDto>>> ListarProdutos(CancellationToken ct = default)
    {
        var produtos = await _produtoRepo.BuscarAsync(p => p.Ativo, ct);
        return Ok(produtos.Select(p => new ProdutoServicoDto
        {
            Id = p.Id,
            Codigo = p.Codigo,
            Grupo = p.Grupo,
            DescricaoBase = p.DescricaoBase,
            PrecoBase = p.PrecoBase,
            EspecificacoesPadrao = p.EspecificacoesPadrao,
            Ativo = p.Ativo
        }).ToList());
    }

    [HttpGet("representantes")]
    public async Task<ActionResult<List<RepresentanteDto>>> ListarRepresentantes(CancellationToken ct = default)
    {
        var reps = await _representanteRepo.BuscarAsync(r => r.Ativo, ct);
        return Ok(reps.Select(r => new RepresentanteDto
        {
            Id = r.Id,
            Nome = r.Nome,
            Telefone = r.Telefone,
            Email = r.Email,
            Ativo = r.Ativo
        }).ToList());
    }

    [HttpGet("empresas-emissoras")]
    public async Task<ActionResult<List<EmpresaEmissoraDto>>> ListarEmpresas(CancellationToken ct = default)
    {
        var empresas = await _empresaRepo.BuscarAsync(e => e.Ativo, ct);
        return Ok(empresas.Select(e => new EmpresaEmissoraDto
        {
            Id = e.Id,
            RazaoSocial = e.RazaoSocial,
            NomeFantasia = e.NomeFantasia,
            Unidade = e.Unidade,
            Cnpj = e.Cnpj,
            Endereco = e.Endereco,
            Cep = e.Cep,
            Cidade = e.Cidade,
            Uf = e.Uf,
            Telefone = e.Telefone,
            Site = e.Site,
            Email = e.Email
        }).ToList());
    }

    [HttpGet("formas-pagamento")]
    public async Task<ActionResult<List<FormaPagamento>>> ListarFormasPagamento(CancellationToken ct = default)
    {
        return Ok(await _formaPagamentoRepo.BuscarAsync(f => f.Ativo, ct));
    }

    [HttpGet("status-propostas")]
    public async Task<ActionResult<List<StatusProposta>>> ListarStatusPropostas(CancellationToken ct = default)
    {
        return Ok(await _statusRepo.ListarTodosAsync(ct));
    }

    [HttpGet("dashboard/resumo")]
    public async Task<ActionResult<DashboardSummaryDto>> ObterDashboard(CancellationToken ct = default)
    {
        return Ok(await _dashboardService.ObterResumoAsync(ct));
    }
}
