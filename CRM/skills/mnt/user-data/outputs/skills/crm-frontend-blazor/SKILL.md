---
name: crm-frontend-blazor
description: Use sempre que o trabalho envolver as telas do CRM (frontend) — cadastro de clientes, criação/listagem de propostas, dashboard, layout, tema, cores ou "aparência" da aplicação. Ative ao criar um componente .razor, ajustar o tema visual, ou decidir como uma tela deve se comportar.
---

# Frontend — CRM de Orçamentos Gráficos

## Decisão de stack (e por quê)

**Blazor Server (.NET 10) + MudBlazor**, mantendo todo o sistema em C# e
reduzindo o número de linguagens/containers no docker-compose.

Alternativa avaliada: **Next.js + TailwindCSS + shadcn/ui** — visualmente
mais flexível e com ecossistema maior, mas exige uma stack Node.js separada
e uma segunda camada de contrato JSON só para o frontend consumir. Descartada
por ora para não fragmentar o time (que já concentra o backend em C#), mas é
uma opção legítima se a exigência estética evoluir para algo que o Material
Design do MudBlazor não entregue (glassmorphism pesado, animações
elaboradas, layout muito fora do padrão "aplicação corporativa").

## MudBlazor

- Biblioteca de componentes Material Design **100% C#**, sem necessidade de
  escrever CSS customizado para ter elevação, sombras e cantos arredondados
  (as "texturas" que dão aparência de produto acabado).
- Licença MIT. Compatibilidade confirmada com .NET 8/9 na linha 8.x/9.x —
  **validar no início do projeto** se já existe release estável para .NET 10
  em `mudblazor.com/docs` / `github.com/MudBlazor/MudBlazor`, já que o
  backend está no .NET 10 LTS.

## Padrões de tela

- **Layout compartilhado** (`MainLayout.razor`) com navegação lateral
  (`MudDrawer`) e barra superior (`MudAppBar`).
- **Listagens** (clientes, propostas): `MudTable` com paginação e busca
  server-side (não carregar tudo em memória).
- **Profundidade visual:** `MudPaper Elevation="4"` ou `MudCard` para os
  blocos principais em vez do `MudPaper Elevation="0"` padrão — é isso que
  cria a sensação de "textura"/profundidade pedida.
- **Tema:** customizar `MudTheme` (paleta de cor própria, tipografia) —
  nunca deixar o tema Material padrão "cru", que é facilmente reconhecível
  como template genérico.
- **Formulários:** sempre `MudForm` + `DataAnnotationsValidator`, com
  feedback de validação inline, nunca só na submissão.

## Regras

- Nenhum componente `.razor` acessa o `CrmDbContext` diretamente — sempre via
  um serviço de aplicação injetado (`IPropostaService`, etc.), consumindo a
  API HTTP ou uma referência direta ao projeto de Application, conforme a
  decisão de hospedagem do Blazor Server dentro da mesma solução.
- Estado de carregamento (`MudProgressCircular`) em toda chamada assíncrona
  que leve mais que uma interação instantânea — especialmente na geração do
  PDF, que envolve I/O.
- Mensagens de erro do usuário nunca expõem exception técnica — usar
  `MudSnackbar` com mensagem de negócio.

## O que evitar

- CSS customizado extenso por cima do MudBlazor — se uma tela está exigindo
  muito CSS próprio, é sinal de que o componente MudBlazor certo não foi
  usado, ou de que a stack deveria ser reavaliada (ver alternativa Next.js
  acima).
- Lógica de cálculo (ex. valor total do orçamento) duplicada no componente
  Blazor — isso é responsabilidade do backend (`Crm.Application`); o
  frontend só exibe.
