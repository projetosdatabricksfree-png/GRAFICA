using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Crm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "proposta_codigo_seq",
                startValue: 62632L);

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo_cliente = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    documento = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    telefone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    endereco = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    cep = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_por = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "empresas_emissoras",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    razao_social = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    nome_fantasia = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    unidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cnpj = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    inscricao_estadual = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    endereco = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    cep = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    telefone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    site = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_por = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_empresas_emissoras", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "formas_pagamento",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_formas_pagamento", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "produtos_servicos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    grupo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao_base = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    preco_base = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_por = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EspecificacoesPadrao = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_produtos_servicos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "representantes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    telefone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_por = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_representantes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "status_proposta",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cor_hex = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_status_proposta", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contatos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cliente_id = table.Column<long>(type: "bigint", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cargo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    telefone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    principal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_por = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contatos", x => x.id);
                    table.ForeignKey(
                        name: "fk_contatos_cliente",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "propostas",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "nextval('proposta_codigo_seq')"),
                    versao = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    empresa_emissora_id = table.Column<long>(type: "bigint", nullable: false),
                    cliente_id = table.Column<long>(type: "bigint", nullable: false),
                    contato_id = table.Column<long>(type: "bigint", nullable: true),
                    representante_id = table.Column<long>(type: "bigint", nullable: false),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    forma_pagamento_id = table.Column<int>(type: "integer", nullable: false),
                    data_emissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    validade_dias = table.Column<int>(type: "integer", nullable: false, defaultValue: 10),
                    prazo_entrega = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "A combinar"),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    clausulas_comerciais = table.Column<string>(type: "text", nullable: false),
                    valor_total = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_por = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    atualizado_por = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propostas", x => x.id);
                    table.ForeignKey(
                        name: "fk_propostas_cliente",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_propostas_contato",
                        column: x => x.contato_id,
                        principalTable: "contatos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_propostas_empresa",
                        column: x => x.empresa_emissora_id,
                        principalTable: "empresas_emissoras",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_propostas_forma_pagamento",
                        column: x => x.forma_pagamento_id,
                        principalTable: "formas_pagamento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_propostas_representante",
                        column: x => x.representante_id,
                        principalTable: "representantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_propostas_status",
                        column: x => x.status_id,
                        principalTable: "status_proposta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historico_interacoes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    proposta_id = table.Column<long>(type: "bigint", nullable: false),
                    tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historico_interacoes", x => x.id);
                    table.ForeignKey(
                        name: "fk_historico_proposta",
                        column: x => x.proposta_id,
                        principalTable: "propostas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "proposta_itens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    proposta_id = table.Column<long>(type: "bigint", nullable: false),
                    produto_servico_id = table.Column<long>(type: "bigint", nullable: true),
                    item_numero = table.Column<int>(type: "integer", nullable: false),
                    codigo_item = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    grupo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    quantidade = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    valor_unitario = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    valor_total = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    Especificacoes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proposta_itens", x => x.id);
                    table.ForeignKey(
                        name: "fk_proposta_itens_produto",
                        column: x => x.produto_servico_id,
                        principalTable: "produtos_servicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_proposta_itens_proposta",
                        column: x => x.proposta_id,
                        principalTable: "propostas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "formas_pagamento",
                columns: new[] { "id", "ativo", "nome" },
                values: new object[,]
                {
                    { 1, true, "A Vista" },
                    { 2, true, "28 DDL" },
                    { 3, true, "50% Entrada + 50% na Entrega" },
                    { 4, true, "Cartão de Crédito / Débito" },
                    { 5, true, "A Combinar" }
                });

            migrationBuilder.InsertData(
                table: "status_proposta",
                columns: new[] { "id", "cor_hex", "descricao", "nome" },
                values: new object[,]
                {
                    { 1, "#757575", "Proposta em elaboração", "Rascunho" },
                    { 2, "#1E88E5", "Enviada ao cliente", "Enviada" },
                    { 3, "#43A047", "Proposta aprovada pelo cliente", "Aprovada" },
                    { 4, "#E53935", "Proposta recusada pelo cliente", "Recusada" },
                    { 5, "#8E24AA", "Proposta cancelada internamente", "Cancelada" },
                    { 6, "#FB8C00", "Prazo de validade expirado", "Expirada" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_contatos_cliente_id",
                table: "contatos",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_historico_interacoes_proposta_id",
                table: "historico_interacoes",
                column: "proposta_id");

            migrationBuilder.CreateIndex(
                name: "IX_proposta_itens_produto_servico_id",
                table: "proposta_itens",
                column: "produto_servico_id");

            migrationBuilder.CreateIndex(
                name: "IX_proposta_itens_proposta_id",
                table: "proposta_itens",
                column: "proposta_id");

            migrationBuilder.CreateIndex(
                name: "IX_propostas_cliente_id",
                table: "propostas",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_propostas_contato_id",
                table: "propostas",
                column: "contato_id");

            migrationBuilder.CreateIndex(
                name: "IX_propostas_empresa_emissora_id",
                table: "propostas",
                column: "empresa_emissora_id");

            migrationBuilder.CreateIndex(
                name: "IX_propostas_forma_pagamento_id",
                table: "propostas",
                column: "forma_pagamento_id");

            migrationBuilder.CreateIndex(
                name: "IX_propostas_representante_id",
                table: "propostas",
                column: "representante_id");

            migrationBuilder.CreateIndex(
                name: "IX_propostas_status_id",
                table: "propostas",
                column: "status_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "historico_interacoes");

            migrationBuilder.DropTable(
                name: "proposta_itens");

            migrationBuilder.DropTable(
                name: "produtos_servicos");

            migrationBuilder.DropTable(
                name: "propostas");

            migrationBuilder.DropTable(
                name: "contatos");

            migrationBuilder.DropTable(
                name: "empresas_emissoras");

            migrationBuilder.DropTable(
                name: "formas_pagamento");

            migrationBuilder.DropTable(
                name: "representantes");

            migrationBuilder.DropTable(
                name: "status_proposta");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropSequence(
                name: "proposta_codigo_seq");
        }
    }
}
