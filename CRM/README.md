# AgsPrint — Sistema de Gestão de Propostas e Orçamentos Gráficos

Sistema corporativo de emissão, versionamento e gestão de propostas comerciais gráficas, desenvolvido em **.NET 10 LTS** com **Clean Architecture**, banco de dados **PostgreSQL 17** (com colunas nativas `JSONB` e `sequences`), frontend interativo **Blazor Server** com **MudBlazor 9**, motor de geração de PDF com **QuestPDF**, e ambiente de desenvolvimento e produção orquestrado com **Docker Compose**.

O sistema foi modelado para atender às necessidades reais do setor gráfico, reproduzindo integralmente o modelo comercial e o documento de orçamento da **Inprima Soluções Gráficas / SP Laser Cópias Esp Ltda** (Proposta #62632).

---

## 🚀 Arquitetura da Solução

```
CRM/
├── src/
│   ├── Crm.Domain/             # Camada de Domínio: Entidades, Value Objects e Auditoria
│   ├── Crm.Application/        # Camada de Aplicação: DTOs, Casos de Uso e FluentValidation
│   ├── Crm.Infrastructure/     # Camada de Infraestrutura: EF Core 10, Npgsql (JSONB), QuestPDF
│   ├── Crm.Api/                # Backend REST API com OpenAPI/Swagger
│   └── Crm.Web/                # Frontend Blazor Server com tema corporativo MudBlazor
├── tests/
│   └── Crm.Application.Tests/  # Testes unitários monetários, validações e integração com PostgreSQL
├── docker-compose.yml          # Orquestração completa de containers (db, migrator, api, web, pgadmin)
├── .env.example                # Modelo de variáveis de ambiente
└── Crm.slnx                    # Solução .NET 10 (formato moderno XML)
```

---

## 📦 Tecnologias Utilizadas

- **.NET 10 SDK** (C# 14 / net10.0)
- **Entity Framework Core 10** + **Npgsql 10**
- **PostgreSQL 17** com tipos `jsonb` e sequenciadores `sequence`
- **QuestPDF 2026** (Licença Comunitária)
- **MudBlazor 9.9.0** (Material Design C# com tema escuro/claro e paleta Inprima)
- **FluentValidation 12**
- **Docker & Docker Compose** (Build multi-stage otimizado)
- **xUnit** para testes automatizados

---

## ⚡ Como Rodar o Projeto

### 1. Pré-requisitos
- .NET 10 SDK
- Docker Desktop (com PostgreSQL rodando ou via Docker Compose)

### 2. Execução Local com .NET
```powershell
# Restaurar dependências e compilar a solução:
dotnet build Crm.slnx

# Executar a Interface Web Blazor:
dotnet run --project src/Crm.Web/Crm.Web.csproj --urls=http://localhost:5000

# Executar a API REST:
dotnet run --project src/Crm.Api/Crm.Api.csproj --urls=http://localhost:5100
```
- **Acesso Web**: [http://localhost:5000](http://localhost:5000)
- **Documentação da API**: [http://localhost:5100/openapi/v1.json](http://localhost:5100/openapi/v1.json)

### 3. Execução com Docker Compose
```bash
docker compose up -d --build
```
- Os containers do PostgreSQL, migrator automático, API e Web subirão de forma ordenada com healthchecks.

---

## 🧪 Execução dos Testes Automatizados
```powershell
dotnet test Crm.slnx
```
Todos os 9 testes cobrem:
- Cálculos de itens e regras de arredondamento comercial (2 e 4 casas decimais)
- Validações de integridade de propostas e clientes via FluentValidation
- Geração física de documentos PDF (`%PDF-`)
- Teste de integração real com persistência e leitura de dados `JSONB` no PostgreSQL

---

## 📄 Modelo de Orçamento em PDF

O sistema gera o documento idêntico ao modelo físico da proposta Inprima #62632:
- Cabeçalho gráfico com CNPJ e unidade
- Bloco do cliente tomador e contato comercial
- Tabela detalhada de itens com formato, papel, cores e acabamento (ex: Corte especial, laminação BOPP fosco frente e verso)
- Cláusulas e condições de fornecimento
- Bloco de assinatura da representante (Suzana Gomes de Souza) e aceite do cliente
