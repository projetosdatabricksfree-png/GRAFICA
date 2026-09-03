using Crm.Domain.Entities;
using Xunit;

namespace Crm.Application.Tests;

public class PropostaCalculoTests
{
    [Fact]
    public void Item_DeveCalcularValorTotalCorretamente_ComArredondamentoMonetario()
    {
        var item = new PropostaItem
        {
            Quantidade = 120,
            ValorUnitario = 2.99m
        };

        item.CalcularTotal();

        Assert.Equal(358.80m, item.ValorTotal);
    }

    [Fact]
    public void Item_DeveCalcularValorTotalItemSemLaminacaoCorretamente()
    {
        var item = new PropostaItem
        {
            Quantidade = 120,
            ValorUnitario = 2.27m
        };

        item.CalcularTotal();

        Assert.Equal(272.40m, item.ValorTotal);
    }

    [Fact]
    public void Item_DeveSuportarPrecoUnitarioComQuatroCasasDecimais_ComoNoModeloReal()
    {
        // No modelo real impresso: 120 unidades a R$ 2.99475/un dá R$ 359.37
        var item = new PropostaItem
        {
            Quantidade = 120,
            ValorUnitario = 2.99475m
        };

        item.CalcularTotal();

        Assert.Equal(359.37m, item.ValorTotal);
    }

    [Fact]
    public void Proposta_DeveSomarValorTotalDeTodosOsItensCorretamente()
    {
        var proposta = new Proposta();

        var item1 = new PropostaItem { Quantidade = 120, ValorUnitario = 2.99m };
        item1.CalcularTotal();

        var item2 = new PropostaItem { Quantidade = 120, ValorUnitario = 2.27m };
        item2.CalcularTotal();

        proposta.Itens.Add(item1);
        proposta.Itens.Add(item2);
        proposta.RecalcularValorTotal();

        Assert.Equal(631.20m, proposta.ValorTotal);
    }
}
