using Crm.Application.DTOs;
using FluentValidation;

namespace Crm.Application.Validators;

public class CriarPropostaRequestValidator : AbstractValidator<CriarPropostaRequest>
{
    public CriarPropostaRequestValidator()
    {
        RuleFor(x => x.EmpresaEmissoraId)
            .GreaterThan(0)
            .WithMessage("A empresa emissora é obrigatória.");

        RuleFor(x => x.ClienteId)
            .GreaterThan(0)
            .WithMessage("O cliente é obrigatório.");

        RuleFor(x => x.RepresentanteId)
            .GreaterThan(0)
            .WithMessage("O representante é obrigatório.");

        RuleFor(x => x.ValidadeDias)
            .GreaterThan(0)
            .WithMessage("A validade da proposta deve ser de pelo menos 1 dia.");

        RuleFor(x => x.Itens)
            .NotEmpty()
            .WithMessage("A proposta deve conter ao menos um item.");

        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.Descricao)
                .NotEmpty()
                .WithMessage("A descrição do item é obrigatória.");

            item.RuleFor(i => i.Quantidade)
                .GreaterThan(0)
                .WithMessage("A quantidade do item deve ser maior que zero.");

            item.RuleFor(i => i.ValorUnitario)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O valor unitário não pode ser negativo.");
        });
    }
}

public class CriarClienteRequestValidator : AbstractValidator<CriarClienteRequest>
{
    public CriarClienteRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("O nome do cliente é obrigatório.")
            .MaximumLength(200)
            .WithMessage("O nome do cliente não pode exceder 200 caracteres.");
    }
}
