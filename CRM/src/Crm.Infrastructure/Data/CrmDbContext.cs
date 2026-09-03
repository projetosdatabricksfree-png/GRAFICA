using Crm.Domain.Entities;
using Crm.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Crm.Infrastructure.Data;

public class CrmDbContext : DbContext
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options)
    {
    }

    public DbSet<EmpresaEmissora> EmpresasEmissoras => Set<EmpresaEmissora>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Contato> Contatos => Set<Contato>();
    public DbSet<Representante> Representantes => Set<Representante>();
    public DbSet<ProdutoServico> ProdutosServicos => Set<ProdutoServico>();
    public DbSet<StatusProposta> StatusPropostas => Set<StatusProposta>();
    public DbSet<FormaPagamento> FormasPagamento => Set<FormaPagamento>();
    public DbSet<Proposta> Propostas => Set<Proposta>();
    public DbSet<PropostaItem> PropostaItens => Set<PropostaItem>();
    public DbSet<HistoricoInteracao> HistoricosInteracoes => Set<HistoricoInteracao>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditoriaSaveChangesInterceptor());
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Sequence própria para código de proposta (evita colisões concorrentes - regra do domínio)
        modelBuilder.HasSequence<long>("proposta_codigo_seq")
            .StartsAt(62632)
            .IncrementsBy(1);

        // Configuração snake_case e tabelas
        modelBuilder.Entity<EmpresaEmissora>(b =>
        {
            b.ToTable("empresas_emissoras");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.RazaoSocial).HasColumnName("razao_social").HasMaxLength(200).IsRequired();
            b.Property(x => x.NomeFantasia).HasColumnName("nome_fantasia").HasMaxLength(200).IsRequired();
            b.Property(x => x.Unidade).HasColumnName("unidade").HasMaxLength(100);
            b.Property(x => x.Cnpj).HasColumnName("cnpj").HasMaxLength(20).IsRequired();
            b.Property(x => x.InscricaoEstadual).HasColumnName("inscricao_estadual").HasMaxLength(30);
            b.Property(x => x.Endereco).HasColumnName("endereco").HasMaxLength(250).IsRequired();
            b.Property(x => x.Cep).HasColumnName("cep").HasMaxLength(20).IsRequired();
            b.Property(x => x.Cidade).HasColumnName("cidade").HasMaxLength(100).IsRequired();
            b.Property(x => x.Uf).HasColumnName("uf").HasMaxLength(2).IsRequired();
            b.Property(x => x.Telefone).HasColumnName("telefone").HasMaxLength(50).IsRequired();
            b.Property(x => x.Site).HasColumnName("site").HasMaxLength(150);
            b.Property(x => x.Email).HasColumnName("email").HasMaxLength(150);
            b.Property(x => x.LogoUrl).HasColumnName("logo_url").HasMaxLength(500);
            b.Property(x => x.Ativo).HasColumnName("ativo").HasDefaultValue(true);
            b.Property(x => x.CriadoEm).HasColumnName("criado_em");
            b.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
            b.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(100);
            b.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(100);
        });

        modelBuilder.Entity<Cliente>(b =>
        {
            b.ToTable("clientes");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.CodigoCliente).HasColumnName("codigo_cliente").HasMaxLength(50);
            b.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(200).IsRequired();
            b.Property(x => x.Documento).HasColumnName("documento").HasMaxLength(30);
            b.Property(x => x.Telefone).HasColumnName("telefone").HasMaxLength(50);
            b.Property(x => x.Email).HasColumnName("email").HasMaxLength(150);
            b.Property(x => x.Endereco).HasColumnName("endereco").HasMaxLength(250);
            b.Property(x => x.Cidade).HasColumnName("cidade").HasMaxLength(100);
            b.Property(x => x.Uf).HasColumnName("uf").HasMaxLength(2);
            b.Property(x => x.Cep).HasColumnName("cep").HasMaxLength(20);
            b.Property(x => x.Ativo).HasColumnName("ativo").HasDefaultValue(true);
            b.Property(x => x.CriadoEm).HasColumnName("criado_em");
            b.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
            b.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(100);
            b.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(100);

            b.HasMany(x => x.Contatos)
                .WithOne(x => x.Cliente)
                .HasForeignKey(x => x.ClienteId)
                .HasConstraintName("fk_contatos_cliente")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Contato>(b =>
        {
            b.ToTable("contatos");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.ClienteId).HasColumnName("cliente_id");
            b.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
            b.Property(x => x.Cargo).HasColumnName("cargo").HasMaxLength(100);
            b.Property(x => x.Telefone).HasColumnName("telefone").HasMaxLength(50);
            b.Property(x => x.Email).HasColumnName("email").HasMaxLength(150);
            b.Property(x => x.Principal).HasColumnName("principal").HasDefaultValue(false);
            b.Property(x => x.CriadoEm).HasColumnName("criado_em");
            b.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
            b.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(100);
            b.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(100);
        });

        modelBuilder.Entity<Representante>(b =>
        {
            b.ToTable("representantes");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
            b.Property(x => x.Telefone).HasColumnName("telefone").HasMaxLength(50);
            b.Property(x => x.Email).HasColumnName("email").HasMaxLength(150);
            b.Property(x => x.Ativo).HasColumnName("ativo").HasDefaultValue(true);
            b.Property(x => x.CriadoEm).HasColumnName("criado_em");
            b.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
            b.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(100);
            b.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(100);
        });

        modelBuilder.Entity<ProdutoServico>(b =>
        {
            b.ToTable("produtos_servicos");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(50);
            b.Property(x => x.Grupo).HasColumnName("grupo").HasMaxLength(100).IsRequired();
            b.Property(x => x.DescricaoBase).HasColumnName("descricao_base").HasMaxLength(300).IsRequired();
            b.Property(x => x.PrecoBase).HasColumnName("preco_base").HasPrecision(12, 4);
            b.Property(x => x.Ativo).HasColumnName("ativo").HasDefaultValue(true);
            b.Property(x => x.CriadoEm).HasColumnName("criado_em");
            b.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
            b.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(100);
            b.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(100);

            // Mapeamento JSONB nativo no PostgreSQL
            b.OwnsOne(x => x.EspecificacoesPadrao, j => j.ToJson());
        });

        modelBuilder.Entity<StatusProposta>(b =>
        {
            b.ToTable("status_proposta");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(50).IsRequired();
            b.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(200);
            b.Property(x => x.CorHex).HasColumnName("cor_hex").HasMaxLength(10).IsRequired();
        });

        modelBuilder.Entity<FormaPagamento>(b =>
        {
            b.ToTable("formas_pagamento");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
            b.Property(x => x.Ativo).HasColumnName("ativo").HasDefaultValue(true);
        });

        modelBuilder.Entity<Proposta>(b =>
        {
            b.ToTable("propostas");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Codigo).HasColumnName("codigo").HasDefaultValueSql("nextval('proposta_codigo_seq')");
            b.Property(x => x.Versao).HasColumnName("versao").HasDefaultValue(1);
            b.Property(x => x.EmpresaEmissoraId).HasColumnName("empresa_emissora_id");
            b.Property(x => x.ClienteId).HasColumnName("cliente_id");
            b.Property(x => x.ContatoId).HasColumnName("contato_id");
            b.Property(x => x.RepresentanteId).HasColumnName("representante_id");
            b.Property(x => x.StatusId).HasColumnName("status_id");
            b.Property(x => x.FormaPagamentoId).HasColumnName("forma_pagamento_id");
            b.Property(x => x.DataEmissao).HasColumnName("data_emissao");
            b.Property(x => x.ValidadeDias).HasColumnName("validade_dias").HasDefaultValue(10);
            b.Property(x => x.PrazoEntrega).HasColumnName("prazo_entrega").HasMaxLength(100).HasDefaultValue("A combinar");
            b.Property(x => x.Observacoes).HasColumnName("observacoes");
            b.Property(x => x.ClausulasComerciais).HasColumnName("clausulas_comerciais");
            b.Property(x => x.ValorTotal).HasColumnName("valor_total").HasPrecision(12, 2);
            b.Property(x => x.CriadoEm).HasColumnName("criado_em");
            b.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
            b.Property(x => x.CriadoPor).HasColumnName("criado_por").HasMaxLength(100);
            b.Property(x => x.AtualizadoPor).HasColumnName("atualizado_por").HasMaxLength(100);

            b.HasOne(x => x.EmpresaEmissora)
                .WithMany(x => x.Propostas)
                .HasForeignKey(x => x.EmpresaEmissoraId)
                .HasConstraintName("fk_propostas_empresa")
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Cliente)
                .WithMany(x => x.Propostas)
                .HasForeignKey(x => x.ClienteId)
                .HasConstraintName("fk_propostas_cliente")
                .OnDelete(DeleteBehavior.Restrict); // Sem delete cascade - preserva historico!

            b.HasOne(x => x.Contato)
                .WithMany()
                .HasForeignKey(x => x.ContatoId)
                .HasConstraintName("fk_propostas_contato")
                .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.Representante)
                .WithMany(x => x.Propostas)
                .HasForeignKey(x => x.RepresentanteId)
                .HasConstraintName("fk_propostas_representante")
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Status)
                .WithMany()
                .HasForeignKey(x => x.StatusId)
                .HasConstraintName("fk_propostas_status")
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.FormaPagamento)
                .WithMany()
                .HasForeignKey(x => x.FormaPagamentoId)
                .HasConstraintName("fk_propostas_forma_pagamento")
                .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(x => x.Itens)
                .WithOne(x => x.Proposta)
                .HasForeignKey(x => x.PropostaId)
                .HasConstraintName("fk_proposta_itens_proposta")
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Historico)
                .WithOne(x => x.Proposta)
                .HasForeignKey(x => x.PropostaId)
                .HasConstraintName("fk_historico_proposta")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PropostaItem>(b =>
        {
            b.ToTable("proposta_itens");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.PropostaId).HasColumnName("proposta_id");
            b.Property(x => x.ProdutoServicoId).HasColumnName("produto_servico_id");
            b.Property(x => x.ItemNumero).HasColumnName("item_numero");
            b.Property(x => x.CodigoItem).HasColumnName("codigo_item").HasMaxLength(50);
            b.Property(x => x.Grupo).HasColumnName("grupo").HasMaxLength(100).IsRequired();
            b.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(300).IsRequired();
            b.Property(x => x.Quantidade).HasColumnName("quantidade").HasPrecision(10, 2);
            b.Property(x => x.ValorUnitario).HasColumnName("valor_unitario").HasPrecision(12, 4);
            b.Property(x => x.ValorTotal).HasColumnName("valor_total").HasPrecision(12, 2);

            b.HasOne(x => x.ProdutoServico)
                .WithMany()
                .HasForeignKey(x => x.ProdutoServicoId)
                .HasConstraintName("fk_proposta_itens_produto")
                .OnDelete(DeleteBehavior.SetNull);

            // Mapeamento JSONB nativo no PostgreSQL
            b.OwnsOne(x => x.Especificacoes, j => j.ToJson());
        });

        modelBuilder.Entity<HistoricoInteracao>(b =>
        {
            b.ToTable("historico_interacoes");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.PropostaId).HasColumnName("proposta_id");
            b.Property(x => x.Tipo).HasColumnName("tipo").HasMaxLength(50).IsRequired();
            b.Property(x => x.Descricao).HasColumnName("descricao").HasMaxLength(500).IsRequired();
            b.Property(x => x.Data).HasColumnName("data");
            b.Property(x => x.Usuario).HasColumnName("usuario").HasMaxLength(100);
        });

        // Seed das tabelas de domínio
        modelBuilder.Entity<StatusProposta>().HasData(
            new StatusProposta { Id = 1, Nome = "Rascunho", Descricao = "Proposta em elaboração", CorHex = "#757575" },
            new StatusProposta { Id = 2, Nome = "Enviada", Descricao = "Enviada ao cliente", CorHex = "#1E88E5" },
            new StatusProposta { Id = 3, Nome = "Aprovada", Descricao = "Proposta aprovada pelo cliente", CorHex = "#43A047" },
            new StatusProposta { Id = 4, Nome = "Recusada", Descricao = "Proposta recusada pelo cliente", CorHex = "#E53935" },
            new StatusProposta { Id = 5, Nome = "Cancelada", Descricao = "Proposta cancelada internamente", CorHex = "#8E24AA" },
            new StatusProposta { Id = 6, Nome = "Expirada", Descricao = "Prazo de validade expirado", CorHex = "#FB8C00" }
        );

        modelBuilder.Entity<FormaPagamento>().HasData(
            new FormaPagamento { Id = 1, Nome = "A Vista", Ativo = true },
            new FormaPagamento { Id = 2, Nome = "28 DDL", Ativo = true },
            new FormaPagamento { Id = 3, Nome = "50% Entrada + 50% na Entrega", Ativo = true },
            new FormaPagamento { Id = 4, Nome = "Cartão de Crédito / Débito", Ativo = true },
            new FormaPagamento { Id = 5, Nome = "A Combinar", Ativo = true }
        );
    }
}
