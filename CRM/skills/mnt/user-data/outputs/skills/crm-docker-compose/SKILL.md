---
name: crm-docker-compose
description: Use sempre que o trabalho envolver o ambiente Docker local do CRM — docker-compose.yml, Dockerfile, variáveis de ambiente, volumes, redes ou execução local dos containers (Postgres, backend, frontend). Ative ao criar ou depurar qualquer arquivo relacionado a Docker neste projeto.
---

# Docker Local — CRM de Orçamentos Gráficos

## Serviços mínimos

| Serviço | Imagem/base | Observação |
|---|---|---|
| `db` | `postgres:17-alpine` (oficial) | Volume nomeado para persistir dados entre `docker compose down`/`up` |
| `api` | build multi-stage a partir de `Crm.Api` | Depende de `db` com healthcheck, não apenas ordem de start |
| `web` | build a partir do projeto Blazor Server | Depende de `api` |
| `pgadmin` | `dpage/pgadmin4` | Apenas no profile `dev` — nunca sobe por padrão nem em produção |

## Regras

- **Nunca hardcode credenciais** no `docker-compose.yml`. Usar arquivo `.env`
  (adicionado ao `.gitignore`, com um `.env.example` versionado) e
  `env_file:` em cada serviço.
- **`depends_on` com healthcheck real:**
  ```yaml
  db:
    image: postgres:17-alpine
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER}"]
      interval: 5s
      timeout: 5s
      retries: 10
  api:
    depends_on:
      db:
        condition: service_healthy
  ```
  Só a ordem de `depends_on` sem `condition` não garante que o Postgres já
  aceita conexões quando a API sobe.
- **Dockerfile do backend em multi-stage:** `mcr.microsoft.com/dotnet/sdk:10.0`
  para build/publish, `mcr.microsoft.com/dotnet/aspnet:10.0` para runtime —
  reduz drasticamente o tamanho da imagem final.
- **Connection string via variável de ambiente**
  (`ConnectionStrings__Default`), nunca fixa em `appsettings.json` — o
  double underscore é a convenção do ASP.NET Core para mapear seção
  aninhada de configuração a partir de env var.
- **Migrations aplicadas de forma explícita e versionada:** um serviço
  `migrator` dedicado (mesma imagem da API, comando
  `dotnet ef database update`) que roda uma vez e sai, executado antes do
  `api` subir — nunca aplicar migration manualmente dentro de um container
  já em produção.
- **Rede:** todos os serviços na mesma rede bridge definida no compose;
  expor ao host só a porta da `web` (e a de `db`/`pgadmin` apenas quando
  precisar depurar com um cliente externo — remover antes de qualquer coisa
  parecida com produção).

## O que evitar

- Volume anônimo para o Postgres (perde dado a cada rebuild) — sempre volume
  nomeado.
- `POSTGRES_PASSWORD` fixa em texto no `docker-compose.yml` versionado no
  Git.
- Rodar `dotnet ef database update` manualmente dentro do container via
  `docker exec` como fluxo padrão — isso deve estar automatizado no serviço
  `migrator`.
