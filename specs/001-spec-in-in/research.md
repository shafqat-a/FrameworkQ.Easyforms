# Research & Architecture Decisions: HTMLDSL Form System

**Feature**: 001-spec-in-in | **Date**: 2025-10-11
**Phase**: 0 (Research & Architecture)

## Overview

This document captures architectural research, technology evaluations, and design decisions for the HTMLDSL Form System. All decisions are based on the feature requirements and specified technology constraints (.NET Core, jQuery, SQL Server/PostgreSQL).

---

## 1. HTML Parsing Library

### Decision
**AngleSharp** for HTML parsing and DOM manipulation

### Rationale
- Pure C# implementation with WHATWG HTML5 compliance
- Strong CSS selector support (needed for data-* attribute extraction)
- DOM tree manipulation capabilities for template validation
- Better performance than HtmlAgilityPack for complex documents
- Active maintenance and .NET Core support
- Built-in HTML sanitization features (aligns with FR-075 security requirements)

### Alternatives Considered
- **HtmlAgilityPack**: Older, more widely used, but lacks HTML5 compliance and has XPath-only selectors (less intuitive than CSS selectors)
- **HAP with CsQuery**: Adds jQuery-like selectors but requires two dependencies
- **Regex parsing**: Too brittle for complex HTML structures with nested elements

### Implementation Notes
- Use AngleSharp's `IHtmlParser` interface for dependency injection
- Leverage `QuerySelectorAll()` for extracting elements with data-* attributes
- Validate HTML structure before parsing (detect malformed tags)

---

## 2. Database Provider Abstraction

### Decision
**Provider pattern** with separate implementations for SQL Server and PostgreSQL

### Rationale
- Clean separation of database-specific DDL syntax
- Enables unit testing with mock providers
- Supports future expansion to other databases (MySQL, Oracle)
- Allows database-specific optimizations (e.g., JSONB in PostgreSQL vs JSON in SQL Server)
- Aligns with requirement FR-046 for multi-database support

### Alternatives Considered
- **Entity Framework Core with code-first migrations**: Too heavyweight for dynamic schema generation; doesn't handle reporting table creation from form definitions
- **Dapper with manual SQL**: Good for queries but lacks abstraction for DDL generation
- **Single provider with dialect switching**: More complex conditional logic, harder to test

### Implementation Notes
```csharp
public interface IDatabaseProvider
{
    string GenerateCreateTableDdl(TableSchema schema);
    string GenerateComputedColumnDdl(string columnName, string expression);
    string MapDataType(string dslType);
    Task<bool> ExecuteDdlAsync(string ddl);
    Task<MigrationResult> MigrateSchemaAsync(SchemaComparison comparison);
}
```

Provider implementations:
- `SqlServerProvider`: Uses `nvarchar`, `datetime2`, `PERSISTED` computed columns
- `PostgreSqlProvider`: Uses `varchar`, `timestamptz`, `GENERATED ALWAYS AS ... STORED`

---

## 3. Expression Evaluation Engine

### Decision
**Custom lightweight expression parser** with limited operator set

### Rationale
- Full C# expression compilation (e.g., Roslyn) is security risk (allows arbitrary code execution)
- Limited DSL expression syntax is sufficient for form calculations (FR-026, FR-027, FR-028)
- Custom parser enables consistent behavior between server (validation) and client (runtime)
- Can generate SQL expressions for computed columns from same AST

### Alternatives Considered
- **Roslyn scripting**: Too powerful, security risk, heavyweight dependency
- **NCalc or DynamicExpresso**: Good libraries but need security review, may not support all needed features
- **JavaScript eval() on server via Jint/Jurassic**: Cross-language inconsistency, performance overhead

### Implementation Notes
Supported expression syntax:
- Arithmetic: `+`, `-`, `*`, `/`, `%`, parentheses
- Comparison: `==`, `!=`, `<`, `>`, `<=`, `>=`
- Logical: `&&`, `||`, `!`
- Functions: `sum()`, `avg()`, `min()`, `max()`, `count()`, `round()`, `abs()`, `ceil()`, `floor()`
- Field references: Alphanumeric identifiers matching `[a-z0-9_-]+`
- Context: `ctx.<field>` for global scope in row expressions

Parser approach:
1. Tokenize input string (lexer)
2. Build abstract syntax tree (recursive descent parser)
3. Evaluate AST with field value context (interpreter)
4. Translate AST to SQL for database computed columns

---

## 4. jQuery Runtime Architecture

### Decision
**Modular jQuery plugins** with centralized state management

### Rationale
- jQuery 3.x provides stable, lightweight DOM manipulation
- Plugin pattern enables widget-specific behaviors without monolithic code
- Event delegation for dynamic table rows (performance optimization)
- No build toolchain required (direct browser execution)
- Aligns with "pure jQuery" constraint from user input

### Alternatives Considered
- **Vanilla JavaScript**: More verbose, requires polyfills for older browsers, no productivity gain
- **React/Vue**: Violates "pure jQuery" constraint, adds build complexity
- **jQuery + TypeScript**: Adds build step, not requested by user

### Implementation Notes
Core runtime structure:
```javascript
// formruntime.js - Main initialization
(function($) {
    'use strict';

    window.FormRuntimeHTMLDSL = {
        mount: function(formElement) {
            // Initialize all subsystems
            this._initState(formElement);
            this._initValidation();
            this._initExpressions();
            this._initTables();
            this._initDataFetch();
            this._emitReady();
        },

        _state: {},  // Central state store
        _listeners: []  // Event listeners
    };
})(jQuery);
```

Plugin pattern for widgets:
```javascript
// widgets/table.js
$.fn.formTable = function(options) {
    return this.each(function() {
        var $table = $(this);
        var template = $table.find('[data-row-template]').detach();

        $table.on('click', '.add-row', function() {
            var $newRow = template.clone();
            // ... row initialization
        });
    });
};
```

---

## 5. Schema Versioning and Migration Strategy

### Decision
**Automatic migration with transformation rules** (per user choice Q2: Option B)

### Rationale
- Simplifies reporting (single schema version per form)
- Reduces query complexity (no version-aware logic needed)
- Aligns with requirement FR-080 for automatic migration
- Requires careful design of transformation rules to prevent data loss

### Alternatives Considered
- **Multiple versions coexist**: More complex queries, version proliferation
- **Manual migration**: Error-prone, requires DBA intervention

### Implementation Notes
Migration workflow:
1. Compare new schema JSON with stored schema JSON (diff algorithm)
2. Classify changes:
   - **Non-breaking**: Add field, add optional validation, change label → No migration needed
   - **Breaking**: Remove field, rename field, change data type → Requires transformation rule
3. Generate transformation SQL:
   - Field rename: `ALTER TABLE ... RENAME COLUMN old TO new`
   - Field removal: Archive old column, add `_archived_YYYYMMDD` suffix
   - Data type change: Create new column, copy with CAST, validate, drop old
4. Execute within transaction, rollback on error
5. Update stored schema JSON after successful migration

Transformation rule DSL:
```json
{
  "transformations": [
    {"type": "rename", "from": "old_name", "to": "new_name"},
    {"type": "convert", "field": "amount", "fromType": "integer", "toType": "decimal", "scale": 2},
    {"type": "remove", "field": "deprecated_field", "archive": true}
  ]
}
```

---

## 6. Sanitization Strategy

### Decision
**Strict allowlist with server-side sanitization** (per user choice Q1: Option A)

### Rationale
- Prevents XSS attacks (FR-075)
- Clear security posture (no inline scripts ever)
- Simplifies content security policy (CSP)
- Form templates are authored by trusted designers, not end users

### Alternatives Considered
- **CSP with inline scripts**: More complex infrastructure, still risky
- **Client-side sanitization only**: Bypassable, not secure

### Implementation Notes
Allowlist using AngleSharp:
```csharp
var allowedTags = new HashSet<string> {
    "form", "input", "select", "textarea", "label", "button",
    "div", "section", "aside", "table", "thead", "tbody", "tfoot",
    "tr", "th", "td", "ol", "ul", "li", "canvas", "h1", "h2", "h3",
    "p", "span", "strong", "em"
};

var allowedAttributes = new HashSet<string> {
    "id", "name", "type", "value", "class", "style",
    "for", "colspan", "rowspan", "required", "readonly",
    "min", "max", "step", "pattern", "placeholder"
};
// Plus all data-* attributes

// Remove any <script>, <iframe>, <object>, <embed>, <link> tags
// Remove event handler attributes (onclick, onerror, etc.)
// Remove javascript: and data: URIs from href/src attributes
```

---

## 7. Print Fidelity Implementation

### Decision
**CSS `@media print` with data-attribute-driven rules**

### Rationale
- Leverages browser's native print rendering (no PDF library needed)
- CSS provides precise control over page breaks, headers, column widths
- Data attributes (data-print-*, data-pagebreak) map cleanly to CSS rules
- Users can export to PDF via browser print dialog
- Meets 95% fidelity requirement (FR-053 through FR-059)

### Alternatives Considered
- **Server-side PDF generation (iText, PdfSharp)**: More control but heavyweight, licensing issues
- **Headless browser (Puppeteer via NodeServices)**: Cross-platform complexity, resource intensive
- **CSS + polyfills**: Adds runtime overhead for marginal improvement

### Implementation Notes
Generate print CSS from form metadata:
```css
/* From data-print-page-size="A4" */
@page {
    size: A4 portrait;
    margin: 10mm 10mm 10mm 10mm;
}

/* From data-print-repeat-head-rows */
table thead {
    display: table-header-group;
}

/* From data-width="18mm" on th */
th[data-col="timestamp"] {
    width: 18mm;
}

/* From data-pagebreak="before" */
[data-section="section-2"] {
    page-break-before: always;
}

/* From data-keep-together */
[data-keep-together] {
    page-break-inside: avoid;
}
```

---

## 8. Validation Strategy

### Decision
**Dual validation** - client-side (jQuery) and server-side (.NET)

### Rationale
- Client-side: Immediate feedback, better UX (FR-023, FR-024)
- Server-side: Security boundary, enforcement of business rules (FR-075)
- Shared expression evaluator ensures consistency
- Remote validation for database-dependent checks (FR-025)

### Alternatives Considered
- **Client-side only**: Insecure, bypassable
- **Server-side only**: Poor UX, slow feedback

### Implementation Notes
Client validation (jQuery):
- Native HTML5 validation attributes (required, pattern, min, max)
- Custom validation via data-* attributes
- Expression evaluation for cross-field constraints
- Visual feedback via Bootstrap-compatible classes (`.is-invalid`, `.invalid-feedback`)

Server validation (.NET):
- Deserialize submission JSON
- Re-evaluate all validation rules from extracted schema
- Return structured error response:
```json
{
  "valid": false,
  "errors": {
    "field_name": ["Error message 1", "Error message 2"],
    "table_id.row_0.col_name": ["Row-level error"]
  }
}
```

---

## 9. External Data Fetching

### Decision
**jQuery AJAX with server-side proxy** for external API calls

### Rationale
- Avoids CORS issues (API calls proxied through backend)
- Enables server-side authentication injection (FR-076, FR-077)
- Caching at server layer (FR-036)
- Audit logging of external API calls

### Alternatives Considered
- **Direct client-to-API calls**: CORS issues, exposes auth tokens in browser
- **GraphQL federation**: Over-engineering for this use case

### Implementation Notes
Client-side (data-fetch.js):
```javascript
function fetchOptions(fieldId, config) {
    var url = '/api/proxy/fetch';
    var params = {
        endpoint: config.url,  // Template: /api/breakers?substation={substation}
        method: config.method || 'GET',
        tokens: extractTokens(config.url)  // {substation: 'value'}
    };

    return $.ajax({
        url: url,
        data: params,
        cache: config.cache !== 'none'
    });
}
```

Server-side (ProxyController.cs):
```csharp
[HttpGet("api/proxy/fetch")]
public async Task<IActionResult> FetchExternalData([FromQuery] ProxyRequest request)
{
    // Validate allowed endpoints (whitelist)
    // Inject auth headers from server config
    // Call external API
    // Cache response based on TTL
    // Map response fields via data-fetch-map
    // Return normalized JSON
}
```

---

## 10. Concurrent User Support

### Decision
**Stateless API with database-level concurrency control**

### Rationale
- Stateless API scales horizontally (no session affinity)
- Database handles concurrent writes via transactions
- Optimistic concurrency for form submissions (version stamps)
- Meets 50 concurrent users requirement (per user choice Q3: Option A)

### Alternatives Considered
- **WebSocket real-time sync**: Over-engineering, not required by spec
- **Pessimistic locking**: Reduces concurrency, complex deadlock handling

### Implementation Notes
Concurrency strategy:
- Form definitions: Read-heavy, cached in memory, invalidate on update
- Submissions: Optimistic concurrency with version check
  ```sql
  UPDATE form_instances
  SET data = @newData, version = version + 1
  WHERE instance_id = @id AND version = @expectedVersion
  ```
- Reporting table inserts: No concurrency issues (append-only)

Performance targets (50 concurrent users):
- Connection pooling: Min 10, max 100 connections per database
- Caching: Form schemas cached for 5 minutes (sliding expiration)
- API response time: <200ms p95 (excluding external data fetch)

---

## 11. Testing Strategy

### Decision
**Layered testing** - unit, integration, contract tests

### Rationale
- Unit tests: Fast, isolated, cover parsing and expression logic
- Integration tests: Database providers, end-to-end schema extraction
- Contract tests: API endpoint contracts, ensure frontend/backend compatibility

### Alternatives Considered
- **E2E only**: Too slow for CI/CD, brittle
- **Unit only**: Misses integration issues

### Implementation Notes
Test structure:
```
tests/
├── Unit/
│   ├── ParserTests.cs              # HTML parsing logic
│   ├── ExpressionTests.cs          # Expression evaluation
│   ├── DdlGeneratorTests.cs        # SQL DDL generation
│   └── ValidationTests.cs          # Validation rules
├── Integration/
│   ├── SqlServerProviderTests.cs   # Real SQL Server DB
│   ├── PostgreSqlProviderTests.cs  # Real PostgreSQL DB
│   └── MigrationTests.cs           # Schema migration workflows
└── Contract/
    ├── FormsApiTests.cs            # POST /api/forms, GET /api/forms/{id}
    └── SubmissionsApiTests.cs      # POST /api/submissions
```

Frontend tests (QUnit):
```
frontend/tests/js/
├── expression-evaluator-tests.js
├── validation-tests.js
└── table-manager-tests.js
```

---

## 12. Error Handling and Observability

### Decision
**Structured logging with correlation IDs**

### Rationale
- Structured logs enable querying and monitoring
- Correlation IDs trace requests across layers
- Centralized error handling middleware
- Aligns with observability best practices

### Alternatives Considered
- **Plain text logs**: Harder to query and analyze
- **No correlation IDs**: Difficult to trace issues across layers

### Implementation Notes
Logging framework: Serilog with structured sinks (Console, File, optional Seq/ELK)

Log enrichment:
```csharp
Log.ForContext("CorrelationId", correlationId)
   .ForContext("FormId", formId)
   .Information("Parsing form {FormId}", formId);
```

Error response format:
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Form validation failed",
    "correlationId": "abc123",
    "details": {
      "field_name": ["Error message"]
    }
  }
}
```

---

## Summary of Key Decisions

| Area | Decision | Primary Rationale |
|------|----------|-------------------|
| HTML Parsing | AngleSharp | HTML5 compliance, CSS selectors, sanitization |
| Database Abstraction | Provider pattern | Multi-DB support, testability, extensibility |
| Expression Engine | Custom parser | Security, consistency, SQL generation |
| Frontend Runtime | Modular jQuery plugins | Lightweight, no build step, widget-specific behavior |
| Schema Migration | Automatic with transformations | Simpler queries, single schema version |
| Security | Strict allowlist, no scripts | Prevents XSS, clear security posture |
| Print | CSS @media print | Browser-native, precise control, no PDF lib |
| Validation | Dual (client + server) | UX + security |
| External APIs | Server-side proxy | CORS, auth injection, caching |
| Concurrency | Stateless API + DB control | Horizontal scaling, 50 concurrent users |
| Testing | Layered (unit/integration/contract) | Fast feedback, comprehensive coverage |
| Observability | Structured logging + correlation IDs | Debuggability, monitoring |

---

**Next Steps**: Proceed to Phase 1 (Design) to generate data-model.md and API contracts based on these architectural decisions.
