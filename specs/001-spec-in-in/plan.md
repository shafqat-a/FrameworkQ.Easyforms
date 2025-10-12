# Implementation Plan: HTMLDSL Form System

**Branch**: `001-spec-in-in` | **Date**: 2025-10-11 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-spec-in-in/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Build an HTML-based domain-specific language (DSL) for designing structured data collection forms with pixel-perfect layout, runtime interactivity (validation, formulas, conditional logic), automatic schema extraction to JSON, and SQL DDL generation for reporting tables. The system enables form designers to use familiar HTML/CSS with semantic data-* annotations while providing runtime form behavior, database generation, and print fidelity for replacing paper-based workflows.

**Technical Approach**: .NET Core backend with provider-based architecture for SQL Server and PostgreSQL support, pure jQuery for client-side rendering and runtime behavior, HTML+CSS templates as the source of truth with server-side schema extraction and database generation.

## Technical Context

**Language/Version**: C# / .NET Core (LTS version)
**Primary Dependencies**:
- Backend: ASP.NET Core (web APIs), HtmlAgilityPack or AngleSharp (HTML parsing), Newtonsoft.Json or System.Text.Json
- Frontend: jQuery (DOM manipulation, event handling, AJAX), jQuery Validation (optional enhancement)
- Database: SQL Server and PostgreSQL via provider abstraction layer

**Storage**:
- Form definitions: HTML files + extracted JSON schemas
- Submissions: SQL Server or PostgreSQL (provider-based)
  - form_instances table (header + raw JSONB/JSON data)
  - Reporting tables (one per data-table/data-grid widget)

**Testing**: xUnit (backend unit/integration tests), JavaScript unit tests (QUnit or Jest for jQuery runtime)

**Target Platform**:
- Backend: Cross-platform server (Windows, Linux) via .NET Core
- Frontend: Modern web browsers (Chrome, Firefox, Safari, Edge)

**Project Type**: Web application (backend API + frontend static files)

**Performance Goals**:
- Schema extraction: <5 seconds for forms with 50 fields and 3 tables
- Database generation: <10 seconds for forms with up to 10 tables
- Form load time: <2 seconds for forms with 100 fields and 5 tables
- Calculated field updates: <100ms response time
- Support 50 concurrent users per form instance

**Constraints**:
- Maximum form complexity: 500 fields, 20 tables
- Table row operations: <100ms for tables with up to 1000 rows
- Print fidelity: 95%+ visual match to paper forms
- Validation feedback: <300ms after user input
- API response times: <1 second for 95% of external data fetches

**Scale/Scope**:
- Target: Small to medium enterprise deployments (10-50 concurrent users)
- Form volume: Dozens to hundreds of distinct form templates
- Submission volume: Thousands to hundreds of thousands per form
- No real-time collaborative editing required

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Status**: ✅ PASS (No project constitution defined yet - template only exists)

**Note**: The project constitution file (`.specify/memory/constitution.md`) contains only template placeholders. Once the constitution is ratified with actual principles, this check should be re-evaluated to ensure:
- Compliance with library-first principles (if applicable)
- Testing requirements (TDD, integration tests)
- Technology stack alignment
- Observability and logging standards
- Versioning and breaking change policies

For now, proceeding with standard software engineering best practices:
- ✅ Clear separation of concerns (backend API, frontend runtime, database layer)
- ✅ Provider abstraction for database portability
- ✅ Testable architecture (unit tests for parsers, integration tests for workflows)
- ✅ HTML as source of truth with extracted schema for versioning

## Project Structure

### Documentation (this feature)

```
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```
backend/
├── src/
│   ├── FrameworkQ.Easyforms.Core/           # Core domain models and interfaces
│   │   ├── Models/                          # Form, Page, Section, Widget entities
│   │   ├── Interfaces/                      # IFormParser, IDatabaseProvider, ISchemaExtractor
│   │   └── Expressions/                     # Expression parser and evaluator
│   ├── FrameworkQ.Easyforms.Parser/         # HTML to JSON schema extraction
│   │   ├── HtmlParser.cs                    # Main parsing logic
│   │   ├── WidgetParsers/                   # Field, Table, Grid parsers
│   │   └── SchemaBuilder.cs                 # Canonical JSON builder
│   ├── FrameworkQ.Easyforms.Database/       # Database generation and providers
│   │   ├── Providers/                       # SqlServerProvider, PostgreSqlProvider
│   │   ├── DdlGenerator.cs                  # SQL DDL generation
│   │   └── MigrationEngine.cs               # Schema migration logic
│   ├── FrameworkQ.Easyforms.Api/            # ASP.NET Core Web API
│   │   ├── Controllers/                     # FormsController, SubmissionsController
│   │   ├── Middleware/                      # Error handling, validation
│   │   └── Program.cs                       # Application entry point
│   └── FrameworkQ.Easyforms.Runtime/        # Runtime data handling
│       ├── ValidationEngine.cs              # Server-side validation
│       └── SubmissionProcessor.cs           # Save/submit logic
└── tests/
    ├── Unit/                                # Parser, expression, DDL unit tests
    ├── Integration/                         # Database provider integration tests
    └── Contract/                            # API contract tests

frontend/
├── src/
│   ├── js/
│   │   ├── formruntime.js                   # Main jQuery runtime initialization
│   │   ├── expression-evaluator.js          # Client-side formula/conditional logic
│   │   ├── validation.js                    # Client-side validation
│   │   ├── table-manager.js                 # Dynamic row add/remove
│   │   ├── data-fetch.js                    # External API data fetching
│   │   └── widgets/                         # Widget-specific behaviors
│   │       ├── table.js
│   │       ├── grid.js
│   │       └── signature.js
│   ├── css/
│   │   ├── forms.css                        # Base form styles
│   │   └── print.css                        # Print media styles
│   └── index.html                           # Demo/test harness
└── tests/
    └── js/                                  # QUnit or Jest tests for runtime

templates/                                    # Sample form HTML templates
├── examples/
│   ├── qf-gmd-01-log-sheet.html
│   ├── qf-gmd-14-roster.html
│   └── qf-gmd-17-checklist.html
└── base-template.html
```

**Structure Decision**: Web application structure with clear separation between backend (.NET Core) and frontend (jQuery static files). Backend is organized into multiple projects following .NET conventions:
- **Core**: Domain models and interfaces (no dependencies)
- **Parser**: HTML parsing and schema extraction (depends on Core)
- **Database**: SQL generation and database providers (depends on Core)
- **Api**: REST API endpoints (depends on all)
- **Runtime**: Submission and validation logic (depends on Core)

Frontend is a single project with modular JavaScript files organized by responsibility. Templates directory contains example forms for testing and reference.

## Complexity Tracking

*Fill ONLY if Constitution Check has violations that must be justified*

**Status**: No violations - Constitution not yet ratified

**Post-Design Re-Evaluation**: ✅ PASS

After completing Phase 0 (Research) and Phase 1 (Design), the architecture continues to align with software engineering best practices:

- ✅ **Separation of Concerns**: Backend (parsing, database, API) clearly separated from frontend (jQuery runtime)
- ✅ **Provider Abstraction**: Database provider pattern enables SQL Server and PostgreSQL support without tight coupling
- ✅ **Testability**: Unit tests for parsers/expression evaluator, integration tests for database providers, contract tests for APIs
- ✅ **Security**: Strict HTML sanitization with allowlist, no inline scripts, server-side validation
- ✅ **Scalability**: Stateless API design supports horizontal scaling for 50 concurrent users
- ✅ **Maintainability**: Modular jQuery plugins, well-defined C# projects with clear dependencies
- ✅ **Documentation**: Comprehensive contracts (OpenAPI, client runtime), quickstart guide, research decisions

No architectural red flags or anti-patterns detected. Design is ready for implementation (Phase 2: Tasks).
