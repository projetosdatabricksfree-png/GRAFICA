using Crm.Application.DTOs;
using Crm.Application.Interfaces;
using Crm.Domain.Entities;

namespace Crm.Application.Services;

public class PropostaService : IPropostaService
{
    private readonly IPropostaRepository _propostaRepo;
    private readonly IRepository<EmpresaEmissora> _empresaRepo;
    private readonly IRepository<Cliente> _clienteRepo;
    private readonly IRepository<Representante> _representanteRepo;
    private readonly IUnitOfWork _uow;

    public PropostaService(
        IPropostaRepository propostaRepo,
        IRepository<EmpresaEmissora> empresaRepo,
        IRepository<Cliente> clienteRepo,
        IRepository<Representante> representanteRepo,
        IUnitOfWork uow)
    {
        _propostaRepo = propostaRepo;
        _empresaRepo = empresaRepo;
        _clienteRepo = clienteRepo;
        _representanteRepo = representanteRepo;
        _uow = uow;
    }

    public async Task<PropostaDto?> ObterPorIdAsync(long id, CancellationToken ct = default)
    {
        var proposta = await _propostaRepo.ObterCompletaPorIdAsync(id, ct);
        return proposta == null ? null : MapearParaDto(proposta);
    }

    public async Task<List<PropostaDto>> ListarAsync(int? statusId, long? clienteId, string? busca, int pagina = 1, int tamanhoPagina = 20, CancellationToken ct = default)
    {
        var lista = await _propostaRepo.ListarComFiltrosAsync(statusId, clienteId, busca, pagina, tamanhoPagina, ct);
        return lista.Select(MapearParaDto).ToList();
    }

    public async Task<int> ContarAsync(int? statusId, long? clienteId, string? busca, CancellationToken ct = default)
    {
        return await _propostaRepo.ContarAsync(statusId, clienteId, busca, ct);
    }

    public async Task<PropostaDto> CriarAsync(CriarPropostaRequest request, string? usuario = null, CancellationToken ct = default)
    {
        var novoCodigo = await _propostaRepo.ObterProximoCodigoAsync(ct);

        var proposta = new Proposta
        {
            Codigo = novoCodigo,
            Versao = 1,
            EmpresaEmissoraId = request.EmpresaEmissoraId,
            ClienteId = request.ClienteId,
            ContatoId = request.ContatoId,
            RepresentanteId = request.RepresentanteId,
            FormaPagamentoId = request.FormaPagamentoId,
            StatusId = StatusProposta.Rascunho,
            DataEmissao = DateTime.UtcNow,
            ValidadeDias = request.ValidadeDias,
            PrazoEntrega = request.PrazoEntrega,
            Observacoes = request.Observacoes,
            CriadoPor = usuario ?? "Sistema"
        };

        int numero = 1;
        foreach (var itemReq in request.Itens)
        {
            var item = new PropostaItem
            {
                ProdutoServicoId = itemReq.ProdutoServicoId,
                ItemNumero = numero++,
                CodigoItem = itemReq.CodigoItem ?? (116317 + numero).ToString(),
                Grupo = itemReq.Grupo,
                Descricao = itemReq.Descricao,
                Quantidade = itemReq.Quantidade,
                ValorUnitario = itemReq.ValorUnitario,
                Especificacoes = itemReq.Especificacoes ?? new()
            };
            item.CalcularTotal();
            proposta.Itens.Add(item);
        }

        proposta.RecalcularValorTotal();

        proposta.Historico.Add(new HistoricoInteracao
        {
            Tipo = "Criacao",
            Descricao = $"Proposta #{proposta.Codigo} criada com {proposta.Itens.Count} itens. Total: R$ {proposta.ValorTotal:N2}",
            Data = DateTime.UtcNow,
            Usuario = usuario ?? "Sistema"
        });

        await _propostaRepo.AdicionarAsync(proposta, ct);
        await _uow.CommitAsync(ct);

        return (await ObterPorIdAsync(proposta.Id, ct))!;
    }

    public async Task<PropostaDto> AtualizarAsync(long id, AtualizarPropostaRequest request, string? usuario = null, CancellationToken ct = default)
    {
        var proposta = await _propostaRepo.ObterCompletaPorIdAsync(id, ct);
        if (proposta == null)
            throw new KeyNotFoundException($"Proposta {id} não encontrada.");

        proposta.EmpresaEmissoraId = request.EmpresaEmissoraId;
        proposta.ClienteId = request.ClienteId;
        proposta.ContatoId = request.ContatoId;
        proposta.RepresentanteId = request.RepresentanteId;
        proposta.StatusId = request.StatusId;
        proposta.FormaPagamentoId = request.FormaPagamentoId;
        proposta.ValidadeDias = request.ValidadeDias;
        proposta.PrazoEntrega = request.PrazoEntrega;
        proposta.Observacoes = request.Observacoes;
        proposta.AtualizadoPor = usuario ?? "Sistema";
        proposta.AtualizadoEm = DateTime.UtcNow;

        proposta.Itens.Clear();
        int numero = 1;
        foreach (var itemReq in request.Itens)
        {
            var item = new PropostaItem
            {
                ProdutoServicoId = itemReq.ProdutoServicoId,
                ItemNumero = numero++,
                CodigoItem = itemReq.CodigoItem ?? (116317 + numero).ToString(),
                Grupo = itemReq.Grupo,
                Descricao = itemReq.Descricao,
                Quantidade = itemReq.Quantidade,
                ValorUnitario = itemReq.ValorUnitario,
                Especificacoes = itemReq.Especificacoes ?? new()
            };
            item.CalcularTotal();
            proposta.Itens.Add(item);
        }

        proposta.RecalcularValorTotal();

        proposta.Historico.Add(new HistoricoInteracao
        {
            Tipo = "Atualizacao",
            Descricao = $"Proposta #{proposta.Codigo} atualizada. Novo total: R$ {proposta.ValorTotal:N2}",
            Data = DateTime.UtcNow,
            Usuario = usuario ?? "Sistema"
        });

        _propostaRepo.Atualizar(proposta);
        await _uow.CommitAsync(ct);

        return (await ObterPorIdAsync(proposta.Id, ct))!;
    }

    public async Task<PropostaDto> ClonarAsync(long id, string? usuario = null, CancellationToken ct = default)
    {
        var original = await _propostaRepo.ObterCompletaPorIdAsync(id, ct);
        if (original == null)
            throw new KeyNotFoundException($"Proposta {id} não encontrada.");

        var novoCodigo = await _propostaRepo.ObterProximoCodigoAsync(ct);

        var clone = new Proposta
        {
            Codigo = novoCodigo,
            Versao = 1,
            EmpresaEmissoraId = original.EmpresaEmissoraId,
            ClienteId = original.ClienteId,
            ContatoId = original.ContatoId,
            RepresentanteId = original.RepresentanteId,
            FormaPagamentoId = original.FormaPagamentoId,
            StatusId = StatusProposta.Rascunho,
            DataEmissao = DateTime.UtcNow,
            ValidadeDias = original.ValidadeDias,
            PrazoEntrega = original.PrazoEntrega,
            Observacoes = original.Observacoes,
            ClausulasComerciais = original.ClausulasComerciais,
            CriadoPor = usuario ?? "Sistema"
        };

        foreach (var itemOriginal in original.Itens)
        {
            var novoItem = new PropostaItem
            {
                ProdutoServicoId = itemOriginal.ProdutoServicoId,
                ItemNumero = itemOriginal.ItemNumero,
                CodigoItem = itemOriginal.CodigoItem,
                Grupo = itemOriginal.Grupo,
                Descricao = itemOriginal.Descricao,
                Quantidade = itemOriginal.Quantidade,
                ValorUnitario = itemOriginal.ValorUnitario,
                Especificacoes = new Domain.ValueObjects.EspecificacaoTecnica
                {
                    Formato = itemOriginal.Especificacoes.Formato,
                    Papel = itemOriginal.Especificacoes.Papel,
                    Gramatura = itemOriginal.Especificacoes.Gramatura,
                    Cores = itemOriginal.Especificacoes.Cores,
                    Acabamento = itemOriginal.Especificacoes.Acabamento,
                    Observacoes = itemOriginal.Especificacoes.Observacoes
                }
            };
            novoItem.CalcularTotal();
            clone.Itens.Add(novoItem);
        }

        clone.RecalcularValorTotal();

        clone.Historico.Add(new HistoricoInteracao
        {
            Tipo = "Clonagem",
            Descricao = $"Clonada a partir da proposta anterior #{original.Codigo}",
            Data = DateTime.UtcNow,
            Usuario = usuario ?? "Sistema"
        });

        await _propostaRepo.AdicionarAsync(clone, ct);
        await _uow.CommitAsync(ct);

        return (await ObterPorIdAsync(clone.Id, ct))!;
    }

    public async Task<bool> AlterarStatusAsync(long id, int novoStatusId, string? motivo = null, string? usuario = null, CancellationToken ct = default)
    {
        var proposta = await _propostaRepo.ObterCompletaPorIdAsync(id, ct);
        if (proposta == null) return false;

        var statusAnterior = proposta.Status?.Nome ?? proposta.StatusId.ToString();
        proposta.StatusId = novoStatusId;
        proposta.AtualizadoEm = DateTime.UtcNow;
        proposta.AtualizadoPor = usuario ?? "Sistema";

        proposta.Historico.Add(new HistoricoInteracao
        {
            Tipo = "AlteracaoStatus",
            Descricao = $"Status alterado de '{statusAnterior}' para Id {novoStatusId}. {motivo}".Trim(),
            Data = DateTime.UtcNow,
            Usuario = usuario ?? "Sistema"
        });

        _propostaRepo.Atualizar(proposta);
        await _uow.CommitAsync(ct);
        return true;
    }

    private static PropostaDto MapearParaDto(Proposta p)
    {
        return new PropostaDto
        {
            Id = p.Id,
            Codigo = p.Codigo,
            Versao = p.Versao,
            EmpresaEmissoraId = p.EmpresaEmissoraId,
            EmpresaEmissoraNome = p.EmpresaEmissora?.NomeFantasia ?? p.EmpresaEmissora?.RazaoSocial ?? "AgsPrint Soluções Gráficas",
            ClienteId = p.ClienteId,
            ClienteNome = p.Cliente?.Nome ?? "CONSUMIDOR",
            ClienteDocumento = p.Cliente?.Documento,
            ClienteCodigo = p.Cliente?.CodigoCliente,
            ContatoId = p.ContatoId,
            ContatoNome = p.Contato?.Nome,
            RepresentanteId = p.RepresentanteId,
            RepresentanteNome = p.Representante?.Nome ?? string.Empty,
            RepresentanteTelefone = p.Representante?.Telefone,
            RepresentanteEmail = p.Representante?.Email,
            StatusId = p.StatusId,
            StatusNome = p.Status?.Nome ?? "Rascunho",
            StatusCorHex = p.Status?.CorHex ?? "#757575",
            FormaPagamentoId = p.FormaPagamentoId,
            FormaPagamentoNome = p.FormaPagamento?.Nome ?? "A Vista",
            DataEmissao = p.DataEmissao,
            ValidadeDias = p.ValidadeDias,
            PrazoEntrega = p.PrazoEntrega,
            Observacoes = p.Observacoes,
            ClausulasComerciais = p.ClausulasComerciais,
            ValorTotal = p.ValorTotal,
            Itens = p.Itens.OrderBy(i => i.ItemNumero).Select(i => new PropostaItemDto
            {
                Id = i.Id,
                ProdutoServicoId = i.ProdutoServicoId,
                ItemNumero = i.ItemNumero,
                CodigoItem = i.CodigoItem,
                Grupo = i.Grupo,
                Descricao = i.Descricao,
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario,
                ValorTotal = i.ValorTotal,
                Especificacoes = i.Especificacoes
            }).ToList(),
            Historico = p.Historico.OrderByDescending(h => h.Data).Select(h => new HistoricoInteracaoDto
            {
                Id = h.Id,
                Tipo = h.Tipo,
                Descricao = h.Descricao,
                Data = h.Data,
                Usuario = h.Usuario
            }).ToList()
        };
    }
}
