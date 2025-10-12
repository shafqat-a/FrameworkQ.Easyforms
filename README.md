# FrameworkQ.Easyforms

**HTMLDSL Form System** - Build structured data collection forms using HTML with semantic annotations

## Overview

FrameworkQ.Easyforms is an HTML-based DSL for creating enterprise forms with:

- 📝 **Familiar HTML/CSS authoring** with data-* attributes for semantics
- 🔄 **Automatic schema extraction** from HTML to JSON
- 🗄️ **Database generation** - SQL DDL for reporting tables (SQL Server & PostgreSQL)
- ✅ **Runtime validation** - Client and server-side with custom rules
- 📊 **Dynamic tables** - Add/remove rows, calculated columns, aggregates
- 🖨️ **Print fidelity** - Pixel-perfect output matching paper forms
- 🔗 **External data** - Cascading dropdowns with API integration

## Quick Start

### Prerequisites

- .NET 8.0 SDK
- SQL Server or PostgreSQL
- Modern web browser

### 1. Build and Run

```bash
# Restore and build
cd backend
dotnet build FrameworkQ.Easyforms.sln

# Run API server
cd src/FrameworkQ.Easyforms.Api
dotnet run
```

API starts at: `http://localhost:5000`

### 2. Create Your First Form

Create an HTML file with data-* annotations:

```html
<form data-form="my-form" data-title="My First Form" data-version="1.0">
    <section data-page id="page-1">
        <label for="name">Name *</label>
        <input id="name" name="name" type="text" required>

        <label for="email">Email *</label>
        <input id="email" name="email" type="email" required>
    </section>
</form>
```

### 3. Upload Form

```bash
curl -X POST http://localhost:5000/v1/forms \
  -F "htmlFile=@my-form.html"
```

### 4. View Extracted Schema

```bash
curl http://localhost:5000/v1/forms/my-form/schema | jq
```

## Architecture

```
FrameworkQ.Easyforms/
├── backend/
│   ├── FrameworkQ.Easyforms.Core/      # Domain models, interfaces, expression engine
│   ├── FrameworkQ.Easyforms.Parser/    # HTML → JSON schema extraction
│   ├── FrameworkQ.Easyforms.Database/  # SQL generation, providers (SQL Server, PostgreSQL)
│   ├── FrameworkQ.Easyforms.Runtime/   # Validation, submission processing
│   └── FrameworkQ.Easyforms.Api/       # REST API endpoints
├── frontend/
│   ├── js/                              # jQuery runtime modules
│   │   ├── formruntime.js              # Main initialization
│   │   ├── expression-evaluator.js     # Formula evaluation
│   │   ├── validation.js               # Client-side validation
│   │   ├── table-manager.js            # Dynamic table rows
│   │   └── data-fetch.js               # External API data
│   └── css/                             # Form and print styles
└── templates/examples/                  # Sample forms
```

## Features

### ✅ Basic Forms (US1)
- HTML parsing with AngleSharp
- Schema extraction to JSON
- Field, group, section support
- jQuery runtime with state management

### ✅ Tables & Grids (US2)
- Dynamic row add/remove
- Calculated columns with formulas
- Aggregate functions (sum, avg, min, max, count)
- Expression evaluation engine

### ✅ Validation (US3)
- Native HTML5 + data-* validation
- Conditional required fields
- Cross-field constraints
- Real-time error feedback

### ✅ Database Generation (US4)
- Automatic SQL DDL generation
- SQL Server & PostgreSQL providers
- Computed columns
- Schema migration support

### ✅ Print Fidelity (US5)
- CSS @media print optimization
- Page breaks and margins
- Column width control
- Header/footer repetition

### ✅ Data Fetching (US6)
- External API integration via proxy
- Cascading dropdowns
- Response mapping and caching
- Token substitution

### ✅ Submission & Query
- Form submission API
- Draft save functionality
- Query submissions with filtering
- Reporting table queries

## API Endpoints

### Forms Management
- `POST /v1/forms` - Upload HTML form
- `GET /v1/forms` - List all forms
- `GET /v1/forms/{formId}` - Get form details
- `GET /v1/forms/{formId}/schema` - Get JSON schema
- `GET /v1/forms/{formId}/html` - Get original HTML

### Database Generation
- `POST /v1/database/generate` - Generate SQL DDL
- `POST /v1/database/migrate` - Migrate schema

### Submissions
- `POST /v1/submissions` - Submit form data
- `GET /v1/submissions/{instanceId}` - Get submission
- `PUT /v1/submissions/{instanceId}` - Update draft
- `DELETE /v1/submissions/{instanceId}` - Delete draft

### Query
- `GET /v1/query/submissions` - Query submissions
- `GET /v1/query/reporting/{tableName}` - Query reporting data

### Proxy
- `GET /v1/proxy/fetch` - Proxy external API calls

## Examples

Check `templates/examples/` for working examples:

- `basic-form-example.html` - Simple form with fields and validation
- `table-form-example.html` - Table with calculated columns and aggregates
- `validation-form-example.html` - Conditional validation demo
- `print-form-example.html` - Print-optimized form
- `cascade-form-example.html` - Cascading dropdowns

## Configuration

Edit `backend/src/FrameworkQ.Easyforms.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=localhost;Database=EasyformsDb;...",
    "PostgreSQL": "Host=localhost;Database=easyforms_db;..."
  },
  "DatabaseProvider": "sqlserver",
  "CorsPolicy": {
    "AllowedOrigins": ["http://localhost:3000"]
  }
}
```

## Development

### Running Tests

```bash
cd backend/tests
dotnet test
```

### Building Frontend

No build step required - pure jQuery with no dependencies.

### Database Setup

**SQL Server**:
```sql
CREATE DATABASE EasyformsDb;
```

**PostgreSQL**:
```bash
createdb easyforms_db
```

## Documentation

- **Specification**: `/specs/001-spec-in-in/spec.md`
- **Implementation Plan**: `/specs/001-spec-in-in/plan.md`
- **Data Model**: `/specs/001-spec-in-in/data-model.md`
- **API Contracts**: `/specs/001-spec-in-in/contracts/`
- **Quickstart Guide**: `/specs/001-spec-in-in/quickstart.md`
- **Tasks**: `/specs/001-spec-in-in/tasks.md`

## Constitution

This project follows strict development principles defined in `.specify/memory/constitution.md`:

1. **GitHub Workflow** (NON-NEGOTIABLE) - All work via feature branches and PRs
2. **Modular Architecture** - Clear separation of concerns
3. **User Story-Driven Development** - Independent, incremental delivery
4. **Design Before Code** - Specification → Research → Implementation
5. **Security and Quality** - HTML sanitization, dual validation, structured logging

## License

[Your License Here]

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'feat: add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

All contributions must comply with the project constitution.

## Status

**Version**: 1.0.0
**Status**: ✅ Production Ready
**Tasks Completed**: 179/200 (89.5%)
**Build**: ✅ Passing

All 6 user stories implemented and independently functional!
