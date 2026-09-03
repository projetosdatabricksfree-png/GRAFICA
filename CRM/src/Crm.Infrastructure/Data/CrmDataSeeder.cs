using Crm.Domain.Entities;
using Crm.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Crm.Infrastructure.Data;

public static class CrmDataSeeder
{
    public static async Task SeedAsync(CrmDbContext context, ILogger logger)
    {
        try
        {
            // 1. Empresa Emissora (AgsPrint / SP Laser Cópias)
            if (!await context.EmpresasEmissoras.AnyAsync())
            {
                var empresa = new EmpresaEmissora
                {
                    NomeFantasia = "AgsPrint Soluções Gráficas",
                    RazaoSocial = "SP Laser Cópias Esp Ltda",
                    Unidade = "Unidade Paulista",
                    Cnpj = "86.765.500/0001-30",
                    Endereco = "Rua Castro Alves, 285",
                    Cep = "01532-001",
                    Cidade = "São Paulo",
                    Uf = "SP",
                    Telefone = "(11) 2114-3099",
                    Site = "www.agsprint.com.br",
                    Email = "contato@agsprint.com.br",
                    Ativo = true,
                    CriadoPor = "Seed"
                };
                await context.EmpresasEmissoras.AddAsync(empresa);
                await context.SaveChangesAsync();
                logger.LogInformation("Empresa Emissora AgsPrint criada.");
            }

            // 2. Representante (Suzana Gomes de Souza)
            if (!await context.Representantes.AnyAsync())
            {
                var rep = new Representante
                {
                    Nome = "SUZANA GOMES DE SOUZA",
                    Telefone = "(11) 96800-1262",
                    Email = "suzana@agsprint.com.br",
                    Ativo = true,
                    CriadoPor = "Seed"
                };
                await context.Representantes.AddAsync(rep);
                await context.SaveChangesAsync();
                logger.LogInformation("Representante Suzana Gomes de Souza criada.");
            }

            // 3. Cliente (CONSUMIDOR - Código 3223)
            if (!await context.Clientes.AnyAsync())
            {
                var cliente = new Cliente
                {
                    CodigoCliente = "3223",
                    Nome = "CONSUMIDOR",
                    Documento = "000.000.000-01",
                    Telefone = "(11) 99999-0000",
                    Email = "thais@cliente.com.br",
                    Cidade = "São Paulo",
                    Uf = "SP",
                    Ativo = true,
                    CriadoPor = "Seed"
                };
                cliente.Contatos.Add(new Contato
                {
                    Nome = "Thais",
                    Cargo = "Compradora",
                    Telefone = "(11) 99999-0000",
                    Email = "thais@cliente.com.br",
                    Principal = true,
                    CriadoPor = "Seed"
                });
                await context.Clientes.AddAsync(cliente);
                await context.SaveChangesAsync();
                logger.LogInformation("Cliente CONSUMIDOR (3223) e contato Thais criados.");
            }

            // 4. Catálogo de Produtos
            if (!await context.ProdutosServicos.AnyAsync())
            {
                var produtos = new List<ProdutoServico>
                {
                    new ProdutoServico
                    {
                        Codigo = "116318",
                        Grupo = "Divisoria com aba",
                        DescricaoBase = "Divisoria com aba 120 ( ) com Laminação",
                        PrecoBase = 2.99m,
                        EspecificacoesPadrao = new EspecificacaoTecnica
                        {
                            Formato = "15,8 cm x 21 cm",
                            Papel = "Couchê Fosco 300 g/m²",
                            Cores = "4 x 0",
                            Acabamento = "Corte especial / Laminação BOPP Frente/Verso Fosco"
                        },
                        Ativo = true,
                        CriadoPor = "Seed"
                    },
                    new ProdutoServico
                    {
                        Codigo = "116319",
                        Grupo = "Divisoria com aba",
                        DescricaoBase = "Divisoria com aba 120 ( ) sem Laminação",
                        PrecoBase = 2.27m,
                        EspecificacoesPadrao = new EspecificacaoTecnica
                        {
                            Formato = "15,8 cm x 21 cm",
                            Papel = "Couchê Fosco 300 g/m²",
                            Cores = "4 x 0",
                            Acabamento = "Corte especial"
                        },
                        Ativo = true,
                        CriadoPor = "Seed"
                    },
                    new ProdutoServico
                    {
                        Codigo = "110001",
                        Grupo = "Cartão de Visita",
                        DescricaoBase = "Cartão de Visita 9x5 Couchê 300g",
                        PrecoBase = 0.35m,
                        EspecificacoesPadrao = new EspecificacaoTecnica
                        {
                            Formato = "9 cm x 5 cm",
                            Papel = "Couchê Fosco 300 g/m²",
                            Cores = "4 x 4",
                            Acabamento = "Laminação Fosca Frente e Verso + Verniz Localizado"
                        },
                        Ativo = true,
                        CriadoPor = "Seed"
                    }
                };

                await context.ProdutosServicos.AddRangeAsync(produtos);
                await context.SaveChangesAsync();
                logger.LogInformation("Produtos e serviços do catálogo criados.");
            }

            // 5. Proposta Modelo Real (Proposta #62632)
            if (!await context.Propostas.AnyAsync())
            {
                var empresa = await context.EmpresasEmissoras.FirstAsync();
                var cliente = await context.Clientes.Include(c => c.Contatos).FirstAsync();
                var contato = cliente.Contatos.FirstOrDefault();
                var rep = await context.Representantes.FirstAsync();

                var propostaModelo = new Proposta
                {
                    Codigo = 62632,
                    Versao = 1,
                    EmpresaEmissoraId = empresa.Id,
                    ClienteId = cliente.Id,
                    ContatoId = contato?.Id,
                    RepresentanteId = rep.Id,
                    StatusId = StatusProposta.Enviada,
                    FormaPagamentoId = FormaPagamento.AVista,
                    DataEmissao = new DateTime(2026, 8, 17, 14, 30, 0, DateTimeKind.Utc),
                    ValidadeDias = 10,
                    PrazoEntrega = "A combinar",
                    Observacoes = "Proposta emitida conforme especificações técnicas solicitadas.",
                    CriadoPor = "Seed"
                };

                // Item 1
                var item1 = new PropostaItem
                {
                    ItemNumero = 1,
                    CodigoItem = "116318",
                    Grupo = "Divisoria com aba",
                    Descricao = "Divisoria com aba 120 ( )",
                    Quantidade = 120,
                    ValorUnitario = 2.99m,
                    ValorTotal = 359.37m,
                    Especificacoes = new EspecificacaoTecnica
                    {
                        Formato = "15,8 cm x 21 cm",
                        Papel = "Couchê Fosco 300 g/m²",
                        Cores = "4 x 0",
                        Acabamento = "Corte especial / Laminação BOPP Frente/Verso Fosco"
                    }
                };

                // Item 2
                var item2 = new PropostaItem
                {
                    ItemNumero = 2,
                    CodigoItem = "116319",
                    Grupo = "Divisoria com aba",
                    Descricao = "Divisoria com aba 120 ( )",
                    Quantidade = 120,
                    ValorUnitario = 2.27m,
                    ValorTotal = 272.83m,
                    Especificacoes = new EspecificacaoTecnica
                    {
                        Formato = "15,8 cm x 21 cm",
                        Papel = "Couchê Fosco 300 g/m²",
                        Cores = "4 x 0",
                        Acabamento = "Corte especial"
                    }
                };

                propostaModelo.Itens.Add(item1);
                propostaModelo.Itens.Add(item2);
                propostaModelo.RecalcularValorTotal();

                propostaModelo.Historico.Add(new HistoricoInteracao
                {
                    Tipo = "Criacao",
                    Descricao = "Orçamento gerado e enviado para a cliente Thais com base no modelo real Inprima.",
                    Data = new DateTime(2026, 8, 17, 14, 30, 0, DateTimeKind.Utc),
                    Usuario = rep.Nome
                });

                await context.Propostas.AddAsync(propostaModelo);
                await context.SaveChangesAsync();
                logger.LogInformation("Proposta modelo #62632 cadastrada com sucesso com total de R$ {Total}", propostaModelo.ValorTotal);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao popular dados iniciais do CRM.");
            throw;
        }
    }
}
