using Crm.Application.DTOs;
using Crm.Domain.ValueObjects;
using Crm.Infrastructure.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Xunit;

namespace Crm.Application.Tests;

public class QuestPdfGenerationTests
{
    public QuestPdfGenerationTests()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [Fact]
    public void Documento_DeveGerarPdfValido_ParaModeloRealInprima()
    {
        var proposta = new PropostaDto
        {
            Id = 1,
            Codigo = 62632,
            Versao = 1,
            EmpresaEmissoraNome = "AgsPrint Soluções Gráficas",
            ClienteNome = "CONSUMIDOR",
            ClienteDocumento = "000.000.000-01",
            ClienteCodigo = "3223",
            ContatoNome = "Thais",
            RepresentanteNome = "SUZANA GOMES DE SOUZA",
            RepresentanteTelefone = "(11) 96800-1262",
            RepresentanteEmail = "suzana@agsprint.com.br",
            StatusNome = "Enviada",
            FormaPagamentoNome = "A Vista",
            DataEmissao = new DateTime(2026, 8, 17),
            ValidadeDias = 10,
            PrazoEntrega = "A combinar",
            ClausulasComerciais = "• Sujeito a alteração de valores após a análise do material.\n• Prazo de execução contado a partir da aprovação do material.\n• Não mantemos os arquivos em nosso sistema após a entrega do material. Caso deseje, peça sua cópia.",
            ValorTotal = 632.20m,
            Itens = new List<PropostaItemDto>
            {
                new()
                {
                    ItemNumero = 1,
                    CodigoItem = "116318",
                    Grupo = "Divisoria com aba",
                    Descricao = "Divisoria com aba 120 ( )",
                    Quantidade = 120,
                    ValorUnitario = 2.99m,
                    ValorTotal = 359.37m,
                    Especificacoes = new EspecificacaoTecnica("15,8 cm x 21 cm", "Couchê Fosco 300 g/m²", "4 x 0", "Corte especial / Laminação BOPP Frente/Verso Fosco")
                },
                new()
                {
                    ItemNumero = 2,
                    CodigoItem = "116319",
                    Grupo = "Divisoria com aba",
                    Descricao = "Divisoria com aba 120 ( )",
                    Quantidade = 120,
                    ValorUnitario = 2.27m,
                    ValorTotal = 272.83m,
                    Especificacoes = new EspecificacaoTecnica("15,8 cm x 21 cm", "Couchê Fosco 300 g/m²", "4 x 0", "Corte especial")
                }
            }
        };

        var doc = new PropostaPdfDocument(proposta);
        var bytes = doc.GeneratePdf();

        Assert.NotNull(bytes);
        Assert.NotEmpty(bytes);
        var header = System.Text.Encoding.ASCII.GetString(bytes.Take(5).ToArray());
        Assert.Equal("%PDF-", header);
        Assert.True(bytes.Length > 5000);

        // Salva o PDF gerado no diretório do CRM para verificação do usuário
        var outputPath = @"c:\Users\Suzana\Documents\GRAFICA\CRM\proposta_gerada_62632.pdf";
        File.WriteAllBytes(outputPath, bytes);
        Assert.True(File.Exists(outputPath));
    }
}
