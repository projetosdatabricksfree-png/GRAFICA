---
name: crm-backend-dotnet
description: Use sempre que o trabalho envolver o backend em C#/.NET do CRM de orçamentos — estrutura da solução, ASP.NET Core, Entity Framework Core + Npgsql, controllers/endpoints, DTOs, autenticação ou testes. Ative ao criar um novo projeto .csproj, uma migration, um controller ou qualquer classe dentro da solução deste sistema.
---

# Backend .NET — CRM de Orçamentos Gráficos

## Stack de referência (validar versão exata no momento da implementação)

- **.NET 10** — release LTS (suporte até novembro/2028). Não iniciar projeto
  novo em .NET 8/9 (fim de suporte em novembro/2026).
- **ASP.NET Core 10** para a API.
- **EF Core 10** + **Npgsql.EntityFrameworkCore.PostgreSQL** (linha 10.x,
  confirmada compatível com EF Core 10/.NET 10) como provider PostgreSQL.
- Documentação oficial: `learn.microsoft.com/aspnet/core`,
  `learn.microsoft.com/ef/core`, `npgsql.org/efcore`.

## Estrutura da solução (Clean Architecture simplificada)

```
Crm.sln
├── src/
│   ├── Crm.Domain/          # Entidades, sem dependência de EF Core ou ASP.NET
│   ├── Crm.Application/     # Casos de uso, DTOs, interfaces de repositório
│   ├── Crm.Infrastructure/  # CrmDbContext, implementação dos repositórios,
│   │                        # serviço de geração de PDF (ver skill crm-pdf-questpdf)
│   └── Crm.Api/             # Controllers/Minimal APIs, autenticação, Program.cs
└── tests/
    └── Crm.Application.Tests/
```

## Regras

- **Um único `DbContext`** (`CrmDbContext`), vive em `Crm.Infrastructure`.
  Nunca referencie `Microsoft.EntityFrameworkCore` a partir de `Crm.Domain`.
- **Migrations sempre via CLI, nunca `EnsureCreated`:**
  `dotnet ef migrations add <Nome> --project src/Crm.Infrastructure --startup-project src/Crm.Api`.
  `EnsureCreated()` não gera histórico de migration e não deve aparecer em
  nenhum ambiente além de um teste descartável.
- **DTOs nunca expõem entidades de domínio diretamente** nos endpoints —
  sempre um contrato próprio em `Crm.Application` (`PropostaResponseDto`,
  `CriarPropostaRequest`, etc.), mesmo que pareça redundante no início.
- **Endpoints versionados:** `/api/v1/propostas`, `/api/v1/clientes`. Facilita
  quebra de contrato futura sem quebrar o frontend Blazor.
- **Validação:** `FluentValidation` (ou `DataAnnotations` para casos simples)
  em `Crm.Application`, nunca dentro do controller.
- **Autenticação:** ASP.NET Core Identity + JWT Bearer, mesmo o frontend
  sendo Blazor Server — mantém a API desacoplada para um eventual segundo
  cliente (app, integração externa).
- **Testes de integração com Testcontainers** (`Testcontainers.PostgreSql`)
  subindo um Postgres real em container, em vez de mockar o `DbContext` —
  mais fiel ao comportamento real (JSONB, constraints, sequências).

## Fluxo típico de um caso de uso (ex.: criar proposta)

1. `Crm.Api` recebe `CriarPropostaRequest`, valida, chama
   `IPropostaService.CriarAsync` (interface em `Crm.Application`).
2. `PropostaService` (implementação em `Crm.Application` ou
   `Crm.Infrastructure`, conforme a dependência de repositório) monta a
   entidade `Proposta` + `PropostaItem[]`, persiste via
   `IPropostaRepository`.
3. Repositório (`Crm.Infrastructure`) usa `CrmDbContext` para salvar.
4. Retorna `PropostaResponseDto`; a geração do PDF é um passo separado
   (endpoint `/api/v1/propostas/{id}/pdf`), não acoplado à criação.

## O que evitar

- Lógica de negócio dentro do controller.
- Chamar `SaveChanges()` fora do repositório/unit-of-work.
- Misturar a entidade de domínio com o modelo de resposta da API (quebra o
  encapsulamento e cria acoplamento acidental com o EF Core no contrato
  público).
