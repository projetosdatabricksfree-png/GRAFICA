namespace Crm.Domain.ValueObjects;

public class EspecificacaoTecnica
{
    public string? Formato { get; set; }
    public string? Papel { get; set; }
    public string? Gramatura { get; set; }
    public string? Cores { get; set; }
    public string? Acabamento { get; set; }
    public string? Observacoes { get; set; }

    public EspecificacaoTecnica() { }

    public EspecificacaoTecnica(string? formato, string? papel, string? cores, string? acabamento)
    {
        Formato = formato;
        Papel = papel;
        Cores = cores;
        Acabamento = acabamento;
    }
}
