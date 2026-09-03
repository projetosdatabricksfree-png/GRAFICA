using Crm.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Crm.Infrastructure.Pdf;

public class PropostaPdfDocument : IDocument
{
    private readonly PropostaDto _model;

    public PropostaPdfDocument(PropostaDto model)
    {
        _model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Arial).FontColor(Colors.Grey.Darken3));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                // Identificação / Logomarca da Gráfica
                row.RelativeItem().Column(brand =>
                {
                    brand.Item().Text(text =>
                    {
                        text.Span("Ags").Bold().FontSize(22).FontColor(Colors.Blue.Darken3);
                        text.Span("Print").Bold().FontSize(22).FontColor(Colors.Orange.Darken2);
                    });
                    brand.Item().Text("soluções gráficas").FontSize(10).FontColor(Colors.Grey.Darken1);
                    brand.Item().Text(_model.EmpresaEmissoraNome).Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                    brand.Item().Text("www.agsprint.com.br").FontSize(8).FontColor(Colors.Blue.Medium);
                });

                // Dados cadastrais e endereço da Empresa Emissora
                row.RelativeItem().AlignRight().Column(info =>
                {
                    info.Item().Text("SP Laser Cópias Esp Ltda").Bold().FontSize(9);
                    info.Item().Text("CNPJ 86.765.500/0001-30").FontSize(8);
                    info.Item().Text("Rua Castro Alves, 285").FontSize(8);
                    info.Item().Text("01532-001 | São Paulo/SP").FontSize(8);
                    info.Item().Text("(11) 2114-3099").Bold().FontSize(8);
                });
            });

            col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(col =>
        {
            // 1. Bloco de Destinatário / Cliente
            col.Item().Element(ComposeRecipientBlock);

            col.Item().PaddingVertical(8).Text("Apresentamos proposta de orçamento conforme solicitação:").Bold().FontSize(9);

            // 2. Tabela de Itens com especificações técnicas
            col.Item().Element(ComposeItemsTable);

            // 3. Condições Comerciais e Informações Adicionais
            col.Item().PaddingTop(12).Element(ComposeCommercialConditions);

            // 4. Assinatura e De Acordo
            col.Item().PaddingTop(15).Element(ComposeSignatureBlock);
        });
    }

    private void ComposeRecipientBlock(IContainer container)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten5).Padding(8).Row(row =>
        {
            row.RelativeItem().Column(clientCol =>
            {
                clientCol.Item().Text("À").Bold().FontSize(8).FontColor(Colors.Grey.Darken1);
                clientCol.Item().Text(_model.ClienteNome).Bold().FontSize(11).FontColor(Colors.Black);
                if (!string.IsNullOrWhiteSpace(_model.ClienteDocumento))
                    clientCol.Item().Text($"Doc: {_model.ClienteDocumento}").FontSize(8);

                var contato = !string.IsNullOrWhiteSpace(_model.ContatoNome) ? _model.ContatoNome : _model.ClienteNome;
                clientCol.Item().PaddingTop(4).Text($"Prezado(a) Sr.(a) {contato}").Bold().FontSize(8).FontColor(Colors.Grey.Darken3);
            });

            row.RelativeItem().AlignRight().Column(metaCol =>
            {
                var dataFormatada = _model.DataEmissao.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR"));
                metaCol.Item().Text($"São Paulo, {dataFormatada}").FontSize(8).FontColor(Colors.Grey.Darken2);
                metaCol.Item().PaddingTop(2).Text(text =>
                {
                    text.Span("Cód. Proposta: ").Bold();
                    text.Span(_model.Codigo.ToString()).Bold().FontColor(Colors.Blue.Darken3);
                });
                if (!string.IsNullOrWhiteSpace(_model.ClienteCodigo))
                {
                    metaCol.Item().Text(text =>
                    {
                        text.Span("Cód. Cliente: ").Bold();
                        text.Span(_model.ClienteCodigo);
                    });
                }
            });
        });
    }

    private void ComposeItemsTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(55); // ID / Código
                columns.RelativeColumn(3);  // Grupo / Descrição
                columns.ConstantColumn(50); // Qtde
                columns.ConstantColumn(70); // Vlr. Unit.
                columns.ConstantColumn(75); // Vlr. Total
            });

            // Cabeçalho da Tabela
            table.Header(header =>
            {
                header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("ID").Bold().FontSize(8);
                header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Grupo / Descrição").Bold().FontSize(8);
                header.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignRight().Text("Qtde(s)").Bold().FontSize(8);
                header.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignRight().Text("Vlr. Unit.").Bold().FontSize(8);
                header.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignRight().Text("Vlr. Total").Bold().FontSize(8);
            });

            // Linhas dos itens
            foreach (var item in _model.Itens)
            {
                var bg = item.ItemNumero % 2 == 0 ? Colors.Grey.Lighten5 : Colors.White;

                table.Cell().Background(bg).Padding(4).Text(item.CodigoItem ?? item.ItemNumero.ToString()).FontSize(8).Bold();
                
                table.Cell().Background(bg).Padding(4).Column(col =>
                {
                    col.Item().Text(t =>
                    {
                        if (!string.IsNullOrWhiteSpace(item.Grupo))
                            t.Span($"{item.Grupo} - ").Bold();
                        t.Span(item.Descricao).Bold();
                    });

                    // Especificações Técnicas logo abaixo do item (fiel ao modelo real da Inprima)
                    var esp = item.Especificacoes;
                    if (esp != null)
                    {
                        col.Item().PaddingTop(2).Column(specs =>
                        {
                            if (!string.IsNullOrWhiteSpace(esp.Formato))
                                specs.Item().Text($"FORMATO: {esp.Formato}").FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                            if (!string.IsNullOrWhiteSpace(esp.Papel))
                                specs.Item().Text($"PAPEL: {esp.Papel}").FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                            if (!string.IsNullOrWhiteSpace(esp.Cores))
                                specs.Item().Text($"CORES: {esp.Cores}").FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                            if (!string.IsNullOrWhiteSpace(esp.Acabamento))
                                specs.Item().Text($"ACABAMENTO: {esp.Acabamento}").FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                            if (!string.IsNullOrWhiteSpace(esp.Observacoes))
                                specs.Item().Text($"OBS: {esp.Observacoes}").FontSize(7.5f).Italic().FontColor(Colors.Grey.Darken1);
                        });
                    }
                });

                table.Cell().Background(bg).Padding(4).AlignRight().Text(item.Quantidade.ToString("N0")).FontSize(8);
                table.Cell().Background(bg).Padding(4).AlignRight().Text($"R$ {item.ValorUnitario:N2}").FontSize(8);
                table.Cell().Background(bg).Padding(4).AlignRight().Text($"R$ {item.ValorTotal:N2}").Bold().FontSize(8).FontColor(Colors.Blue.Darken3);
            }

            // Linha de Total Geral
            table.Cell().ColumnSpan(4).Background(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("TOTAL DO ORÇAMENTO:").Bold().FontSize(9);
            table.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"R$ {_model.ValorTotal:N2}").Bold().FontSize(10).FontColor(Colors.Green.Darken3);
        });
    }

    private void ComposeCommercialConditions(IContainer container)
    {
        container.Row(row =>
        {
            // Cláusulas padrão
            row.RelativeItem(3).Column(col =>
            {
                var linhas = _model.ClausulasComerciais.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                foreach (var linha in linhas)
                {
                    col.Item().PaddingBottom(1).Text(linha.Trim()).FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                }
            });

            // Informações adicionais
            row.RelativeItem(2).BorderLeft(1).BorderColor(Colors.Grey.Lighten2).PaddingLeft(8).Column(col =>
            {
                col.Item().Text("Informações Adicionais:").Bold().FontSize(8).FontColor(Colors.Grey.Darken3);
                
                col.Item().PaddingTop(2).Text(t =>
                {
                    t.Span("Forma de pagamento: ").Bold().FontSize(7.5f);
                    t.Span(_model.FormaPagamentoNome).FontSize(7.5f);
                });

                col.Item().Text(t =>
                {
                    t.Span("Prazo de entrega: ").Bold().FontSize(7.5f);
                    t.Span(_model.PrazoEntrega).FontSize(7.5f);
                });

                col.Item().Text(t =>
                {
                    t.Span("Validade: ").Bold().FontSize(7.5f);
                    t.Span($"{_model.ValidadeDias} dias").FontSize(7.5f);
                });

                if (!string.IsNullOrWhiteSpace(_model.RepresentanteNome))
                {
                    col.Item().Text(t =>
                    {
                        t.Span("Representante: ").Bold().FontSize(7.5f);
                        t.Span(_model.RepresentanteNome).FontSize(7.5f);
                    });
                }
            });
        });
    }

    private void ComposeSignatureBlock(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                // Assinatura do Representante
                row.RelativeItem().Column(rep =>
                {
                    rep.Item().Text("Atenciosamente,").FontSize(8);
                    rep.Item().PaddingTop(2).Text(_model.RepresentanteNome).Bold().FontSize(9).FontColor(Colors.Black);
                    var foneEmail = $"{_model.RepresentanteTelefone}  {_model.RepresentanteEmail}".Trim();
                    if (!string.IsNullOrWhiteSpace(foneEmail))
                        rep.Item().Text(foneEmail).FontSize(8).FontColor(Colors.Grey.Darken2);
                });

                // Campo De Acordo para assinatura do cliente
                row.RelativeItem().AlignRight().Column(acc =>
                {
                    acc.Item().Text("De acordo: _______/_______/__________").FontSize(8).FontColor(Colors.Grey.Darken3);
                    acc.Item().PaddingTop(2).Text("Cliente - visto e data").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                });
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.BorderTop(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingTop(4).Row(row =>
        {
            row.RelativeItem().Text("AgsPrint • Sistema de Gestão de Propostas Gráficas").FontSize(7).FontColor(Colors.Grey.Darken1);
            row.RelativeItem().AlignRight().Text(x =>
            {
                x.Span("Página ");
                x.CurrentPageNumber();
                x.Span(" de ");
                x.TotalPages();
            });
        });
    }
}
