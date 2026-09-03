using Crm.Application.DTOs;
using Crm.Application.Validators;
using Xunit;

namespace Crm.Application.Tests;

public class PropostaValidatorTests
{
    private readonly CriarPropostaRequestValidator _validator = new();

    [Fact]
    public void Validador_DeveRejeitarPropostaSemItens()
    {
        var request = new CriarPropostaRequest
        {
            EmpresaEmissoraId = 1,
            ClienteId = 1,
            RepresentanteId = 1,
            ValidadeDias = 10,
            Itens = new List<ItemPropostaRequest>()
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Itens");
    }

    [Fact]
    public void Validador_DeveAprovarPropostaValida()
    {
        var request = new CriarPropostaRequest
        {
            EmpresaEmissoraId = 1,
            ClienteId = 1,
            RepresentanteId = 1,
            ValidadeDias = 10,
            Itens = new List<ItemPropostaRequest>
            {
                new()
                {
                    Descricao = "Divisoria com aba 120",
                    Quantidade = 120,
                    ValorUnitario = 2.99m
                }
            }
        };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
