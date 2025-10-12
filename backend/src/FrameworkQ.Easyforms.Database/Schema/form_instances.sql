-- FrameworkQ.Easyforms - Form Instances Table Schema
-- Stores form submission data

-- SQL Server
CREATE TABLE form_instances (
    instance_id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    form_id VARCHAR(100) NOT NULL,
    form_version VARCHAR(50) NOT NULL,
    submitted_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    submitted_by NVARCHAR(255),
    status VARCHAR(50) NOT NULL DEFAULT 'draft',
    header_context NVARCHAR(MAX),
    raw_data NVARCHAR(MAX) NOT NULL,
    CONSTRAINT fk_instances_form FOREIGN KEY (form_id) REFERENCES forms(id)
);

CREATE INDEX idx_instances_form ON form_instances(form_id, form_version);
CREATE INDEX idx_instances_submitted ON form_instances(submitted_at DESC);
CREATE INDEX idx_instances_user ON form_instances(submitted_by);
CREATE INDEX idx_instances_status ON form_instances(status);

-- PostgreSQL equivalent:
-- CREATE TABLE form_instances (
--     instance_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
--     form_id VARCHAR(100) NOT NULL,
--     form_version VARCHAR(50) NOT NULL,
--     submitted_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
--     submitted_by VARCHAR(255),
--     status VARCHAR(50) NOT NULL DEFAULT 'draft',
--     header_context JSONB,
--     raw_data JSONB NOT NULL,
--     CONSTRAINT fk_instances_form FOREIGN KEY (form_id) REFERENCES forms(id)
-- );
--
-- CREATE INDEX idx_instances_form ON form_instances(form_id, form_version);
-- CREATE INDEX idx_instances_submitted ON form_instances(submitted_at DESC);
-- CREATE INDEX idx_instances_user ON form_instances(submitted_by);
-- CREATE INDEX idx_instances_status ON form_instances(status);
-- CREATE INDEX idx_instances_header ON form_instances USING GIN(header_context);
-- CREATE INDEX idx_instances_data ON form_instances USING GIN(raw_data);
