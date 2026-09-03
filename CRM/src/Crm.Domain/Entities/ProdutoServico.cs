using Crm.Domain.Common;
using Crm.Domain.ValueObjects;

namespace Crm.Domain.Entities;

public class ProdutoServico : IAuditableEntity
{
    public long Id { get; set; }
    public string? Codigo { get; set; } // Ex: "116318"
    public string Grupo { get; set; } = string.Empty; // Ex: "Divisoria com aba", "Cartao de Visita", "Folders"
    public string DescricaoBase { get; set; } = string.Empty; // Ex: "Divisoria com aba 120 ( )"
    public decimal PrecoBase { get; set; }
    public EspecificacaoTecnica EspecificacoesPadrao { get; set; } = new();
    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }
}
