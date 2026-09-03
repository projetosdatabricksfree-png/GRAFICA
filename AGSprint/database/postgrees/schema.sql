-- =====================================================================
-- CRM de Orçamentos Gráficos — Schema PostgreSQL
-- Schema de banco: ags (todos os objetos vivem sob este namespace)
-- Referência de convenções: skill crm-dominio-dados
--   * snake_case
--   * PK bigint GENERATED ALWAYS AS IDENTITY (smallint só nas tabelas de
--     domínio, cujo id é fixo e curto)
--   * FKs sempre nomeadas explicitamente
--   * tabelas de domínio em vez de enum/string solta (status, forma de
--     pagamento, tipo de interação)
--   * especificações técnicas variáveis (papel, cores, acabamento) em JSONB
--   * auditoria (criado_em/atualizado_em/criado_por) em toda tabela
--     transacional; "quem" é sempre preenchido pela aplicação (contexto do
--     usuário autenticado), "quando" pode ser mantido por trigger
-- Compatível com PostgreSQL 16+.
-- =====================================================================

CREATE SCHEMA IF NOT EXISTS ags;

-- Opcional, só para conforto de sessão em cliente psql/DBeaver manual —
-- a aplicação (EF Core) deve sempre referenciar os objetos qualificados
-- (ags.tabela) ou configurar o schema no connection string, não depender
-- de search_path implícito:
-- SET search_path TO ags, public;

-- Extensão opcional (ative se quiser busca fuzzy por nome de cliente/produto):
-- CREATE EXTENSION IF NOT EXISTS pg_trgm;

-- ---------------------------------------------------------------------
-- Função utilitária: mantém atualizado_em em todo UPDATE
-- ---------------------------------------------------------------------
CREATE OR REPLACE FUNCTION ags.set_atualizado_em()
RETURNS trigger AS $$
BEGIN
    NEW.atualizado_em = now();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- =====================================================================
-- 1. USUÁRIOS — autenticação e auditoria (quem criou/alterou o quê)
-- =====================================================================
CREATE TABLE ags.usuarios (
    id              bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome            varchar(150) NOT NULL,
    email           varchar(180) NOT NULL,
    senha_hash      varchar(255) NOT NULL,
    ativo           boolean NOT NULL DEFAULT true,
    criado_em       timestamptz NOT NULL DEFAULT now(),
    atualizado_em   timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT uq_usuarios_email UNIQUE (email)
);

CREATE TRIGGER trg_usuarios_atualizado_em
    BEFORE UPDATE ON ags.usuarios
    FOR EACH ROW EXECUTE FUNCTION ags.set_atualizado_em();

COMMENT ON TABLE ags.usuarios IS 'Usuários do sistema (login). Representantes podem, ou não, ter usuário vinculado.';

-- =====================================================================
-- 2. TABELAS DE DOMÍNIO (lookup)
-- =====================================================================
CREATE TABLE ags.status_proposta (
    id          smallint PRIMARY KEY,
    codigo      varchar(20) NOT NULL,
    descricao   varchar(60) NOT NULL,
    CONSTRAINT uq_status_proposta_codigo UNIQUE (codigo)
);

CREATE TABLE ags.forma_pagamento (
    id          smallint PRIMARY KEY,
    codigo      varchar(20) NOT NULL,
    descricao   varchar(60) NOT NULL,
    CONSTRAINT uq_forma_pagamento_codigo UNIQUE (codigo)
);

CREATE TABLE ags.tipo_interacao (
    id          smallint PRIMARY KEY,
    codigo      varchar(20) NOT NULL,
    descricao   varchar(60) NOT NULL,
    CONSTRAINT uq_tipo_interacao_codigo UNIQUE (codigo)
);

COMMENT ON TABLE ags.status_proposta IS 'Domínio do ciclo de vida da proposta: rascunho, enviada, aprovada, recusada, expirada.';
COMMENT ON TABLE ags.forma_pagamento IS 'Domínio de condição de pagamento: à vista, a prazo, boleto, cartão.';
COMMENT ON TABLE ags.tipo_interacao  IS 'Domínio da linha do tempo da proposta: criação, envio, ligação, aprovação, recusa, reenvio.';

-- =====================================================================
-- 3. EMPRESAS EMISSORAS — multi-emissor desde o início
-- =====================================================================
CREATE TABLE ags.empresas_emissoras (
    id              bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    razao_social    varchar(180) NOT NULL,
    nome_fantasia   varchar(180),
    cnpj            varchar(18) NOT NULL,
    endereco        varchar(200),
    cidade          varchar(100),
    uf              char(2),
    cep             varchar(9),
    telefone        varchar(20),
    email           varchar(180),
    site            varchar(180),
    ativo           boolean NOT NULL DEFAULT true,
    criado_em       timestamptz NOT NULL DEFAULT now(),
    atualizado_em   timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT uq_empresas_emissoras_cnpj UNIQUE (cnpj)
);

CREATE TRIGGER trg_empresas_emissoras_atualizado_em
    BEFORE UPDATE ON ags.empresas_emissoras
    FOR EACH ROW EXECUTE FUNCTION ags.set_atualizado_em();

-- =====================================================================
-- 4. REPRESENTANTES — quem assina a proposta (ex.: "Suzana Gomes de Souza")
-- =====================================================================
CREATE TABLE ags.representantes (
    id              bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    usuario_id      bigint,
    nome            varchar(150) NOT NULL,
    telefone        varchar(20),
    email           varchar(180),
    ativo           boolean NOT NULL DEFAULT true,
    criado_em       timestamptz NOT NULL DEFAULT now(),
    atualizado_em   timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT fk_representantes_usuario FOREIGN KEY (usuario_id) REFERENCES ags.usuarios (id)
);

CREATE INDEX idx_representantes_usuario_id ON ags.representantes (usuario_id);
CREATE INDEX idx_representantes_nome ON ags.representantes (lower(nome));

CREATE TRIGGER trg_representantes_atualizado_em
    BEFORE UPDATE ON ags.representantes
    FOR EACH ROW EXECUTE FUNCTION ags.set_atualizado_em();

-- =====================================================================
-- 5. CLIENTES
-- =====================================================================
CREATE TABLE ags.clientes (
    id              bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    tipo_pessoa     char(2) NOT NULL DEFAULT 'PF',
    nome            varchar(180) NOT NULL,
    documento       varchar(20),
    codigo_externo  varchar(30),
    telefone        varchar(20),
    email           varchar(180),
    endereco        varchar(200),
    cidade          varchar(100),
    uf              char(2),
    cep             varchar(9),
    ativo           boolean NOT NULL DEFAULT true,
    criado_em       timestamptz NOT NULL DEFAULT now(),
    atualizado_em   timestamptz NOT NULL DEFAULT now(),
    criado_por      bigint,
    CONSTRAINT ck_clientes_tipo_pessoa CHECK (tipo_pessoa IN ('PF','PJ')),
    CONSTRAINT fk_clientes_criado_por FOREIGN KEY (criado_por) REFERENCES ags.usuarios (id)
);

CREATE INDEX idx_clientes_nome ON ags.clientes (lower(nome));
CREATE INDEX idx_clientes_documento ON ags.clientes (documento) WHERE documento IS NOT NULL;
CREATE INDEX idx_clientes_criado_por ON ags.clientes (criado_por);

CREATE TRIGGER trg_clientes_atualizado_em
    BEFORE UPDATE ON ags.clientes
    FOR EACH ROW EXECUTE FUNCTION ags.set_atualizado_em();

COMMENT ON COLUMN ags.clientes.documento IS 'CPF/CNPJ. Propositalmente NÃO é UNIQUE: o modelo real de referência usa documento genérico ("CONSUMIDOR" / 000.000.000-01) para clientes de balcão.';

-- =====================================================================
-- 6. CONTATOS — um cliente pode ter vários
-- =====================================================================
CREATE TABLE ags.contatos (
    id              bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    cliente_id      bigint NOT NULL,
    nome            varchar(150) NOT NULL,
    cargo           varchar(80),
    telefone        varchar(20),
    email           varchar(180),
    principal       boolean NOT NULL DEFAULT false,
    criado_em       timestamptz NOT NULL DEFAULT now(),
    atualizado_em   timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT fk_contatos_cliente FOREIGN KEY (cliente_id) REFERENCES ags.clientes (id) ON DELETE CASCADE
);

CREATE INDEX idx_contatos_cliente_id ON ags.contatos (cliente_id);
-- no máximo um contato "principal" por cliente
CREATE UNIQUE INDEX uq_contatos_principal_por_cliente ON ags.contatos (cliente_id) WHERE principal;

CREATE TRIGGER trg_contatos_atualizado_em
    BEFORE UPDATE ON ags.contatos
    FOR EACH ROW EXECUTE FUNCTION ags.set_atualizado_em();

-- =====================================================================
-- 7. PRODUTOS / SERVIÇOS — catálogo gráfico
-- =====================================================================
CREATE TABLE ags.produtos_servicos (
    id              bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    grupo           varchar(80) NOT NULL,
    descricao_base  varchar(200) NOT NULL,
    unidade_medida  varchar(20) NOT NULL DEFAULT 'UN',
    especificacoes  jsonb NOT NULL DEFAULT '{}',
    ativo           boolean NOT NULL DEFAULT true,
    criado_em       timestamptz NOT NULL DEFAULT now(),
    atualizado_em   timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX idx_produtos_servicos_grupo ON ags.produtos_servicos (grupo);
CREATE INDEX idx_produtos_servicos_especificacoes ON ags.produtos_servicos USING gin (especificacoes);

CREATE TRIGGER trg_produtos_servicos_atualizado_em
    BEFORE UPDATE ON ags.produtos_servicos
    FOR EACH ROW EXECUTE FUNCTION ags.set_atualizado_em();

COMMENT ON COLUMN ags.produtos_servicos.especificacoes IS 'Schema livre: formato, papel, gramatura, cores, acabamento. Evita alterar a tabela a cada novo tipo de produto gráfico.';

-- =====================================================================
-- 8. PROPOSTAS — cabeçalho do orçamento
-- =====================================================================
CREATE SEQUENCE ags.proposta_codigo_seq START WITH 1000 INCREMENT BY 1;

CREATE TABLE ags.propostas (
    id                  bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    codigo              bigint NOT NULL DEFAULT nextval('ags.proposta_codigo_seq'),
    empresa_emissora_id bigint NOT NULL,
    cliente_id          bigint NOT NULL,
    contato_id          bigint,
    representante_id    bigint NOT NULL,
    status_id           smallint NOT NULL DEFAULT 1,
    forma_pagamento_id  smallint,
    data_emissao        date NOT NULL DEFAULT current_date,
    validade_dias       smallint NOT NULL DEFAULT 10,
    prazo_entrega       varchar(100),
    observacoes         text,
    valor_total         numeric(12,2) NOT NULL DEFAULT 0,
    criado_em           timestamptz NOT NULL DEFAULT now(),
    atualizado_em       timestamptz NOT NULL DEFAULT now(),
    criado_por          bigint,
    atualizado_por      bigint,
    CONSTRAINT uq_propostas_codigo UNIQUE (codigo),
    CONSTRAINT ck_propostas_validade_dias CHECK (validade_dias > 0),
    CONSTRAINT ck_propostas_valor_total CHECK (valor_total >= 0),
    CONSTRAINT fk_propostas_empresa_emissora FOREIGN KEY (empresa_emissora_id) REFERENCES ags.empresas_emissoras (id),
    CONSTRAINT fk_propostas_cliente FOREIGN KEY (cliente_id) REFERENCES ags.clientes (id),
    CONSTRAINT fk_propostas_contato FOREIGN KEY (contato_id) REFERENCES ags.contatos (id),
    CONSTRAINT fk_propostas_representante FOREIGN KEY (representante_id) REFERENCES ags.representantes (id),
    CONSTRAINT fk_propostas_status FOREIGN KEY (status_id) REFERENCES ags.status_proposta (id),
    CONSTRAINT fk_propostas_forma_pagamento FOREIGN KEY (forma_pagamento_id) REFERENCES ags.forma_pagamento (id),
    CONSTRAINT fk_propostas_criado_por FOREIGN KEY (criado_por) REFERENCES ags.usuarios (id),
    CONSTRAINT fk_propostas_atualizado_por FOREIGN KEY (atualizado_por) REFERENCES ags.usuarios (id)
);

CREATE INDEX idx_propostas_cliente_id ON ags.propostas (cliente_id);
CREATE INDEX idx_propostas_representante_id ON ags.propostas (representante_id);
CREATE INDEX idx_propostas_empresa_emissora_id ON ags.propostas (empresa_emissora_id);
CREATE INDEX idx_propostas_status_data ON ags.propostas (status_id, data_emissao DESC);

CREATE TRIGGER trg_propostas_atualizado_em
    BEFORE UPDATE ON ags.propostas
    FOR EACH ROW EXECUTE FUNCTION ags.set_atualizado_em();

COMMENT ON COLUMN ags.propostas.codigo IS 'Código de negócio exibido no PDF (ex.: "Cod. Proposta: 62632"). Gerado por sequência própria — nunca MAX(codigo)+1, para evitar colisão em criação concorrente.';
COMMENT ON COLUMN ags.propostas.valor_total IS 'Denormalizado a partir da soma de proposta_itens.valor_total. Recalculado pela camada de aplicação a cada inclusão/alteração/remoção de item — nunca confie em trigger para esse cálculo, pois a regra de negócio (descontos, arredondamento) pode evoluir.';

-- =====================================================================
-- 9. ITENS DA PROPOSTA
-- =====================================================================
CREATE TABLE ags.proposta_itens (
    id                  bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    proposta_id         bigint NOT NULL,
    produto_servico_id  bigint,
    ordem               smallint NOT NULL DEFAULT 1,
    grupo               varchar(80) NOT NULL,
    descricao           varchar(300) NOT NULL,
    quantidade          numeric(10,2) NOT NULL,
    valor_unitario      numeric(12,4) NOT NULL,
    valor_total         numeric(12,2) NOT NULL,
    especificacoes      jsonb NOT NULL DEFAULT '{}',
    criado_em           timestamptz NOT NULL DEFAULT now(),
    atualizado_em       timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT ck_proposta_itens_quantidade CHECK (quantidade > 0),
    CONSTRAINT ck_proposta_itens_valor_unitario CHECK (valor_unitario >= 0),
    CONSTRAINT ck_proposta_itens_valor_total CHECK (valor_total >= 0),
    CONSTRAINT fk_proposta_itens_proposta FOREIGN KEY (proposta_id) REFERENCES ags.propostas (id) ON DELETE CASCADE,
    CONSTRAINT fk_proposta_itens_produto_servico FOREIGN KEY (produto_servico_id) REFERENCES ags.produtos_servicos (id)
);

CREATE INDEX idx_proposta_itens_proposta_id ON ags.proposta_itens (proposta_id);
CREATE INDEX idx_proposta_itens_produto_servico_id ON ags.proposta_itens (produto_servico_id);
CREATE INDEX idx_proposta_itens_especificacoes ON ags.proposta_itens USING gin (especificacoes);
CREATE UNIQUE INDEX uq_proposta_itens_ordem ON ags.proposta_itens (proposta_id, ordem);

CREATE TRIGGER trg_proposta_itens_atualizado_em
    BEFORE UPDATE ON ags.proposta_itens
    FOR EACH ROW EXECUTE FUNCTION ags.set_atualizado_em();

COMMENT ON COLUMN ags.proposta_itens.grupo IS 'Copiado de produtos_servicos no momento da inclusão do item — a proposta preserva o texto histórico mesmo que o catálogo mude depois.';
COMMENT ON COLUMN ags.proposta_itens.valor_total IS 'Calculado e gravado pela aplicação (quantidade × valor_unitário, arredondado) — propositalmente não é coluna GENERATED, para permitir regra de arredondamento/desconto específica sem depender de expressão SQL.';

-- =====================================================================
-- 10. HISTÓRICO DE INTERAÇÃO — linha do tempo CRM da proposta
-- =====================================================================
CREATE TABLE ags.historico_interacao (
    id                  bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    proposta_id         bigint NOT NULL,
    tipo_interacao_id   smallint NOT NULL,
    descricao           text,
    criado_em           timestamptz NOT NULL DEFAULT now(),
    criado_por          bigint,
    CONSTRAINT fk_historico_interacao_proposta FOREIGN KEY (proposta_id) REFERENCES ags.propostas (id) ON DELETE CASCADE,
    CONSTRAINT fk_historico_interacao_tipo FOREIGN KEY (tipo_interacao_id) REFERENCES ags.tipo_interacao (id),
    CONSTRAINT fk_historico_interacao_criado_por FOREIGN KEY (criado_por) REFERENCES ags.usuarios (id)
);

CREATE INDEX idx_historico_interacao_proposta_id ON ags.historico_interacao (proposta_id, criado_em DESC);

COMMENT ON TABLE ags.historico_interacao IS 'Append-only: nunca UPDATE/DELETE em produção, apenas INSERT — é o log de auditoria comercial da proposta.';

-- =====================================================================
-- 11. DOCUMENTOS GERADOS — versionamento do PDF emitido
-- =====================================================================
CREATE TABLE ags.proposta_documentos (
    id              bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    proposta_id     bigint NOT NULL,
    versao          smallint NOT NULL,
    caminho_arquivo varchar(300) NOT NULL,
    gerado_em       timestamptz NOT NULL DEFAULT now(),
    gerado_por      bigint,
    CONSTRAINT uq_proposta_documentos_versao UNIQUE (proposta_id, versao),
    CONSTRAINT fk_proposta_documentos_proposta FOREIGN KEY (proposta_id) REFERENCES ags.propostas (id) ON DELETE CASCADE,
    CONSTRAINT fk_proposta_documentos_gerado_por FOREIGN KEY (gerado_por) REFERENCES ags.usuarios (id)
);

CREATE INDEX idx_proposta_documentos_proposta_id ON ags.proposta_documentos (proposta_id);

COMMENT ON TABLE ags.proposta_documentos IS 'Cada reemissão do PDF cria uma nova versão, preservando o documento anterior (alinhado ao aviso do modelo real: "não mantemos os arquivos após a entrega" — aqui o sistema passa a manter).';

-- =====================================================================
-- SEED — dados de referência dos domínios (necessários para o FK default)
-- =====================================================================
INSERT INTO ags.status_proposta (id, codigo, descricao) VALUES
    (1, 'RASCUNHO', 'Rascunho'),
    (2, 'ENVIADA',  'Enviada'),
    (3, 'APROVADA', 'Aprovada'),
    (4, 'RECUSADA', 'Recusada'),
    (5, 'EXPIRADA', 'Expirada');

INSERT INTO ags.forma_pagamento (id, codigo, descricao) VALUES
    (1, 'A_VISTA', 'À vista'),
    (2, 'A_PRAZO', 'A prazo'),
    (3, 'BOLETO',  'Boleto'),
    (4, 'CARTAO',  'Cartão');

INSERT INTO ags.tipo_interacao (id, codigo, descricao) VALUES
    (1, 'CRIACAO',      'Proposta criada'),
    (2, 'ENVIO_EMAIL',  'Enviada por e-mail'),
    (3, 'LIGACAO',      'Contato por telefone'),
    (4, 'APROVACAO',    'Aprovada pelo cliente'),
    (5, 'RECUSA',       'Recusada pelo cliente'),
    (6, 'REENVIO',      'Reenviada');
