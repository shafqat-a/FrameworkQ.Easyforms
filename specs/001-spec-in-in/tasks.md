# Tasks: HTMLDSL Form System

**Input**: Design documents from `/specs/001-spec-in-in/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Tests are OPTIONAL in this project and are NOT included in this task list. Testing tasks can be added later if explicitly requested.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`
- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions
- **Backend**: `backend/src/FrameworkQ.Easyforms.*/`
- **Frontend**: `frontend/src/js/`, `frontend/src/css/`
- **Templates**: `templates/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [x] T001 [P] Create backend project structure: `backend/src/FrameworkQ.Easyforms.Core/`, `.Parser/`, `.Database/`, `.Api/`, `.Runtime/`
- [x] T002 [P] Create frontend project structure: `frontend/src/js/`, `frontend/src/css/`, `frontend/tests/js/`
- [x] T003 [P] Create test project structure: `backend/tests/Unit/`, `backend/tests/Integration/`, `backend/tests/Contract/`
- [x] T004 Initialize .NET Core solution file at `backend/FrameworkQ.Easyforms.sln` with all projects
- [x] T005 [P] Add NuGet dependencies: AngleSharp, ASP.NET Core, Newtonsoft.Json/System.Text.Json to appropriate projects
- [x] T006 [P] Configure project references: Core → no deps, Parser → Core, Database → Core, Api → All, Runtime → Core
- [x] T007 [P] Add jQuery 3.6+ to `frontend/src/js/jquery-3.6.0.min.js`
- [x] T008 [P] Create base CSS files: `frontend/src/css/forms.css`, `frontend/src/css/print.css`
- [x] T009 [P] Configure linting: Create `.editorconfig` for C# code style
- [x] T010 Create `backend/src/FrameworkQ.Easyforms.Api/appsettings.json` with database connection strings and provider configuration

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T011 [P] [Foundation] Define core interfaces in `backend/src/FrameworkQ.Easyforms.Core/Interfaces/IFormParser.cs`
- [x] T012 [P] [Foundation] Define `IDatabaseProvider` interface in `backend/src/FrameworkQ.Easyforms.Core/Interfaces/IDatabaseProvider.cs`
- [x] T013 [P] [Foundation] Define `ISchemaExtractor` interface in `backend/src/FrameworkQ.Easyforms.Core/Interfaces/ISchemaExtractor.cs`
- [x] T014 [P] [Foundation] Create base domain models in `backend/src/FrameworkQ.Easyforms.Core/Models/`: FormDefinition.cs, Page.cs, Section.cs, Widget.cs
- [x] T015 [P] [Foundation] Create expression AST classes in `backend/src/FrameworkQ.Easyforms.Core/Expressions/`: ExpressionNode.cs, BinaryOp.cs, FunctionCall.cs, FieldRef.cs
- [x] T016 [Foundation] Implement expression tokenizer in `backend/src/FrameworkQ.Easyforms.Core/Expressions/Tokenizer.cs`
- [x] T017 [Foundation] Implement expression parser (recursive descent) in `backend/src/FrameworkQ.Easyforms.Core/Expressions/Parser.cs`
- [x] T018 [Foundation] Implement expression evaluator in `backend/src/FrameworkQ.Easyforms.Core/Expressions/Evaluator.cs` (depends on T016, T017)
- [x] T019 [P] [Foundation] Create database schema: forms table DDL in `backend/src/FrameworkQ.Easyforms.Database/Schema/forms.sql`
- [x] T020 [P] [Foundation] Create database schema: form_instances table DDL in `backend/src/FrameworkQ.Easyforms.Database/Schema/form_instances.sql`
- [x] T021 [P] [Foundation] Setup ASP.NET Core middleware: error handling in `backend/src/FrameworkQ.Easyforms.Api/Middleware/ErrorHandlingMiddleware.cs`
- [x] T022 [P] [Foundation] Setup ASP.NET Core middleware: request logging in `backend/src/FrameworkQ.Easyforms.Api/Middleware/LoggingMiddleware.cs`
- [x] T023 [Foundation] Configure Serilog logging in `backend/src/FrameworkQ.Easyforms.Api/Program.cs`
- [x] T024 [Foundation] Configure CORS policy in `backend/src/FrameworkQ.Easyforms.Api/Program.cs`
- [x] T025 [Foundation] Configure database provider factory pattern in `backend/src/FrameworkQ.Easyforms.Database/DatabaseProviderFactory.cs`

**Checkpoint**: ✅ Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Basic Form Creation (Priority: P1) 🎯 MVP

**Goal**: Enable form designers to create HTML forms with data-* annotations that can be parsed and rendered

**Independent Test**: Create an HTML form with fields, pages, sections using data-* attributes → Upload to system → Verify schema extracted correctly → View form in browser → Verify renders correctly

### Implementation for User Story 1

- [x] T026 [P] [US1] Create Field widget model in `backend/src/FrameworkQ.Easyforms.Core/Models/Field.cs`
- [x] T027 [P] [US1] Create Group widget model in `backend/src/FrameworkQ.Easyforms.Core/Models/Group.cs`
- [x] T028 [P] [US1] Create ValidationRule model in `backend/src/FrameworkQ.Easyforms.Core/Models/ValidationRule.cs`
- [x] T029 [US1] Implement HTML parser initialization in `backend/src/FrameworkQ.Easyforms.Parser/HtmlParser.cs` using AngleSharp
- [x] T030 [US1] Implement form-level parser in `backend/src/FrameworkQ.Easyforms.Parser/HtmlParser.cs` (extract data-form, data-title, data-version)
- [x] T031 [US1] Implement page parser in `backend/src/FrameworkQ.Easyforms.Parser/HtmlParser.cs` (extract data-page containers)
- [x] T032 [US1] Implement section parser in `backend/src/FrameworkQ.Easyforms.Parser/HtmlParser.cs` (extract data-section containers)
- [x] T033 [P] [US1] Implement Field widget parser in `backend/src/FrameworkQ.Easyforms.Parser/WidgetParsers/FieldParser.cs` (extract input/select/textarea with name attributes)
- [x] T034 [P] [US1] Implement Group widget parser in `backend/src/FrameworkQ.Easyforms.Parser/WidgetParsers/GroupParser.cs` (extract data-group containers)
- [x] T035 [US1] Implement schema builder in `backend/src/FrameworkQ.Easyforms.Parser/SchemaBuilder.cs` (FormDefinition → JSON)
- [x] T036 [US1] Create FormsController in `backend/src/FrameworkQ.Easyforms.Api/Controllers/FormsController.cs` with POST /v1/forms endpoint (upload HTML)
- [x] T037 [US1] Implement GET /v1/forms/{formId} endpoint in FormsController.cs
- [x] T038 [US1] Implement GET /v1/forms/{formId}/schema endpoint in FormsController.cs (return extracted JSON)
- [x] T039 [US1] Implement GET /v1/forms/{formId}/html endpoint in FormsController.cs (return original HTML)
- [x] T040 [P] [US1] Create base form CSS in `frontend/src/css/forms.css` (form layout, sections, field groups)
- [x] T041 [P] [US1] Create jQuery runtime initialization in `frontend/src/js/formruntime.js` (FormRuntimeHTMLDSL.mount function)
- [x] T042 [US1] Implement state management in `frontend/src/js/formruntime.js` (_state object, _listeners array)
- [x] T043 [US1] Implement getValue() method in `frontend/src/js/formruntime.js` (serialize form to JSON)
- [x] T044 [US1] Implement setValue() method in `frontend/src/js/formruntime.js` (populate form from JSON)
- [x] T045 [US1] Emit formdsl:ready event in `frontend/src/js/formruntime.js`
- [x] T046 [US1] Emit formdsl:change event in `frontend/src/js/formruntime.js` on field changes
- [x] T047 [P] [US1] Create example template in `templates/examples/basic-form-example.html` (employee feedback form)
- [x] T048 [US1] Create HTML sanitizer in `backend/src/FrameworkQ.Easyforms.Parser/HtmlSanitizer.cs` (allowlist: form, input, select, textarea, label, section, div, etc.)
- [x] T049 [US1] Integrate sanitizer into HtmlParser.cs upload workflow

**Checkpoint**: ✅ At this point, basic forms can be created, uploaded, parsed, stored, retrieved, and rendered with jQuery runtime

---

## Phase 4: User Story 2 - Complex Table and Grid Support (Priority: P2)

**Goal**: Enable table widgets with dynamic rows, calculated columns, and aggregations

**Independent Test**: Create form with only a table widget → Add/remove rows dynamically → Enter data in columns → Verify calculated column updates → Verify aggregate totals compute correctly

### Implementation for User Story 2

- [x] T050 [P] [US2] Create Table widget model in `backend/src/FrameworkQ.Easyforms.Core/Models/Table.cs`
- [x] T051 [P] [US2] Create Column model in `backend/src/FrameworkQ.Easyforms.Core/Models/Column.cs`
- [x] T052 [P] [US2] Create Aggregate model in `backend/src/FrameworkQ.Easyforms.Core/Models/Aggregate.cs`
- [x] T053 [P] [US2] Create Grid widget model in `backend/src/FrameworkQ.Easyforms.Core/Models/Grid.cs`
- [x] T054 [US2] Implement Table widget parser in `backend/src/FrameworkQ.Easyforms.Parser/WidgetParsers/TableParser.cs` (extract data-table, columns from <th>, row template from <tr data-row-template>)
- [x] T055 [US2] Implement Grid widget parser in `backend/src/FrameworkQ.Easyforms.Parser/WidgetParsers/GridParser.cs` (extract data-grid, row-gen, col-gen attributes)
- [x] T056 [US2] Implement column parser in TableParser.cs (extract data-col, data-type, data-formula, data-required from <th>)
- [x] T057 [US2] Implement aggregate parser in TableParser.cs (extract data-agg, data-target from <tfoot>)
- [x] T058 [US2] Update SchemaBuilder.cs to include table and grid widgets in JSON output
- [x] T059 [P] [US2] Create table manager module in `frontend/src/js/table-manager.js` (row add/remove logic)
- [x] T060 [P] [US2] Create table widget plugin in `frontend/src/js/widgets/table.js` ($.fn.formTable)
- [x] T061 [US2] Implement row template cloning in table.js (clone <tr data-row-template>, update input names with row index)
- [x] T062 [US2] Implement add row button binding in table.js (bind click on .add-row or data-allow-add-rows)
- [x] T063 [US2] Implement remove row button binding in table.js (bind click on .remove-row)
- [x] T064 [P] [US2] Create expression evaluator module in `frontend/src/js/expression-evaluator.js` (parse and evaluate data-compute expressions)
- [x] T065 [US2] Implement arithmetic operators in expression-evaluator.js (+, -, *, /, %, parentheses)
- [x] T066 [US2] Implement comparison operators in expression-evaluator.js (==, !=, <, >, <=, >=)
- [x] T067 [US2] Implement logical operators in expression-evaluator.js (&&, ||, !)
- [x] T068 [US2] Implement math functions in expression-evaluator.js (sum, avg, min, max, count, round, abs, ceil, floor)
- [x] T069 [US2] Implement field reference resolution in expression-evaluator.js (resolve field names to values, support ctx. prefix)
- [x] T070 [US2] Integrate expression evaluator with formruntime.js (_initExpressions method)
- [x] T071 [US2] Implement computed field marking as readonly in formruntime.js
- [x] T072 [US2] Implement computed field recalculation on dependency changes in formruntime.js
- [x] T073 [US2] Implement aggregate calculation logic in table.js (sum, avg, min, max, count over table columns)
- [x] T074 [US2] Implement aggregate target field update in table.js (update data-target element with aggregate result)
- [x] T075 [US2] Bind aggregate recalculation to row add/remove/change events in table.js
- [x] T076 [P] [US2] Create grid widget plugin in `frontend/src/js/widgets/grid.js` ($.fn.formGrid)
- [x] T077 [US2] Implement column generation logic in grid.js (days-of-month, time slots, value lists)
- [x] T078 [US2] Implement row generation logic in grid.js (time ranges, numeric ranges, value lists)
- [x] T079 [P] [US2] Create table example in `templates/examples/table-form-example.html` (performance ratings table with aggregates)

**Checkpoint**: ✅ At this point, tables and grids work with dynamic rows, calculated columns, and aggregates

---

## Phase 5: User Story 3 - Runtime Validation and Conditional Logic (Priority: P3)

**Goal**: Add client-side validation, conditional visibility, and cross-field constraints

**Independent Test**: Create form with validation rules → Enter invalid data → Verify error messages appear → Add conditional fields → Change dependency → Verify fields show/hide correctly

### Implementation for User Story 3

- [x] T080 [P] [US3] Create validation module in `frontend/src/js/validation.js`
- [x] T081 [US3] Implement native HTML5 validation support in validation.js (required, pattern, min, max, step, minlength, maxlength)
- [x] T082 [US3] Implement data-* validation mirroring in validation.js (data-required, data-pattern, data-min, data-max)
- [x] T083 [US3] Implement conditional required validation in validation.js (data-required-when expression evaluation)
- [x] T084 [US3] Implement cross-field constraint validation in validation.js (data-constraint expression evaluation on containers)
- [x] T085 [US3] Implement validation timing configuration in validation.js (data-validate-on: input/change/blur/submit)
- [x] T086 [US3] Implement error message display in validation.js (add .is-invalid class, create .invalid-feedback elements)
- [x] T087 [US3] Implement custom error message support in validation.js (data-error, data-error-required, data-error-pattern, etc.)
- [x] T088 [US3] Implement error summary panel in validation.js (collect errors into data-error-summary container)
- [x] T089 [US3] Integrate validation module with formruntime.js (_initValidation method)
- [x] T090 [US3] Implement validate() public method in formruntime.js (trigger validation on form or specific field)
- [x] T091 [US3] Emit formdsl:validation event in formruntime.js
- [x] T092 [US3] Implement conditional visibility logic in formruntime.js (data-when expression evaluation)
- [x] T093 [US3] Implement show/hide toggling in formruntime.js (add/remove .hidden class or style.display)
- [x] T094 [US3] Bind conditional visibility to dependency field changes in formruntime.js
- [x] T095 [US3] Implement validation on form submit in formruntime.js (prevent submit if validation fails)
- [x] T096 [US3] Implement scroll-to-first-error on validation failure in formruntime.js
- [x] T097 [P] [US3] Create ValidationEngine in `backend/src/FrameworkQ.Easyforms.Runtime/ValidationEngine.cs` (server-side validation)
- [x] T098 [US3] Implement server-side validation rule evaluation in ValidationEngine.cs (required, pattern, min, max, enum)
- [x] T099 [US3] Implement server-side expression evaluation for constraints in ValidationEngine.cs (reuse Core/Expressions)
- [x] T100 [US3] Implement validation result formatting in ValidationEngine.cs (return structured errors: field → [messages])
- [x] T101 [P] [US3] Create validation example in `templates/examples/validation-form-example.html` (form with complex validation rules)

**Checkpoint**: ✅ At this point, forms have full client and server-side validation with conditional visibility

---

## Phase 6: User Story 4 - Schema Extraction and Database Generation (Priority: P4)

**Goal**: Automatically extract schema from HTML and generate database tables for form submissions

**Independent Test**: Upload form HTML → Extract schema JSON → Generate SQL DDL → Execute DDL → Verify tables created with correct columns, types, constraints

### Implementation for User Story 4

- [x] T102 [P] [US4] Create SqlServerProvider in `backend/src/FrameworkQ.Easyforms.Database/Providers/SqlServerProvider.cs`
- [x] T103 [P] [US4] Create PostgreSqlProvider in `backend/src/FrameworkQ.Easyforms.Database/Providers/PostgreSqlProvider.cs`
- [x] T104 [US4] Implement MapDataType in SqlServerProvider.cs (DSL types → SQL Server types: string→nvarchar, integer→int, decimal→decimal, etc.)
- [x] T105 [US4] Implement MapDataType in PostgreSqlProvider.cs (DSL types → PostgreSQL types: string→varchar, integer→integer, decimal→numeric, etc.)
- [x] T106 [P] [US4] Create DdlGenerator in `backend/src/FrameworkQ.Easyforms.Database/DdlGenerator.cs`
- [x] T107 [US4] Implement GenerateCreateTableDdl in DdlGenerator.cs (create form_instances table DDL)
- [x] T108 [US4] Implement reporting table DDL generation in DdlGenerator.cs (one table per data-table/data-grid widget)
- [x] T109 [US4] Implement standard column generation in DdlGenerator.cs (instance_id, page_id, section_id, widget_id, row_key, recorded_at)
- [x] T110 [US4] Implement data column generation in DdlGenerator.cs (from table column definitions)
- [x] T111 [US4] Implement computed column generation in DdlGenerator.cs (SQL Server: PERSISTED, PostgreSQL: GENERATED ALWAYS AS ... STORED)
- [x] T112 [US4] Implement constraint generation in DdlGenerator.cs (NOT NULL, CHECK for min/max, unique indexes)
- [x] T113 [US4] Implement custom index generation in DdlGenerator.cs (from data-indexes attribute)
- [x] T114 [US4] Implement ExecuteDdlAsync in SqlServerProvider.cs (execute DDL statements via ADO.NET)
- [x] T115 [US4] Implement ExecuteDdlAsync in PostgreSqlProvider.cs (execute DDL statements via Npgsql)
- [x] T116 [US4] Create DatabaseController in `backend/src/FrameworkQ.Easyforms.Api/Controllers/DatabaseController.cs`
- [x] T117 [US4] Implement POST /v1/database/generate endpoint in DatabaseController.cs (generate and execute DDL)
- [x] T118 [US4] Add dryRun parameter support in /v1/database/generate (return DDL without executing)
- [x] T119 [P] [US4] Create MigrationEngine in `backend/src/FrameworkQ.Easyforms.Database/MigrationEngine.cs`
- [x] T120 [US4] Implement schema comparison in MigrationEngine.cs (compare old vs new schema JSON, identify changes)
- [x] T121 [US4] Implement transformation rule generation in MigrationEngine.cs (add, remove, rename, convert)
- [x] T122 [US4] Implement migration SQL generation in MigrationEngine.cs (ALTER TABLE statements for schema changes)
- [x] T123 [US4] Implement MigrateSchemaAsync in MigrationEngine.cs (execute migration within transaction, rollback on error)
- [x] T124 [US4] Implement POST /v1/database/migrate endpoint in DatabaseController.cs (migrate form schema)

**Checkpoint**: ✅ At this point, forms automatically generate database schemas and support schema migration

---

## Phase 7: User Story 5 - Print Fidelity and Pixel-Perfect Layout (Priority: P5)

**Goal**: Enable forms to print with exact fidelity to paper originals

**Independent Test**: Create form with print attributes → Print to PDF → Compare with original paper form → Verify 95%+ visual match

### Implementation for User Story 5

- [x] T125 [P] [US5] Create PrintConfig model in `backend/src/FrameworkQ.Easyforms.Core/Models/PrintConfig.cs`
- [x] T126 [US5] Implement print attribute parsing in HtmlParser.cs (data-print-page-size, data-print-margins-mm, data-print-orientation, data-print-scale)
- [x] T127 [US5] Implement page break attribute parsing in HtmlParser.cs (data-pagebreak, data-keep-together, data-keep-with-next)
- [x] T128 [US5] Implement table print attribute parsing in TableParser.cs (data-print-repeat-head-rows, data-width, data-align, data-valign)
- [x] T129 [P] [US5] Create print CSS in `frontend/src/css/print.css` (@media print rules)
- [x] T130 [US5] Implement @page size generation in print.css (from data-print-page-size: A4, Letter, Legal, Custom)
- [x] T131 [US5] Implement @page margin generation in print.css (from data-print-margins-mm)
- [x] T132 [US5] Implement page break CSS in print.css (page-break-before, page-break-after, page-break-inside:avoid)
- [x] T133 [US5] Implement table header repetition in print.css (thead {display: table-header-group})
- [x] T134 [US5] Implement column width CSS in print.css (from data-width on <th>: convert mm/px/% to CSS)
- [x] T135 [US5] Implement cell alignment CSS in print.css (from data-align, data-valign)
- [x] T136 [US5] Implement keep-together CSS in print.css ([data-keep-together] {page-break-inside: avoid})
- [x] T137 [US5] Implement page counter logic in formruntime.js (populate data-page-counter, data-page-total elements)
- [x] T138 [P] [US5] Create print example in `templates/examples/print-form-example.html` (log sheet with complex table headers)

**Checkpoint**: ✅ At this point, forms print with high fidelity to paper originals

---

## Phase 8: User Story 6 - Dynamic Data Sources and Cascading Lookups (Priority: P6)

**Goal**: Enable fields to fetch options from external APIs with cascading dependencies

**Independent Test**: Create form with data-fetch field → Focus field → Verify options loaded from API → Change parent field → Verify dependent field options reload

### Implementation for User Story 6

- [x] T139 [P] [US6] Create FetchConfig model in `backend/src/FrameworkQ.Easyforms.Core/Models/FetchConfig.cs`
- [x] T140 [US6] Implement data-fetch attribute parsing in FieldParser.cs (method, URL, fetch-on, map, cache, depends)
- [x] T141 [P] [US6] Create data-fetch module in `frontend/src/js/data-fetch.js`
- [x] T142 [US6] Implement fetchOptions function in data-fetch.js (AJAX call to proxy endpoint)
- [x] T143 [US6] Implement URL token substitution in data-fetch.js (replace {fieldName}, {search} with current values)
- [x] T144 [US6] Implement response field mapping in data-fetch.js (data-fetch-map: value:id,label:name)
- [x] T145 [US6] Implement client-side caching in data-fetch.js (session storage with TTL)
- [x] T146 [US6] Implement debouncing for autocomplete in data-fetch.js (data-fetch-debounce)
- [x] T147 [US6] Implement cascade dependency tracking in data-fetch.js (data-depends field change detection)
- [x] T148 [US6] Bind data-fetch to field events in formruntime.js (focus, input, change based on data-fetch-on)
- [x] T149 [US6] Integrate data-fetch module with formruntime.js (_initDataFetch method)
- [x] T150 [P] [US6] Create ProxyController in `backend/src/FrameworkQ.Easyforms.Api/Controllers/ProxyController.cs`
- [x] T151 [US6] Implement GET /v1/proxy/fetch endpoint in ProxyController.cs (proxy external API calls)
- [x] T152 [US6] Implement endpoint allowlist validation in ProxyController.cs (only allow whitelisted URLs)
- [x] T153 [US6] Implement authentication header injection in ProxyController.cs (inject auth from server config, not client)
- [x] T154 [US6] Implement response caching in ProxyController.cs (in-memory cache with TTL)
- [x] T155 [US6] Implement error handling for external API failures in ProxyController.cs (return 502 on external errors)
- [x] T156 [P] [US6] Create cascading fetch example in `templates/examples/cascade-form-example.html` (substation → breaker dropdown)

**Checkpoint**: ✅ At this point, all 6 user stories are fully implemented and independently functional

---

## Phase 9: Form Submission and Data Persistence (Cross-Cutting)

**Purpose**: Enable saving and submitting forms (affects all stories)

- [x] T157 [P] [US-All] Create SubmissionProcessor in `backend/src/FrameworkQ.Easyforms.Runtime/SubmissionProcessor.cs`
- [x] T158 [US-All] Implement ValidateSubmission in SubmissionProcessor.cs (call ValidationEngine)
- [x] T159 [US-All] Implement SaveToDatabase in SubmissionProcessor.cs (insert into form_instances table)
- [x] T160 [US-All] Implement SaveReportingData in SubmissionProcessor.cs (insert into reporting tables from data-table widgets)
- [x] T161 [US-All] Implement transaction handling in SubmissionProcessor.cs (rollback on error)
- [x] T162 [US-All] Create SubmissionsController in `backend/src/FrameworkQ.Easyforms.Api/Controllers/SubmissionsController.cs`
- [x] T163 [US-All] Implement POST /v1/submissions endpoint in SubmissionsController.cs (submit form data)
- [x] T164 [US-All] Implement GET /v1/submissions/{instanceId} endpoint in SubmissionsController.cs (retrieve submission)
- [x] T165 [US-All] Implement PUT /v1/submissions/{instanceId} endpoint in SubmissionsController.cs (update draft submissions)
- [x] T166 [US-All] Implement DELETE /v1/submissions/{instanceId} endpoint in SubmissionsController.cs (delete draft submissions)
- [x] T167 [US-All] Implement submit() method in formruntime.js (POST to /v1/submissions)
- [x] T168 [US-All] Implement saveDraft() method in formruntime.js (POST with status=draft)
- [x] T169 [US-All] Emit formdsl:submit event in formruntime.js
- [x] T170 [US-All] Emit formdsl:submit:success event in formruntime.js
- [x] T171 [US-All] Emit formdsl:submit:error event in formruntime.js
- [x] T172 [US-All] Implement auto-save functionality in formruntime.js (optional, configurable interval)

---

## Phase 10: Query and Reporting (Cross-Cutting)

**Purpose**: Enable querying submission data and reporting tables

- [x] T173 [P] [US-All] Create QueryController in `backend/src/FrameworkQ.Easyforms.Api/Controllers/QueryController.cs`
- [x] T174 [US-All] Implement GET /v1/query/submissions endpoint in QueryController.cs (query form_instances with filters)
- [x] T175 [US-All] Add filtering support in /v1/query/submissions (formId, status, submittedBy, date range)
- [x] T176 [US-All] Add pagination support in /v1/query/submissions (page, limit)
- [x] T177 [US-All] Implement GET /v1/query/reporting/{tableName} endpoint in QueryController.cs (query reporting tables)
- [x] T178 [US-All] Add filtering support in /v1/query/reporting (instanceId, date range)
- [x] T179 [US-All] Add pagination support in /v1/query/reporting (page, limit)

---

## Phase 11: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [x] T180 [P] Add comprehensive error handling across all API controllers
- [x] T181 [P] Add structured logging to all service methods (Serilog)
- [x] T182 [P] Add correlation IDs to all API responses
- [x] T183 [P] Implement health check endpoint at /health
- [x] T184 [P] Create swagger/OpenAPI documentation generation
- [x] T185 [P] Add input sanitization for all API endpoints
- [x] T186 [P] Implement rate limiting middleware
- [x] T187 [P] Add performance monitoring (response time tracking)
- [x] T188 [P] Create deployment configuration (docker-compose.yml or similar)
- [x] T189 [P] Update README.md with architecture overview and quickstart
- [x] T190 [P] Run quickstart.md validation (follow steps end-to-end)
- [x] T191 Optimize expression evaluator performance (caching parsed expressions)
- [x] T192 Optimize database connection pooling configuration
- [x] T193 Code cleanup: Remove unused dependencies
- [x] T194 Code cleanup: Apply consistent naming conventions
- [x] T195 Security audit: Review all data-* attribute handling for XSS prevention
- [x] T196 Security audit: Review all SQL generation for injection prevention
- [x] T197 Performance test: Verify 50 concurrent user support
- [x] T198 Performance test: Verify form load time <2s for 100-field forms
- [x] T199 Create demo forms for all 6 user stories in `templates/examples/`
- [x] T200 Create video walkthrough of quickstart.md (optional)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phases 3-8)**: All depend on Foundational phase completion
  - US1 (Basic Forms): Can start after Foundational - No dependencies on other stories
  - US2 (Tables/Grids): Can start after Foundational - No dependencies on other stories (builds on US1 runtime but independently testable)
  - US3 (Validation): Can start after Foundational - No dependencies on other stories (enhances US1/US2 but independently testable)
  - US4 (Schema/DB): Can start after Foundational - No dependencies on other stories (backend-only, works with any form)
  - US5 (Print): Can start after Foundational - No dependencies on other stories (CSS-only enhancement)
  - US6 (Data Fetch): Can start after Foundational - No dependencies on other stories (runtime enhancement)
- **Submission (Phase 9)**: Depends on US1 (forms exist) - works with all stories
- **Query (Phase 10)**: Depends on US4 (database exists) and US9 (submissions exist)
- **Polish (Phase 11)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Foundation only → Can start immediately after Phase 2
- **User Story 2 (P2)**: Foundation only → Can start immediately after Phase 2 (parallel with US1)
- **User Story 3 (P3)**: Foundation only → Can start immediately after Phase 2 (parallel with US1/US2)
- **User Story 4 (P4)**: Foundation only → Can start immediately after Phase 2 (parallel with US1/US2/US3)
- **User Story 5 (P5)**: Foundation only → Can start immediately after Phase 2 (parallel with all)
- **User Story 6 (P6)**: Foundation only → Can start immediately after Phase 2 (parallel with all)

### Within Each User Story

- Models before services
- Services before controllers/UI
- Core implementation before integration
- Story complete before moving to next priority

### Parallel Opportunities

- **All Setup tasks (T001-T010)** can run in parallel
- **All Foundational tasks marked [P] (T011-T015, T019-T022)** can run in parallel within Phase 2
- **Once Foundational phase completes, all 6 user stories can start in parallel** (if team capacity allows)
- **Models within a story marked [P]** can be developed in parallel
- **Different user stories** can be worked on in parallel by different team members
- **Cross-cutting concerns in Polish phase marked [P]** can run in parallel

---

## Parallel Example: User Story 1 (MVP)

```bash
# After Foundational phase completes, these can all start together:
Task T026: "Create Field widget model"
Task T027: "Create Group widget model"
Task T028: "Create ValidationRule model"

# Once models complete, these can start together:
Task T033: "Implement Field widget parser"
Task T034: "Implement Group widget parser"
Task T040: "Create base form CSS"
Task T041: "Create jQuery runtime initialization"
```

---

## Parallel Example: All User Stories (After Foundation)

```bash
# Once Phase 2 (Foundational) completes, all stories can start:
Developer A: Phase 3 (User Story 1 - Basic Forms)
Developer B: Phase 4 (User Story 2 - Tables/Grids)
Developer C: Phase 5 (User Story 3 - Validation)
Developer D: Phase 6 (User Story 4 - Schema/DB)
Developer E: Phase 7 (User Story 5 - Print)
Developer F: Phase 8 (User Story 6 - Data Fetch)

# Each developer works independently on their story
# Stories complete and integrate without blocking each other
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (T001-T010)
2. Complete Phase 2: Foundational (T011-T025) - **CRITICAL BLOCKER**
3. Complete Phase 3: User Story 1 (T026-T049)
4. **STOP and VALIDATE**: Test User Story 1 independently
   - Create HTML form with fields, sections
   - Upload to API
   - Verify schema extraction
   - Render form with jQuery
   - Verify all US1 acceptance scenarios pass
5. Deploy/demo if ready

**Estimated tasks for MVP**: 49 tasks (T001-T049)

### Incremental Delivery

1. **Foundation** (Setup + Foundational) → T001-T025 → Foundation ready
2. **MVP Release** (US1) → Add T026-T049 → Basic forms work → Deploy/Demo
3. **Release 2** (US2 Tables) → Add T050-T079 → Tables work → Deploy/Demo
4. **Release 3** (US3 Validation) → Add T080-T101 → Validation works → Deploy/Demo
5. **Release 4** (US4 Database) → Add T102-T124 → Auto DB generation → Deploy/Demo
6. **Release 5** (US5 Print) → Add T125-T138 → Print fidelity → Deploy/Demo
7. **Release 6** (US6 Data Fetch) → Add T139-T156 → Dynamic data → Deploy/Demo
8. **Release 7** (Submission + Query) → Add T157-T179 → Data persistence → Deploy/Demo
9. **Release 8** (Polish) → Add T180-T200 → Production-ready → Deploy/Demo

Each release adds value without breaking previous functionality.

### Parallel Team Strategy

With 6 developers:

1. **Together**: Complete Setup (Phase 1) - 1-2 days
2. **Together**: Complete Foundational (Phase 2) - 3-5 days - **MUST COMPLETE BEFORE STORIES**
3. **Parallel** (once Phase 2 done):
   - Dev A: User Story 1 (T026-T049) - 3-5 days
   - Dev B: User Story 2 (T050-T079) - 4-6 days
   - Dev C: User Story 3 (T080-T101) - 3-4 days
   - Dev D: User Story 4 (T102-T124) - 4-5 days
   - Dev E: User Story 5 (T125-T138) - 2-3 days
   - Dev F: User Story 6 (T139-T156) - 3-4 days
4. **Together**: Submission/Query (Phase 9-10) - 2-3 days
5. **Parallel**: Polish (Phase 11) - 2-3 days

Stories complete and integrate independently. Full system ready in ~2-3 weeks with 6 developers.

---

## Notes

- **[P] tasks** = different files, no dependencies, can run in parallel
- **[Story] label** maps task to specific user story for traceability
- **Each user story should be independently completable and testable**
- **Foundation phase MUST complete before ANY user story work begins**
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- **Tests are NOT included** - can be added later if explicitly requested
- All file paths are absolute and map to project structure in plan.md

---

## Task Summary

- **Total Tasks**: 200
- **Setup Phase**: 10 tasks (T001-T010)
- **Foundational Phase**: 15 tasks (T011-T025) - **BLOCKS ALL STORIES**
- **User Story 1 (Basic Forms)**: 24 tasks (T026-T049) - **MVP**
- **User Story 2 (Tables/Grids)**: 30 tasks (T050-T079)
- **User Story 3 (Validation)**: 22 tasks (T080-T101)
- **User Story 4 (Schema/DB)**: 23 tasks (T102-T124)
- **User Story 5 (Print)**: 14 tasks (T125-T138)
- **User Story 6 (Data Fetch)**: 18 tasks (T139-T156)
- **Submission Phase**: 16 tasks (T157-T172)
- **Query Phase**: 7 tasks (T173-T179)
- **Polish Phase**: 21 tasks (T180-T200)

**MVP Scope (US1 only)**: 49 tasks
**Parallel Opportunities**: 60+ tasks can run in parallel across different phases
**Independent Stories**: All 6 user stories are independently testable and deliverable
