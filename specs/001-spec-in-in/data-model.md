# Data Model: HTMLDSL Form System

**Feature**: 001-spec-in-in | **Date**: 2025-10-11
**Phase**: 1 (Design)

## Overview

This document defines the data model for the HTMLDSL Form System, including domain entities, database schema for form storage and submissions, and relationships between entities. The model supports both the authoring/design-time schema (form definitions) and runtime schema (form submissions and reporting data).

---

## Domain Model (C# Classes)

### Core Entities

#### FormDefinition
Represents a complete form template with all metadata and structure.

```csharp
public class FormDefinition
{
    public string Id { get; set; }              // Unique slug (e.g., "qf-gmd-01")
    public string Title { get; set; }           // Human-readable title
    public string Version { get; set; }         // Version string (e.g., "1.0")
    public string[] Locales { get; set; }       // Supported locales (e.g., ["en", "bn"])
    public string StorageMode { get; set; }     // "jsonb" or "normalized"
    public string[] Tags { get; set; }          // Classification tags
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string HtmlSource { get; set; }      // Original HTML template
    public string SchemaJson { get; set; }      // Extracted JSON schema
    public List<Page> Pages { get; set; }       // Pages in the form
}
```

#### Page
Represents a logical page within a form.

```csharp
public class Page
{
    public string Id { get; set; }              // Unique page ID
    public string Title { get; set; }           // Page title
    public int Order { get; set; }              // Display order
    public List<Section> Sections { get; set; } // Sections on this page
}
```

#### Section
Represents a grouping of widgets within a page.

```csharp
public class Section
{
    public string Id { get; set; }              // Unique section ID
    public string Title { get; set; }           // Section title
    public string NumberingScheme { get; set; } // "auto", "decimal", "alpha", "roman", "none"
    public int Level { get; set; }              // Nesting level (0..N)
    public bool Collapsible { get; set; }       // Can be collapsed
    public bool Collapsed { get; set; }         // Initially collapsed
    public int Order { get; set; }              // Display order
    public List<Widget> Widgets { get; set; }   // Widgets in this section
}
```

#### Widget (Abstract Base)
Base class for all interactive form elements.

```csharp
public abstract class Widget
{
    public string Id { get; set; }              // Unique widget ID
    public WidgetType Type { get; set; }        // Field, Table, Grid, etc.
    public string When { get; set; }            // Conditional visibility expression
}

public enum WidgetType
{
    Field,
    Group,
    Table,
    Grid,
    Checklist,
    Signature,
    FormHeader,
    Notes,
    RadioGroup,
    CheckboxGroup,
    HierarchicalChecklist
}
```

#### Field : Widget
Single input field widget.

```csharp
public class Field : Widget
{
    public string Name { get; set; }            // Field key (required)
    public string DataType { get; set; }        // string, integer, decimal, date, etc.
    public string Label { get; set; }           // Display label
    public bool Required { get; set; }          // Is required
    public bool Readonly { get; set; }          // Is readonly
    public string DefaultValue { get; set; }    // Default value
    public string Unit { get; set; }            // Display unit (e.g., "kV")
    public string Pattern { get; set; }         // Validation regex
    public string Min { get; set; }             // Minimum value
    public string Max { get; set; }             // Maximum value
    public string Format { get; set; }          // Display format (e.g., "0.000")
    public string[] EnumValues { get; set; }    // Allowed enum values
    public string Compute { get; set; }         // Formula expression
    public bool Override { get; set; }          // Can override computed value
    public ValidationRule[] ValidationRules { get; set; }
    public FetchConfig FetchConfig { get; set; }
}
```

#### Group : Widget
Container for multiple fields with layout hints.

```csharp
public class Group : Widget
{
    public string Layout { get; set; }          // "columns:N" or "table"
    public List<Field> Fields { get; set; }     // Fields in this group
}
```

#### Table : Widget
Row-based data entry table.

```csharp
public class Table : Widget
{
    public string RowMode { get; set; }         // "infinite" or "finite"
    public int? MinRows { get; set; }           // Minimum rows
    public int? MaxRows { get; set; }           // Maximum rows
    public bool AllowAddRows { get; set; }      // Can add rows
    public bool AllowDeleteRows { get; set; }   // Can delete rows
    public string[] RowKey { get; set; }        // Natural key columns
    public List<Column> Columns { get; set; }   // Column definitions
    public List<Aggregate> Aggregates { get; set; } // Footer aggregates
    public PrintConfig PrintConfig { get; set; }
}
```

#### Column
Table column definition.

```csharp
public class Column
{
    public string Name { get; set; }            // Column key
    public string Label { get; set; }           // Display label
    public string DataType { get; set; }        // Column data type
    public bool Required { get; set; }
    public bool Readonly { get; set; }
    public string Unit { get; set; }
    public string Format { get; set; }
    public string[] EnumValues { get; set; }
    public string Formula { get; set; }         // Computed column expression
    public string DefaultValue { get; set; }
    public string Min { get; set; }
    public string Max { get; set; }
    public string Pattern { get; set; }
    public string Width { get; set; }           // Print width (e.g., "18mm")
    public string Align { get; set; }           // "left", "center", "right"
    public string VAlign { get; set; }          // "top", "middle", "bottom"
}
```

#### Grid : Widget
2D matrix with generated rows/columns.

```csharp
public class Grid : Widget
{
    public string RowGeneration { get; set; }   // "finite|infinite", "times:07:00-14:00/60"
    public string ColumnGeneration { get; set; }// "days-of-month", "times:...", "values:A,B,C"
    public string CellType { get; set; }        // Cell data type
    public string[] CellEnumValues { get; set; }// Allowed cell values
}
```

#### ValidationRule
Validation constraint for a field or container.

```csharp
public class ValidationRule
{
    public string Type { get; set; }            // "required", "pattern", "min", "max", "constraint"
    public string Expression { get; set; }      // For conditional and cross-field rules
    public string Message { get; set; }         // Error message
    public string Severity { get; set; }        // "error", "warning", "info"
    public string ValidateOn { get; set; }      // "input", "change", "blur", "submit"
}
```

#### FetchConfig
External data fetching configuration.

```csharp
public class FetchConfig
{
    public string Method { get; set; }          // HTTP method (GET, POST)
    public string Url { get; set; }             // Endpoint URL with tokens
    public string FetchOn { get; set; }         // "load", "focus", "input", "change"
    public int MinChars { get; set; }           // Min chars for autocomplete
    public int DebounceMs { get; set; }         // Debounce delay
    public string Map { get; set; }             // Field mapping (e.g., "value:id,label:name")
    public string Cache { get; set; }           // "ttl:10m", "session", "none"
    public string[] Depends { get; set; }       // Dependent field IDs
}
```

#### Aggregate
Table footer aggregate calculation.

```csharp
public class Aggregate
{
    public string Function { get; set; }        // "sum", "avg", "min", "max", "count"
    public string Column { get; set; }          // Source column name
    public string TargetId { get; set; }        // Target field ID for result
}
```

#### PrintConfig
Print-specific configuration.

```csharp
public class PrintConfig
{
    public string PageSize { get; set; }        // "A4", "Letter", "Legal", "Custom:WxH"
    public string Orientation { get; set; }     // "portrait", "landscape"
    public int[] MarginsM { get; set; }        // [top, right, bottom, left] in mm
    public double Scale { get; set; }           // Print scale (0.5..2.0)
    public int RepeatHeadRows { get; set; }     // Header rows to repeat
    public string PageBreak { get; set; }       // "before", "after", "avoid"
    public bool KeepTogether { get; set; }      // Prevent breaking
}
```

---

## Database Schema

### Form Storage Tables

#### forms
Stores form definitions and extracted schemas.

```sql
CREATE TABLE forms (
    id VARCHAR(100) PRIMARY KEY,                -- Form ID (slug)
    title NVARCHAR(255) NOT NULL,
    version VARCHAR(50) NOT NULL,
    locales VARCHAR(100),                       -- Comma-separated locale codes
    storage_mode VARCHAR(20),                   -- "jsonb" or "normalized"
    tags VARCHAR(255),                          -- Comma-separated tags
    html_source NVARCHAR(MAX) NOT NULL,         -- Original HTML template
    schema_json NVARCHAR(MAX) NOT NULL,         -- Extracted JSON schema
    created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    updated_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UNIQUE (id, version)                        -- Unique form+version combo
);

CREATE INDEX idx_forms_tags ON forms(tags);
CREATE INDEX idx_forms_updated ON forms(updated_at DESC);
```

PostgreSQL equivalent uses `TEXT` for large fields and `JSONB` for schema_json.

---

### Submission Tables

#### form_instances
Stores form submission headers and raw data.

```sql
CREATE TABLE form_instances (
    instance_id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), -- Submission ID
    form_id VARCHAR(100) NOT NULL,              -- References forms(id)
    form_version VARCHAR(50) NOT NULL,          -- Form version at submission time
    submitted_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    submitted_by NVARCHAR(255),                 -- User ID or email
    status VARCHAR(50) NOT NULL,                -- "draft", "submitted", "approved", "rejected"
    header_context NVARCHAR(MAX),               -- JSONB: header fields (org, doc_no, etc.)
    raw_data NVARCHAR(MAX) NOT NULL,            -- JSONB: complete form data
    FOREIGN KEY (form_id) REFERENCES forms(id)
);

CREATE INDEX idx_instances_form ON form_instances(form_id, form_version);
CREATE INDEX idx_instances_submitted ON form_instances(submitted_at DESC);
CREATE INDEX idx_instances_user ON form_instances(submitted_by);
```

PostgreSQL uses `UUID` and `JSONB` for header_context and raw_data.

---

### Reporting Tables (Dynamic)

Reporting tables are generated dynamically per form definition, one table per `data-table` or `data-grid` widget.

#### Naming Convention
`<form_id>_<page_id>_<section_id>_<widget_id>`

Example: `qf_gmd_01_page_1_main_measurements`

#### Standard Columns (All Reporting Tables)

```sql
instance_id UNIQUEIDENTIFIER NOT NULL,          -- References form_instances
page_id VARCHAR(100) NOT NULL,
section_id VARCHAR(100) NOT NULL,
widget_id VARCHAR(100) NOT NULL,
row_key VARCHAR(255),                           -- Natural key (if data-row-key defined)
row_index INT NOT NULL,                         -- Sequential row number
recorded_at DATETIME2 NOT NULL,
-- [Data columns based on column definitions]
-- [Computed columns based on data-formula]
FOREIGN KEY (instance_id) REFERENCES form_instances(instance_id) ON DELETE CASCADE
```

#### Example Reporting Table

For a table widget with columns `forced`, `scheduled`, `total` (where `total = forced + scheduled`):

```sql
CREATE TABLE qf_gmd_01_page_1_main_measurements (
    instance_id UNIQUEIDENTIFIER NOT NULL,
    page_id VARCHAR(100) NOT NULL DEFAULT 'page-1',
    section_id VARCHAR(100) NOT NULL DEFAULT 'main',
    widget_id VARCHAR(100) NOT NULL DEFAULT 'measurements',
    row_key VARCHAR(255),
    row_index INT NOT NULL,
    recorded_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    -- Data columns
    forced INT,
    scheduled INT,
    total AS (COALESCE(forced, 0) + COALESCE(scheduled, 0)) PERSISTED, -- Computed

    FOREIGN KEY (instance_id) REFERENCES form_instances(instance_id) ON DELETE CASCADE
);

CREATE INDEX idx_measurements_instance ON qf_gmd_01_page_1_main_measurements(instance_id);
CREATE INDEX idx_measurements_recorded ON qf_gmd_01_page_1_main_measurements(recorded_at);
```

PostgreSQL equivalent:
```sql
total INTEGER GENERATED ALWAYS AS (COALESCE(forced, 0) + COALESCE(scheduled, 0)) STORED
```

---

## Data Type Mapping

### DSL Type → SQL Server

| DSL Type   | SQL Server Type   | Notes                          |
|------------|-------------------|--------------------------------|
| string     | NVARCHAR(255)     | Variable-length text           |
| text       | NVARCHAR(MAX)     | Long text                      |
| integer    | INT               | 32-bit integer                 |
| decimal    | DECIMAL(18,6)     | High precision decimal         |
| date       | DATE              | Date only                      |
| time       | TIME              | Time only                      |
| datetime   | DATETIME2         | Date + time with timezone      |
| bool       | BIT               | Boolean (0/1)                  |
| enum       | NVARCHAR(100)     | Constrained string             |
| attachment | NVARCHAR(500)     | File path or URL               |
| signature  | NVARCHAR(500)     | Image path or data URL         |

### DSL Type → PostgreSQL

| DSL Type   | PostgreSQL Type   | Notes                          |
|------------|-------------------|--------------------------------|
| string     | VARCHAR(255)      | Variable-length text           |
| text       | TEXT              | Long text                      |
| integer    | INTEGER           | 32-bit integer                 |
| decimal    | NUMERIC(18,6)     | High precision decimal         |
| date       | DATE              | Date only                      |
| time       | TIME              | Time only                      |
| datetime   | TIMESTAMPTZ       | Date + time with timezone      |
| bool       | BOOLEAN           | Boolean (true/false)           |
| enum       | VARCHAR(100)      | Constrained string             |
| attachment | VARCHAR(500)      | File path or URL               |
| signature  | VARCHAR(500)      | Image path or data URL         |

---

## Relationships

```
FormDefinition (1) ──< (M) Page
Page (1) ──< (M) Section
Section (1) ──< (M) Widget
Widget (1) ──< (M) ValidationRule
Table (1) ──< (M) Column
Table (1) ──< (M) Aggregate

FormDefinition (1) ──< (M) FormInstance
FormInstance (1) ──< (M) ReportingTableRow (per widget)
```

---

## Schema Extraction Pipeline

```
HTML Template
    ↓
[HTML Parser (AngleSharp)]
    ↓
DOM Tree
    ↓
[Widget Parsers]
    ↓
Domain Model (FormDefinition + Pages + Sections + Widgets)
    ↓
[Schema Builder]
    ↓
Canonical JSON Schema
    ↓
[DDL Generator]
    ↓
SQL CREATE TABLE Statements
    ↓
Database
```

---

## Schema Versioning and Migration

### Schema Comparison

When a form definition changes:

1. Load existing schema JSON from `forms.schema_json`
2. Parse new HTML and extract new schema JSON
3. Compare schemas to identify changes:
   - Added fields/columns
   - Removed fields/columns
   - Renamed fields/columns
   - Changed data types
   - Changed validation rules

### Migration Rules

```csharp
public class MigrationPlan
{
    public string FormId { get; set; }
    public string OldVersion { get; set; }
    public string NewVersion { get; set; }
    public List<Transformation> Transformations { get; set; }
}

public class Transformation
{
    public string Type { get; set; }            // "add", "remove", "rename", "convert"
    public string Field { get; set; }           // Field/column name
    public string FromType { get; set; }        // Old data type
    public string ToType { get; set; }          // New data type
    public string NewName { get; set; }         // For rename transformations
    public bool Archive { get; set; }           // Archive removed fields
}
```

### Migration Execution

```sql
-- Example: Rename column
BEGIN TRANSACTION;

EXEC sp_rename 'qf_gmd_01_page_1_main_measurements.old_name', 'new_name', 'COLUMN';

UPDATE forms SET schema_json = @newSchema, version = @newVersion, updated_at = GETUTCDATE()
WHERE id = @formId;

COMMIT;
```

---

## Indexes and Constraints

### Form Tables
- Primary key: `id` (form slug)
- Unique constraint: `(id, version)`
- Indexes: `tags`, `updated_at`

### Submission Tables
- Primary key: `instance_id`
- Foreign key: `form_id` → `forms.id`
- Indexes: `(form_id, form_version)`, `submitted_at`, `submitted_by`

### Reporting Tables
- Foreign key: `instance_id` → `form_instances.instance_id` (CASCADE DELETE)
- Indexes: `instance_id`, `recorded_at`
- Optional indexes: User-defined via `data-indexes`
- Unique constraints: From `data-row-key` (composite unique index)

---

## Performance Considerations

1. **Form Schema Caching**: Cache extracted schemas in memory (5-minute sliding expiration)
2. **Connection Pooling**: Min 10, max 100 connections per database provider
3. **Batch Inserts**: Use bulk insert for reporting table rows (10+ rows)
4. **Indexed Queries**: All common query paths covered by indexes
5. **Computed Columns**: Database-native for performance (no runtime calculation)
6. **JSON Storage**: Use JSONB (PostgreSQL) or native JSON (SQL Server) for efficient querying

---

**Next Steps**: Proceed to API contract generation based on this data model.
