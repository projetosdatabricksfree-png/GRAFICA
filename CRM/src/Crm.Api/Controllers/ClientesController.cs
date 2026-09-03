using Crm.Application.DTOs;
using Crm.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/v1/clientes")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly IValidator<CriarClienteRequest> _validator;

    public ClientesController(IClienteService clienteService, IValidator<CriarClienteRequest> validator)
    {
        _clienteService = clienteService;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ClienteDto>>> Listar([FromQuery] string? busca, CancellationToken ct = default)
    {
        var clientes = await _clienteService.ListarAsync(busca, ct);
        return Ok(clientes);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ClienteDto>> ObterPorId(long id, CancellationToken ct = default)
    {
        var cliente = await _clienteService.ObterPorIdAsync(id, ct);
        if (cliente == null) return NotFound(new { mensagem = $"Cliente {id} não encontrado." });
        return Ok(cliente);
    }

    [HttpPost]
    public async Task<ActionResult<ClienteDto>> Criar([FromBody] CriarClienteRequest request, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var criado = await _clienteService.CriarAsync(request, User?.Identity?.Name ?? "API", ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criado.Id }, criado);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ClienteDto>> Atualizar(long id, [FromBody] CriarClienteRequest request, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        try
        {
            var atualizado = await _clienteService.AtualizarAsync(id, request, User?.Identity?.Name ?? "API", ct);
            return Ok(atualizado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Excluir(long id, CancellationToken ct = default)
    {
        var sucesso = await _clienteService.ExcluirLogicoAsync(id, User?.Identity?.Name ?? "API", ct);
        if (!sucesso) return NotFound(new { mensagem = $"Cliente {id} não encontrado." });
        return NoContent();
    }
}
