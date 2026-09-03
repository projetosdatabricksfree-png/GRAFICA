using Crm.Domain.ValueObjects;

namespace Crm.Application.DTOs;

public class ClienteDto
{
    public long Id { get; set; }
    public string? CodigoCliente { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Documento { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Uf { get; set; }
    public string? Cep { get; set; }
    public bool Ativo { get; set; }
    public int TotalPropostas { get; set; }
    public List<ContatoDto> Contatos { get; set; } = new();
}

public class ContatoDto
{
    public long Id { get; set; }
    public long ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public bool Principal { get; set; }
}

public class CriarClienteRequest
{
    public string? CodigoCliente { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Documento { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; } = "São Paulo";
    public string? Uf { get; set; } = "SP";
    public string? Cep { get; set; }
    public List<CriarContatoRequest> Contatos { get; set; } = new();
}

public class CriarContatoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public bool Principal { get; set; }
}

public class RepresentanteDto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public bool Ativo { get; set; }
}

public class EmpresaEmissoraDto
{
    public long Id { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string? Unidade { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Site { get; set; }
    public string? Email { get; set; }
}

public class ProdutoServicoDto
{
    public long Id { get; set; }
    public string? Codigo { get; set; }
    public string Grupo { get; set; } = string.Empty;
    public string DescricaoBase { get; set; } = string.Empty;
    public decimal PrecoBase { get; set; }
    public EspecificacaoTecnica EspecificacoesPadrao { get; set; } = new();
    public bool Ativo { get; set; }
}

public class DashboardSummaryDto
{
    public int TotalPropostas { get; set; }
    public int PropostasAbertas { get; set; }
    public int PropostasAprovadas { get; set; }
    public decimal ValorTotalOrcado { get; set; }
    public decimal ValorTotalAprovado { get; set; }
    public int TotalClientes { get; set; }
    public List<PropostaDto> UltimasPropostas { get; set; } = new();
}
