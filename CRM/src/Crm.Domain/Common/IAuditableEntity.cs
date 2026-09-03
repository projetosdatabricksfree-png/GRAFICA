namespace Crm.Domain.Common;

public interface IAuditableEntity
{
    DateTime CriadoEm { get; set; }
    DateTime? AtualizadoEm { get; set; }
    string? CriadoPor { get; set; }
    string? AtualizadoPor { get; set; }
}
