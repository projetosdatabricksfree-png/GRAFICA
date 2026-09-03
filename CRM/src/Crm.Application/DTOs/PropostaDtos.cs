using Crm.Domain.ValueObjects;

namespace Crm.Application.DTOs;

public class PropostaDto
{
    public long Id { get; set; }
    public long Codigo { get; set; }
    public int Versao { get; set; }
    public long EmpresaEmissoraId { get; set; }
    public string EmpresaEmissoraNome { get; set; } = string.Empty;
    public long ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string? ClienteDocumento { get; set; }
    public string? ClienteCodigo { get; set; }
    public long? ContatoId { get; set; }
    public string? ContatoNome { get; set; }
    public long RepresentanteId { get; set; }
    public string RepresentanteNome { get; set; } = string.Empty;
    public string? RepresentanteTelefone { get; set; }
    public string? RepresentanteEmail { get; set; }
    public int StatusId { get; set; }
    public string StatusNome { get; set; } = string.Empty;
    public string StatusCorHex { get; set; } = string.Empty;
    public int FormaPagamentoId { get; set; }
    public string FormaPagamentoNome { get; set; } = string.Empty;
    public DateTime DataEmissao { get; set; }
    public int ValidadeDias { get; set; }
    public string PrazoEntrega { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public string ClausulasComerciais { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }

    public List<PropostaItemDto> Itens { get; set; } = new();
    public List<HistoricoInteracaoDto> Historico { get; set; } = new();
}

public class PropostaItemDto
{
    public long Id { get; set; }
    public long? ProdutoServicoId { get; set; }
    public int ItemNumero { get; set; }
    public string? CodigoItem { get; set; }
    public string Grupo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public EspecificacaoTecnica Especificacoes { get; set; } = new();
}

public class HistoricoInteracaoDto
{
    public long Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public string? Usuario { get; set; }
}

public class CriarPropostaRequest
{
    public long EmpresaEmissoraId { get; set; }
    public long ClienteId { get; set; }
    public long? ContatoId { get; set; }
    public long RepresentanteId { get; set; }
    public int FormaPagamentoId { get; set; } = 1;
    public int ValidadeDias { get; set; } = 10;
    public string PrazoEntrega { get; set; } = "A combinar";
    public string? Observacoes { get; set; }
    public List<ItemPropostaRequest> Itens { get; set; } = new();
}

public class ItemPropostaRequest
{
    public long? ProdutoServicoId { get; set; }
    public string? CodigoItem { get; set; }
    public string Grupo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public EspecificacaoTecnica Especificacoes { get; set; } = new();
}

public class AtualizarPropostaRequest
{
    public long EmpresaEmissoraId { get; set; }
    public long ClienteId { get; set; }
    public long? ContatoId { get; set; }
    public long RepresentanteId { get; set; }
    public int StatusId { get; set; }
    public int FormaPagamentoId { get; set; }
    public int ValidadeDias { get; set; }
    public string PrazoEntrega { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public List<ItemPropostaRequest> Itens { get; set; } = new();
}
