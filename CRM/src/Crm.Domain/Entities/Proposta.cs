using Crm.Domain.Common;

namespace Crm.Domain.Entities;

public class Proposta : IAuditableEntity
{
    public long Id { get; set; }
    public long Codigo { get; set; } // Sequence sequencial de negócio (ex: 62632)
    public int Versao { get; set; } = 1; // Controle de versionamento

    public long EmpresaEmissoraId { get; set; }
    public EmpresaEmissora EmpresaEmissora { get; set; } = null!;

    public long ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public long? ContatoId { get; set; }
    public Contato? Contato { get; set; }

    public long RepresentanteId { get; set; }
    public Representante Representante { get; set; } = null!;

    public int StatusId { get; set; } = StatusProposta.Rascunho;
    public StatusProposta Status { get; set; } = null!;

    public int FormaPagamentoId { get; set; } = FormaPagamento.AVista;
    public FormaPagamento FormaPagamento { get; set; } = null!;

    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
    public int ValidadeDias { get; set; } = 10;
    public string PrazoEntrega { get; set; } = "A combinar";
    public string? Observacoes { get; set; }

    // Cláusulas comerciais padrão impressas no PDF
    public string ClausulasComerciais { get; set; } = 
        "• Sujeito a alteração de valores após a análise do material.\n" +
        "• Prazo de execução contado a partir da aprovação do material.\n" +
        "• Não mantemos os arquivos em nosso sistema após a entrega do material. Caso deseje, peça sua cópia.";

    public decimal ValorTotal { get; set; } // Calculado e persistido pela camada de aplicação

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }

    public ICollection<PropostaItem> Itens { get; set; } = new List<PropostaItem>();
    public ICollection<HistoricoInteracao> Historico { get; set; } = new List<HistoricoInteracao>();

    public void RecalcularValorTotal()
    {
        ValorTotal = Itens.Sum(i => i.ValorTotal);
    }
}
