using Crm.Application.DTOs;
using Crm.Application.Interfaces;
using Crm.Domain.Entities;

namespace Crm.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepo;
    private readonly IUnitOfWork _uow;

    public ClienteService(IClienteRepository clienteRepo, IUnitOfWork uow)
    {
        _clienteRepo = clienteRepo;
        _uow = uow;
    }

    public async Task<List<ClienteDto>> ListarAsync(string? busca = null, CancellationToken ct = default)
    {
        var clientes = await _clienteRepo.ListarAtivosAsync(busca, ct);
        return clientes.Select(MapearParaDto).ToList();
    }

    public async Task<ClienteDto?> ObterPorIdAsync(long id, CancellationToken ct = default)
    {
        var c = await _clienteRepo.ObterComContatosAsync(id, ct);
        return c == null ? null : MapearParaDto(c);
    }

    public async Task<ClienteDto> CriarAsync(CriarClienteRequest request, string? usuario = null, CancellationToken ct = default)
    {
        var cliente = new Cliente
        {
            CodigoCliente = request.CodigoCliente,
            Nome = request.Nome,
            Documento = request.Documento,
            Telefone = request.Telefone,
            Email = request.Email,
            Endereco = request.Endereco,
            Cidade = request.Cidade,
            Uf = request.Uf,
            Cep = request.Cep,
            Ativo = true,
            CriadoPor = usuario ?? "Sistema",
            CriadoEm = DateTime.UtcNow
        };

        foreach (var c in request.Contatos)
        {
            cliente.Contatos.Add(new Contato
            {
                Nome = c.Nome,
                Cargo = c.Cargo,
                Telefone = c.Telefone,
                Email = c.Email,
                Principal = c.Principal,
                CriadoPor = usuario ?? "Sistema",
                CriadoEm = DateTime.UtcNow
            });
        }

        await _clienteRepo.AdicionarAsync(cliente, ct);
        await _uow.CommitAsync(ct);

        return (await ObterPorIdAsync(cliente.Id, ct))!;
    }

    public async Task<ClienteDto> AtualizarAsync(long id, CriarClienteRequest request, string? usuario = null, CancellationToken ct = default)
    {
        var cliente = await _clienteRepo.ObterComContatosAsync(id, ct);
        if (cliente == null)
            throw new KeyNotFoundException($"Cliente {id} não encontrado.");

        cliente.CodigoCliente = request.CodigoCliente;
        cliente.Nome = request.Nome;
        cliente.Documento = request.Documento;
        cliente.Telefone = request.Telefone;
        cliente.Email = request.Email;
        cliente.Endereco = request.Endereco;
        cliente.Cidade = request.Cidade;
        cliente.Uf = request.Uf;
        cliente.Cep = request.Cep;
        cliente.AtualizadoPor = usuario ?? "Sistema";
        cliente.AtualizadoEm = DateTime.UtcNow;

        _clienteRepo.Atualizar(cliente);
        await _uow.CommitAsync(ct);

        return (await ObterPorIdAsync(cliente.Id, ct))!;
    }

    public async Task<bool> ExcluirLogicoAsync(long id, string? usuario = null, CancellationToken ct = default)
    {
        var cliente = await _clienteRepo.ObterPorIdAsync(id, ct);
        if (cliente == null) return false;

        // Regra do domínio: soft delete para nunca perder histórico comercial
        cliente.Ativo = false;
        cliente.AtualizadoPor = usuario ?? "Sistema";
        cliente.AtualizadoEm = DateTime.UtcNow;

        _clienteRepo.Atualizar(cliente);
        await _uow.CommitAsync(ct);
        return true;
    }

    private static ClienteDto MapearParaDto(Cliente c)
    {
        return new ClienteDto
        {
            Id = c.Id,
            CodigoCliente = c.CodigoCliente,
            Nome = c.Nome,
            Documento = c.Documento,
            Telefone = c.Telefone,
            Email = c.Email,
            Endereco = c.Endereco,
            Cidade = c.Cidade,
            Uf = c.Uf,
            Cep = c.Cep,
            Ativo = c.Ativo,
            TotalPropostas = c.Propostas?.Count ?? 0,
            Contatos = c.Contatos.Select(ct => new ContatoDto
            {
                Id = ct.Id,
                ClienteId = ct.ClienteId,
                Nome = ct.Nome,
                Cargo = ct.Cargo,
                Telefone = ct.Telefone,
                Email = ct.Email,
                Principal = ct.Principal
            }).ToList()
        };
    }
}
