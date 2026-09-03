using Crm.Application.DTOs;
using Crm.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/v1/propostas")]
public class PropostasController : ControllerBase
{
    private readonly IPropostaService _propostaService;
    private readonly IPdfService _pdfService;
    private readonly IValidator<CriarPropostaRequest> _validator;

    public PropostasController(
        IPropostaService propostaService,
        IPdfService pdfService,
        IValidator<CriarPropostaRequest> validator)
    {
        _propostaService = propostaService;
        _pdfService = pdfService;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<List<PropostaDto>>> Listar(
        [FromQuery] int? statusId,
        [FromQuery] long? clienteId,
        [FromQuery] string? busca,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20,
        CancellationToken ct = default)
    {
        var total = await _propostaService.ContarAsync(statusId, clienteId, busca, ct);
        var itens = await _propostaService.ListarAsync(statusId, clienteId, busca, pagina, tamanhoPagina, ct);

        Response.Headers.Append("X-Total-Count", total.ToString());
        return Ok(itens);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PropostaDto>> ObterPorId(long id, CancellationToken ct = default)
    {
        var proposta = await _propostaService.ObterPorIdAsync(id, ct);
        if (proposta == null) return NotFound(new { mensagem = $"Proposta {id} não encontrada." });
        return Ok(proposta);
    }

    [HttpPost]
    public async Task<ActionResult<PropostaDto>> Criar([FromBody] CriarPropostaRequest request, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var criado = await _propostaService.CriarAsync(request, User?.Identity?.Name ?? "API", ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<PropostaDto>> Atualizar(long id, [FromBody] AtualizarPropostaRequest request, CancellationToken ct = default)
    {
        try
        {
            var atualizado = await _propostaService.AtualizarAsync(id, request, User?.Identity?.Name ?? "API", ct);
            return Ok(atualizado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpPost("{id:long}/clonar")]
    public async Task<ActionResult<PropostaDto>> Clonar(long id, CancellationToken ct = default)
    {
        try
        {
            var clone = await _propostaService.ClonarAsync(id, User?.Identity?.Name ?? "API", ct);
            return CreatedAtAction(nameof(ObterPorId), new { id = clone.Id }, clone);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpPatch("{id:long}/status")]
    public async Task<IActionResult> AlterarStatus(long id, [FromBody] AlterarStatusRequest request, CancellationToken ct = default)
    {
        var sucesso = await _propostaService.AlterarStatusAsync(id, request.StatusId, request.Motivo, User?.Identity?.Name ?? "API", ct);
        if (!sucesso) return NotFound(new { mensagem = $"Proposta {id} não encontrada." });
        return NoContent();
    }

    [HttpGet("{id:long}/pdf")]
    public async Task<IActionResult> ObterPdf(long id, CancellationToken ct = default)
    {
        try
        {
            var bytes = await _pdfService.GerarPropostaPdfAsync(id, ct);
            var proposta = await _propostaService.ObterPorIdAsync(id, ct);
            var codigo = proposta?.Codigo ?? id;
            var fileName = $"Proposta_{codigo}.pdf";
            return File(bytes, "application/pdf", fileName);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}

public class AlterarStatusRequest
{
    public int StatusId { get; set; }
    public string? Motivo { get; set; }
}
