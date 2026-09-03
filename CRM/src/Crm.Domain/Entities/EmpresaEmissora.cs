using Crm.Domain.Common;

namespace Crm.Domain.Entities;

public class EmpresaEmissora : IAuditableEntity
{
    public long Id { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string? Unidade { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string? InscricaoEstadual { get; set; }
    public string Endereco { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Cidade { get; set; } = "São Paulo";
    public string Uf { get; set; } = "SP";
    public string Telefone { get; set; } = string.Empty;
    public string? Site { get; set; }
    public string? Email { get; set; }
    public string? LogoUrl { get; set; }
    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }

    public ICollection<Proposta> Propostas { get; set; } = new List<Proposta>();
}
