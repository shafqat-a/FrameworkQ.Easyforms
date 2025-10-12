-- FrameworkQ.Easyforms - Forms Table Schema
-- Stores form definitions and extracted schemas

-- SQL Server
CREATE TABLE forms (
    id VARCHAR(100) PRIMARY KEY,
    title NVARCHAR(255) NOT NULL,
    version VARCHAR(50) NOT NULL,
    locales VARCHAR(100),
    storage_mode VARCHAR(20) DEFAULT 'jsonb',
    tags VARCHAR(255),
    html_source NVARCHAR(MAX) NOT NULL,
    schema_json NVARCHAR(MAX) NOT NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT uq_form_version UNIQUE (id, version)
);

CREATE INDEX idx_forms_tags ON forms(tags);
CREATE INDEX idx_forms_updated ON forms(updated_at DESC);

-- PostgreSQL equivalent:
-- CREATE TABLE forms (
--     id VARCHAR(100) PRIMARY KEY,
--     title VARCHAR(255) NOT NULL,
--     version VARCHAR(50) NOT NULL,
--     locales VARCHAR(100),
--     storage_mode VARCHAR(20) DEFAULT 'jsonb',
--     tags VARCHAR(255),
--     html_source TEXT NOT NULL,
--     schema_json JSONB NOT NULL,
--     created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
--     updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
--     CONSTRAINT uq_form_version UNIQUE (id, version)
-- );
--
-- CREATE INDEX idx_forms_tags ON forms(tags);
-- CREATE INDEX idx_forms_updated ON forms(updated_at DESC);
-- CREATE INDEX idx_forms_schema ON forms USING GIN(schema_json);
