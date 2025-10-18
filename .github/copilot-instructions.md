# FrameworkQ.Easyforms Development Instructions

This is an **HTMLDSL Form System** - HTML-based forms with data-* attributes that generate schemas, databases, and runtime validation. Built with .NET 9.0 backend + jQuery frontend.

## Core Architecture Pattern

**5-layer .NET backend** (strict dependencies):
- `Core` → Domain models, interfaces, expression engine (foundation)
- `Parser` → HTML→JSON extraction (AngleSharp)
- `Database` → SQL DDL generation (SQL Server/PostgreSQL)
- `Runtime` → Validation, submission processing
- `Api` → REST endpoints (orchestration layer)

**Frontend**: Pure jQuery modules (no build step)
- `formruntime.js` - Initialization and state management
- `expression-evaluator.js` - Client-side formula evaluation (mirrors C# engine)
- `validation.js`, `table-manager.js`, `data-fetch.js` - Feature modules

## Development Workflow (MANDATORY)

**ALL work must start with GitHub Issues** (see `CLAUDE.md` constitution):
```bash
# 1. Create issue first
gh issue create --title "Task description" --body "Details"

# 2. Branch with issue number
git checkout -b feature/42-task-name

# 3. Commit with issue reference
git commit -m "feat: implement feature (#42)"

# 4. PR that closes issue
gh pr create --title "Title" --body "Closes #42"
```

## Quick Start Commands

```bash
# Build and run API
cd backend && dotnet build FrameworkQ.Easyforms.sln
cd src/FrameworkQ.Easyforms.Api && dotnet run
# API: http://localhost:5000 (or 4000 with start script)

# Start both API + Web UI
./scripts/start-easyforms.sh --build --force

# Upload a form for testing
curl -X POST http://localhost:5000/v1/forms -F "htmlFile=@templates/examples/basic-form-example.html"
```

## Project-Specific Patterns

### HTMLDSL Convention
Forms use **HTML with semantic data-* attributes**:
```html
<form data-form="form-id" data-title="Form Title" data-version="1.0">
  <section data-page id="page-1" data-title="Page Title">
    <section data-section id="section-1" data-title="Section Title">
      <input name="field_name" type="text" data-type="string" data-required="true">
      <table data-table id="table-1" data-row-mode="infinite">
        <th data-col="column_name" data-type="integer" data-formula="col1 + col2">
      </table>
    </section>
  </section>
</form>
```

### Expression Engine (C# ↔ JavaScript Parity)
Both backend and frontend evaluate identical formulas:
- **Syntax**: `forced + scheduled`, `sum(column_name)`, `field > 0 && other_field != ""`
- **Functions**: `sum, avg, min, max, count, round, abs, ceil, floor`
- **Context**: Row-level (table columns) vs global (form fields)

### Database Schema Generation
Each `data-table` becomes a reporting table:
- **Naming**: `{form_id}_{page_id}_{section_id}_{widget_id}`
- **Standard columns**: `instance_id, page_id, section_id, widget_id, row_index, recorded_at`
- **Data columns**: Map `data-type` to SQL types (provider-specific)
- **Computed columns**: `data-formula` → SQL computed columns

### Validation Strategy (Dual-Layer)
1. **Client-side**: Instant UX feedback (jQuery validation.js)
2. **Server-side**: Security enforcement (Runtime.ValidationEngine.cs)
- Use HTML5 attributes (`required, min, max, pattern`) + mirror as `data-*`
- Conditional logic: `data-required-when="expression"`, `data-when="expression"`

## Key Files to Understand

### Backend Structure
- `Core/Models/FormDefinition.cs` - Root domain model
- `Core/Expressions/` - Formula tokenizer, parser, evaluator
- `Parser/HtmlParser.cs` - AngleSharp-based HTML→JSON extraction
- `Database/Providers/` - SQL Server vs PostgreSQL DDL generation
- `Api/Controllers/FormsController.cs` - Upload, schema extraction endpoints

### Frontend Integration
- `frontend/src/js/formruntime.js` - Form initialization and API orchestration  
- `templates/examples/` - Working HTMLDSL form examples
- `templates/benchmark/` - Real-world enterprise forms (Bengali + English)

### Configuration & Scripts
- `backend/src/FrameworkQ.Easyforms.Api/appsettings.json` - Database providers, CORS
- `scripts/start-easyforms.sh` - Development server startup
- `scripts/kill-ports.sh` - Clean shutdown

## Integration Points

### External Data Sources
Use `data-fetch` attributes for cascading dropdowns:
```html
<select data-fetch="GET:/api/substations?office={office}" 
        data-depends="office:required"
        data-value-key="id" data-label-key="name">
```
Goes through `/v1/proxy/fetch` endpoint for CORS + auth injection.

### Form Submission Flow
1. Client collects data via `FormRuntime.getData()`
2. POST to `/v1/submissions` with form data
3. Server validates via `ValidationEngine`
4. Extracts table data to reporting tables
5. Stores submission in `form_instances` + per-table rows

### Print Fidelity
CSS-based print optimization:
```html
<form data-print-page-size="A4" data-print-orientation="portrait">
  <table data-print-repeat-head-rows="2">
    <th data-width="25mm">Column</th>
```
Use `@media print` styles in CSS for pixel-perfect output.

## Testing & Validation

### Form Upload Test
```bash
curl -X POST http://localhost:5000/v1/forms \
  -F "htmlFile=@templates/examples/basic-form-example.html"
```

### Schema Extraction Test  
```bash
curl http://localhost:5000/v1/forms/employee-feedback/schema | jq
```

### Submission Test
```bash
curl -X POST http://localhost:5000/v1/submissions \
  -H "Content-Type: application/json" \
  -d '{"formId":"employee-feedback","data":{"employee_name":"John Doe"}}'
```

## Common Debugging

- **Expression errors**: Check both C# `Core/Expressions/` and JS `expression-evaluator.js` 
- **Schema extraction issues**: AngleSharp parsing in `Parser/HtmlParser.cs`
- **Database DDL problems**: Provider-specific logic in `Database/Providers/`
- **CORS errors**: Check `appsettings.json` AllowedOrigins
- **Port conflicts**: Use `scripts/kill-ports.sh` or `--force` flag

## Anti-Patterns to Avoid

- **Don't** modify domain models without updating both C# and JS expression engines
- **Don't** add backend dependencies to Core project (keep it pure)
- **Don't** use eval() in frontend (security risk - we have custom expression parser)
- **Don't** bypass the GitHub Issue workflow (constitution requirement)
- **Don't** assume single database provider (support both SQL Server and PostgreSQL)

Remember: This system prioritizes **pixel-perfect HTML layout** with **extractable semantics** over configuration-driven forms. The HTML IS the source of truth.