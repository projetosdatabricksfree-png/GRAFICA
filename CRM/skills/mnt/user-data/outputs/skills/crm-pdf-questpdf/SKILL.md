---
name: crm-pdf-questpdf
description: Use sempre que o trabalho envolver a geração do PDF de orçamento/proposta do CRM — layout do documento, cabeçalho da empresa emissora, tabela de itens com especificação técnica, condições comerciais e área de assinatura. Ative ao criar ou alterar o serviço de geração de documentos, ou ao tentar replicar fielmente o modelo real de orçamento em código.
---

# Geração de PDF — Orçamento (QuestPDF)

## Biblioteca

**QuestPDF** (`questpdf.com`) — API fluente 100% C#, sem dependência de motor
HTML/Chromium. Roda inteiramente na infraestrutura local (sem chamada
externa), o que é compatível com o requisito de tudo rodar em Docker local.

- **Licenciamento:** gratuito para pessoas físicas, ONGs, projetos
  open-source e empresas com receita bruta anual abaixo de US$ 1 milhão;
  acima disso exige licença comercial (`questpdf.com/pricing`). **Confirmar
  o enquadramento do cliente (Inprima/SP Laser Cópias) antes de colocar em
  produção** — isso não é uma decisão técnica, é uma decisão de negócio que
  precisa ser validada com quem contrata o sistema.
- Documentação e tutorial oficial de fatura (~250 linhas de C#) em
  `questpdf.com/getting-started`.

## Estrutura do documento a replicar

Baseado no modelo real fornecido pelo cliente (proposta Inprima), o PDF tem:

1. **Cabeçalho do emissor** — razão social, nome fantasia, CNPJ, endereço,
   telefone, site.
2. **Bloco de destinatário** — "Prezado Sr.(a) {nome}", cidade e data de
   emissão, código de proposta e código de cliente.
3. **Tabela de itens** — colunas ID / Grupo / Descrição / Qtde / Vlr. Unit. /
   Vlr. Total, com uma ou mais linhas de especificação técnica logo abaixo de
   cada item (formato, papel/gramatura, cores, acabamento) — no modelo real
   isso aparece como texto solto abaixo da linha do item, não como coluna.
4. **Bloco de condições comerciais** — texto padrão ("Sujeito a alteração de
   valores...", "Não mantemos os arquivos..."), forma de pagamento, prazo de
   entrega, validade da proposta.
5. **Rodapé de assinatura** — nome do representante, telefone/e-mail, e a
   linha "De acordo: ___/___/___ — Cliente, visto e data."

## Regras

- **Componentizar**, nunca um único método gigante:
  `ComposeHeader(container, emissor)`,
  `ComposeItemsTable(container, itens)`,
  `ComposeConditions(container, proposta)`,
  `ComposeSignature(container, representante)`.
- Seguir o padrão oficial `Document.Create(container => container.Page(page => ...))`.
- **Fontes embutidas/locais** — não depender de fontes do sistema operacional
  do container, para o PDF ficar idêntico em qualquer ambiente Docker.
- **Dado de teste = dado real.** Ao validar o layout, gere o PDF a partir dos
  mesmos valores do modelo fornecido pelo cliente (ex. item "Divisória com
  aba 120", 2 variações de acabamento) e compare visualmente com o PDF
  original antes de considerar o layout pronto.
- Cálculo de `Vlr. Total` (quantidade × valor unitário) é responsabilidade da
  camada de aplicação (`Crm.Application`), não do serviço de PDF — o serviço
  de PDF só formata o que já veio calculado.

## O que evitar

- HTML-to-PDF (Puppeteer, wkhtmltopdf, headless Chrome) — foge do stack C#
  e introduz uma dependência externa pesada que o QuestPDF existe justamente
  para evitar.
- Gerar o PDF diretamente no controller da API — o serviço de geração vive em
  `Crm.Infrastructure` e é chamado por um caso de uso, igual a qualquer outra
  operação (ver skill `crm-backend-dotnet`).
