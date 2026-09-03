namespace Crm.Domain.Entities;

public class HistoricoInteracao
{
    public long Id { get; set; }
    public long PropostaId { get; set; }
    public Proposta Proposta { get; set; } = null!;

    public string Tipo { get; set; } = string.Empty; // "Criacao", "AlteracaoStatus", "DownloadPdf", "EmailEnviado", "Ligacao", "Aprovacao"
    public string Descricao { get; set; } = string.Empty;
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public string? Usuario { get; set; }
}
