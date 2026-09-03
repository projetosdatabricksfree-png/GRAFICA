# Sprint 1 - Setup e Base

## Objetivo da Sprint
Montar a arquitetura base do sistema AGSprint e criar a modelagem de domínio básica para propostas e clientes.

## Histórias e Tasks

- [ ] **História 1.1: Configurar infraestrutura do projeto**
  - [x] Criar repositório e pastas de Gestão e Sprints.
  - [x] Documentar o architecture-design.md no diretório `docs/superpowers/specs`.
  - [ ] Criar projetos e solution .NET em `src/` (Api, Domain, Tests).
  - [ ] Configurar vínculos de referência entre os projetos.

- [ ] **História 1.2: Modelar domínio base de propostas**
  - [ ] Implementar classe `Cliente` no projeto Domain.
  - [ ] Implementar classe `Proposta` e suas dependências.
  - [ ] Validar entidades conforme a regra `crm-dominio-dados` (uso de Entity Framework).

## 🛑 HARDGATES PARA FECHAMENTO 🛑
*Nenhuma sprint pode ser fechada com [x] na raiz sem que todos estes itens estejam marcados como [x]:*

- [ ] Todas as tasks da sprint estão com `[x]`.
- [ ] O código base (`dotnet build`) compila sem erros ou avisos graves.
- [ ] As regras de arquitetura (projetos separados para API, Domain, Tests) estão aplicadas.
