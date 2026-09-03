# GRAFICA — Ecossistema de Gestão e Orçamentos Gráficos

Repositório central do ecossistema de gestão gráfica e orçamentária, contendo os módulos de gestão ágil de sprints e o sistema CRM corporativo.

---

## 📁 Estrutura do Repositório

```text
GRAFICA/
├── AGSprint/               # Especificações, modelagem de banco de dados e gestão ágil de sprints
│   ├── database/           # Modelagem e schemas SQL (PostgreSQL)
│   ├── docs/               # Especificações técnicas e arquiteturais
│   ├── Gestao/             # Backlog e controle de sprints
│   └── docker-compose.yml  # Orquestração de serviços de suporte
│
└── CRM/                    # Sistema CRM em .NET 10 LTS (Clean Architecture)
    ├── src/
    │   ├── Crm.Domain/             # Entidades, Value Objects e Regras de Negócio
    │   ├── Crm.Application/        # Casos de uso, DTOs e FluentValidation
    │   ├── Crm.Infrastructure/     # EF Core 10, Npgsql (JSONB), QuestPDF
    │   ├── Crm.Api/                # Web API RESTful com Swagger/OpenAPI
    │   └── Crm.Web/                # Frontend Blazor Server com MudBlazor
    ├── tests/
    │   └── Crm.Application.Tests/  # Testes unitários e de integração
    ├── Crm.slnx                    # Solução .NET 10
    └── docker-compose.yml          # Ambiente containerizado completo (API, Web, DB, pgAdmin)
```

---

## 🛠️ Tecnologias Principais

- **Backend**: .NET 10 (C# 14), ASP.NET Core Web API, Entity Framework Core 10
- **Frontend**: Blazor Server, MudBlazor 9
- **Geração de Documentos**: QuestPDF 2026
- **Banco de Dados**: PostgreSQL 17 (suporte a `JSONB` e `sequences`)
- **Containers**: Docker & Docker Compose
- **Testes**: xUnit, FluentValidation

---

## 🚀 Como Iniciar

Consulte o [README do CRM](CRM/README.md) e as especificações em [AGSprint](AGSprint/docs/superpowers/specs/2026-09-02-agsprint-architecture-design.md) para obter instruções detalhadas de configuração e execução.
