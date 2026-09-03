using Crm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Crm.Application.Tests;

public class DatabaseIntegrationTests
{
    private readonly string _connectionString = 
        "Host=localhost;Port=5432;Database=crm_db;Username=ags_user;Password=ags_pass";

    [Fact]
    public async Task BancoDeDados_DeveExecutarSeedECarregarPropostaModeloComJsonb()
    {
        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        await using var context = new CrmDbContext(options);

        // Garante aplicação de migrations e seed
        await context.Database.MigrateAsync();
        await CrmDataSeeder.SeedAsync(context, NullLogger.Instance);

        // 1. Valida Empresa Emissora
        var empresa = await context.EmpresasEmissoras.FirstOrDefaultAsync();
        Assert.NotNull(empresa);
        Assert.Equal("AgsPrint Soluções Gráficas", empresa.NomeFantasia);
        Assert.Equal("86.765.500/0001-30", empresa.Cnpj);

        // 2. Valida Cliente CONSUMIDOR e Contato
        var cliente = await context.Clientes.Include(c => c.Contatos).FirstOrDefaultAsync(c => c.CodigoCliente == "3223");
        Assert.NotNull(cliente);
        Assert.Equal("CONSUMIDOR", cliente.Nome);
        Assert.NotEmpty(cliente.Contatos);
        Assert.Equal("Thais", cliente.Contatos.First().Nome);

        // 3. Valida Proposta Modelo Real 62632
        var proposta = await context.Propostas
            .Include(p => p.Itens)
            .Include(p => p.Representante)
            .Include(p => p.Status)
            .FirstOrDefaultAsync(p => p.Codigo == 62632);

        Assert.NotNull(proposta);
        Assert.Equal(2, proposta.Itens.Count);
        Assert.Equal(632.20m, proposta.ValorTotal);
        Assert.Equal("SUZANA GOMES DE SOUZA", proposta.Representante.Nome);

        // 4. Valida persistência e leitura das especificações técnicas em JSONB
        var item1 = proposta.Itens.First(i => i.CodigoItem == "116318");
        Assert.NotNull(item1.Especificacoes);
        Assert.Equal("15,8 cm x 21 cm", item1.Especificacoes.Formato);
        Assert.Equal("Couchê Fosco 300 g/m²", item1.Especificacoes.Papel);
        Assert.Equal("4 x 0", item1.Especificacoes.Cores);
        Assert.Contains("Laminação BOPP", item1.Especificacoes.Acabamento);

        var item2 = proposta.Itens.First(i => i.CodigoItem == "116319");
        Assert.NotNull(item2.Especificacoes);
        Assert.Equal("Corte especial", item2.Especificacoes.Acabamento);
    }
}
