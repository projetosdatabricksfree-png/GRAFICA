# Design de Arquitetura - AGSprint

## 1. Visão Geral
O AGSprint será um sistema responsável pela gestão e geração de propostas/orçamentos gráficos em PDF (baseado no modelo `proposta_orcamento_modelo.pdf`).
O desenvolvimento do sistema e a gestão das tarefas serão organizados estritamente através de um sistema de pastas (Sprints) com regras de passagem (Hardgates).

## 2. Estrutura do Repositório e Gestão

A estrutura principal do projeto terá duas vertentes: **Gestão** (planejamento e tarefas) e **Código Fonte** (implementação).

```text
AGSprint/
├── Gestao/
│   └── Sprints/
│       ├── sprint_principal/      # Funciona como Backlog e épicos
│       │   └── backlog.md
│       ├── sprint_1/              # Iterações atuais e futuras
│       │   └── sprint.md
│       └── sprint_N/
├── src/
│   ├── AGSprint.Api/              # API Web .NET Core para endpoints
│   ├── AGSprint.Domain/           # Entidades e EF Core
│   └── AGSprint.Tests/            # Testes unitários/integração
└── agent.md                       # Memória do desenvolvimento guiada pelo agente
```

## 3. Gestão de Sprints e Hardgates

Dentro de cada pasta de Sprint (ex: `sprint_1`), haverá um arquivo consolidado (`sprint.md`).

### Padrão do Arquivo `sprint.md`
- **Histórias e Tasks**: Usa a sintaxe `[ ]` (aberto) e `[x]` (fechado) para controle estrito de progresso.
- **Hardgates**: Critérios obrigatórios que determinam se uma sprint pode ser fechada. Exemplos de Hardgates:
  - 100% das tasks marcadas com `[x]`.
  - Código compila sem avisos (warnings) críticos.
  - Testes relacionados passam com sucesso.
  - PDF gerado tem paridade visual básica com o `ARQUIVO_MODELO`.

## 4. Stack Tecnológica
Conforme os padrões do CRM atual:
- **Backend**: C# .NET 8/9, Web API.
- **Banco de Dados**: PostgreSQL com Entity Framework Core (Code First, migrations geradas).
- **Geração de PDF**: A ser definida no desenvolvimento (ex: iText7, DinkToPdf, ou QuestPDF).

## 5. Fluxo de Trabalho (Brainstorming)
Nenhuma nova funcionalidade ou alteração arquitetural é realizada sem:
1. Questionamento usando a abordagem de `brainstorming`.
2. Atualização e aprovação no documento de design ou plano de implementação.
3. Atualização dos hardgates e tasks da sprint atual.
