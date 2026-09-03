using Crm.Domain.ValueObjects;

namespace Crm.Domain.Entities;

public class PropostaItem
{
    public long Id { get; set; }
    public long PropostaId { get; set; }
    public Proposta Proposta { get; set; } = null!;

    public long? ProdutoServicoId { get; set; }
    public ProdutoServico? ProdutoServico { get; set; }

    public int ItemNumero { get; set; } // 1, 2, ...
    public string? CodigoItem { get; set; } // Ex: "116318", "116319"
    public string Grupo { get; set; } = string.Empty; // Ex: "Divisoria com aba"
    public string Descricao { get; set; } = string.Empty; // Ex: "Divisoria com aba 120 ( )"

    public decimal Quantidade { get; set; } // Ex: 120
    public decimal ValorUnitario { get; set; } // Ex: 2.99
    public decimal ValorTotal { get; set; } // Ex: 359.37 (calculado na aplicação)

    // Especificações técnicas individuais deste item da proposta (sobrescreve o produto padrão se necessário)
    public EspecificacaoTecnica Especificacoes { get; set; } = new();

    public void CalcularTotal()
    {
        ValorTotal = Math.Round(Quantidade * ValorUnitario, 2, MidpointRounding.AwayFromZero);
    }
}
