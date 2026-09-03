using Crm.Domain.Common;

namespace Crm.Domain.Entities;

public class Representante : IAuditableEntity
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty; // Ex: "SUZANA GOMES DE SOUZA"
    public string? Telefone { get; set; } // Ex: "(11) 96800-1262"
    public string? Email { get; set; } // Ex: "suzana@inprima.com.br"
    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }

    public ICollection<Proposta> Propostas { get; set; } = new List<Proposta>();
}
