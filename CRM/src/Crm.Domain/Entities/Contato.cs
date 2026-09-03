using Crm.Domain.Common;

namespace Crm.Domain.Entities;

public class Contato : IAuditableEntity
{
    public long Id { get; set; }
    public long ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public string Nome { get; set; } = string.Empty; // Ex: "Thais"
    public string? Cargo { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public bool Principal { get; set; } = false;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }
}
