using Crm.Application.DTOs;
using Crm.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Crm.Infrastructure.Pdf;

public class PdfService : IPdfService
{
    private readonly IPropostaService _propostaService;

    static PdfService()
    {
        // Define licença comunitária do QuestPDF (regra de conformidade do projeto)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public PdfService(IPropostaService propostaService)
    {
        _propostaService = propostaService;
    }

    public async Task<byte[]> GerarPropostaPdfAsync(long propostaId, CancellationToken ct = default)
    {
        var proposta = await _propostaService.ObterPorIdAsync(propostaId, ct);
        if (proposta == null)
            throw new KeyNotFoundException($"Proposta com ID {propostaId} não encontrada.");

        return GerarPropostaPdf(proposta);
    }

    public byte[] GerarPropostaPdf(PropostaDto proposta)
    {
        var document = new PropostaPdfDocument(proposta);
        return document.GeneratePdf();
    }
}
