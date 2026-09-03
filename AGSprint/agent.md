# AGSprint - Memória de Desenvolvimento

## Contexto Inicial
- **Objetivo**: Criar o sistema AGSprint, uma gráfica de impressão que gera arquivos no formato de `ARQUIVO_MODELO/proposta_orcamento_modelo.pdf`.
- **Arquitetura de Gestão**: 
  - Estrutura baseada em pastas de Sprints (sprint_principal, sprint_1, sprint_2, etc.).
  - Dentro das sprints: arquivos de sprints contendo Histórias e Tasks.
  - Campos de controle de estado `[ ]` (aberto) e `[x]` (fechado) para tasks e histórias.
  - Hardgates: Sprints só são fechadas quando os hardgates são passados.
- **Processo de Desenvolvimento**:
  - Guiado ativamente pela skill `brainstorming`.
  - O agente deve questionar e validar decisões de design antes de qualquer implementação.
  - Nenhuma implementação será feita sem aprovação prévia.

## Decisões Arquiteturais Pendentes (Brainstorming)
- Definir a stack tecnológica do AGSprint (Backend, Frontend, Banco de dados).
- Estruturar como os hardgates das sprints serão validados (scripts, manual, CI/CD).
