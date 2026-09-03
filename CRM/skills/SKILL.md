---
name: crm-dominio-dados
description: Use sempre que o trabalho envolver modelagem, alteração ou migration do banco PostgreSQL do CRM de orçamentos gráficos (projeto Inprima) — criação de tabelas, relacionamentos, novos campos, ou decidir onde uma entidade/atributo deve viver no esquema. Ative também ao revisar um DDL, um DbContext do EF Core, ou uma migration antes de aplicá-la.
---

# Domínio de Dados — CRM de Orçamentos Gráficos

## Contexto do projeto

Sistema para uma gráfica (referência: Inprima / SP Laser Cópias Esp Ltda) gerar,
versionar e acompanhar propostas de orçamento como o modelo real do cliente:
cabeçalho do emissor, código de proposta, código de cliente, itens com
especificação técnica de impressão (formato, papel, gramatura, cores,
acabamento), condições comerciais e assinatura de aceite.

Banco: PostgreSQL 16+. Acesso via EF Core (ver skill `crm-backend-dotnet`).

## Entidades macro (não é o DDL final — é o ponto de partida)

| Entidade | Campos-chave | Observação |
|---|---|---|
| `empresas_emissoras` | razao_social, cnpj, endereco, telefone, site | Multi-emissor desde o início, mesmo com 1 registro hoje — evita migration dolorosa depois |
| `clientes` | nome, documento (CPF/CNPJ), código externo | Documento pode ser genérico ("CONSUMIDOR" no modelo real) — não torne obrigatório nem único |
| `contatos` | cliente_id (FK), nome, telefone, email | Um cliente pode ter vários contatos |
| `representantes` | nome, telefone, email, usuario_id (FK opcional) | É quem assina a proposta ("Atenciosamente, ...") |
| `produtos_servicos` | grupo, descricao_base, especificacoes (JSONB) | Ver regra de JSONB abaixo |
| `propostas` | codigo (sequência própria, ex. 62632), cliente_id, representante_id, data_emissao, validade_dias, forma_pagamento, prazo_entrega, status_id | `codigo` é number de negócio, não o PK técnico |
| `proposta_itens` | proposta_id, produto_servico_id, grupo, descricao, quantidade, valor_unitario, valor_total, especificacoes (JSONB) | `valor_total` é calculado na aplicação, nunca column generated que dependa de arredondamento manual |
| `status_proposta` | tabela de domínio (rascunho, enviada, aprovada, expirada, recusada) | Nunca string solta na coluna de `propostas` |
| `historico_interacao` | proposta_id, tipo, descricao, criado_em, criado_por | Linha do tempo estilo CRM: e-mail, ligação, aprovação, reenvio |

## Regras

- **JSONB para especificações técnicas.** Cada produto gráfico do modelo real
  tem atributos diferentes (papel, corte, laminação, cores 4x0/4x4). Modelar
  isso em colunas fixas quebra a cada novo tipo de produto — use uma coluna
  `especificacoes jsonb` tanto em `produtos_servicos` quanto em
  `proposta_itens` (o item pode sobrescrever a especificação padrão do
  produto, como no modelo real onde o mesmo produto aparece com e sem
  laminação).
- **Tabelas de domínio em vez de enums de string.** `status_proposta`,
  `forma_pagamento` (à vista, a prazo, etc.) viram tabelas próprias com FK —
  facilita relatório e evita erro de digitação silencioso.
- **Nunca duplicar dado do cliente na proposta.** Nome do cliente, documento,
  etc. vêm sempre via FK para `clientes`; a proposta guarda apenas o que é
  imutável no momento da emissão (ex. condições comerciais daquele orçamento
  específico).
- **Código de proposta.** Gere via sequência PostgreSQL própria
  (`CREATE SEQUENCE proposta_codigo_seq`), nunca via `MAX(codigo)+1` — evita
  colisão em geração concorrente.
- **Auditoria.** `criado_em`, `atualizado_em`, `criado_por` em toda tabela
  transacional (propostas, proposta_itens, historico_interacao) — preencha
  via EF Core `SaveChanges` interceptor, não confie em trigger de banco para
  isso se a lógica de "por quem" depende do usuário autenticado na API.
- **Nomenclatura Postgres:** snake_case, chave primária `bigint identity` ou
  `uuid` (prefira `uuid` se houver replicação/sincronização futura),
  FKs sempre nomeadas explicitamente (`fk_proposta_itens_proposta`).

## O que evitar

- Colunas fixas por tipo de acabamento/papel (`tem_laminacao boolean`,
  `tipo_papel varchar`) — isso é o que o JSONB deve resolver.
- Guardar o valor total do orçamento apenas na proposta sem manter os itens —
  o modelo real precisa reconstruir a tabela de itens a qualquer momento para
  reemitir o PDF.
- `ON DELETE CASCADE` em `clientes → propostas` — histórico comercial não pode
  sumir se um cliente for removido; use exclusão lógica (`ativo boolean`) no
  cliente.
