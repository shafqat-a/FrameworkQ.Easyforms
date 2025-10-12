# Feature Specification: HTMLDSL Form System

**Feature Branch**: `001-spec-in-in`
**Created**: 2025-10-11
**Status**: Draft
**Input**: User description: "spec in in the docs folder"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Basic Form Creation (Priority: P1)

Form designers need to create structured data collection forms using familiar HTML and CSS syntax, with semantic annotations for field types, labels, and validation rules.

**Why this priority**: This is the core capability that enables all other features. Without the ability to create basic forms with fields and sections, no other functionality is possible.

**Independent Test**: Can be fully tested by creating an HTML document with a form containing basic input fields, labels, and sections using data-* attributes, and verifying the form renders correctly in a browser. Delivers immediate value by allowing form creation without specialized tools.

**Acceptance Scenarios**:

1. **Given** a form designer has HTML/CSS knowledge, **When** they create a new HTML document with a `<form data-form>` element containing input fields with name attributes and data-* annotations, **Then** the form structure is valid and can be parsed
2. **Given** a form with multiple pages and sections defined using `data-page` and `data-section` attributes, **When** the designer views the form in a browser, **Then** all pages and sections are visually organized according to the HTML/CSS layout
3. **Given** a form with various field types (text, number, date, select), **When** the designer annotates fields with appropriate `data-type` attributes, **Then** each field displays appropriate input controls
4. **Given** a form with required fields marked using `data-required` or native `required` attributes, **When** the form is rendered, **Then** required fields are visually indicated to users

---

### User Story 2 - Complex Table and Grid Support (Priority: P2)

Form designers need to create data collection tables with multiple rows, columns, calculated fields, aggregations, and complex header structures to match existing paper forms.

**Why this priority**: Many real-world forms (log sheets, performance reports, rosters) require tabular data entry with calculations. This is essential for replacing paper-based workflows but builds on the basic form creation capability.

**Independent Test**: Can be tested independently by creating a form containing only a table widget with row templates, column definitions, formulas, and aggregate calculations. Delivers value for table-based data collection use cases.

**Acceptance Scenarios**:

1. **Given** a form designer needs a data entry table, **When** they create a `<table data-table>` with column headers using `<th data-col>` and a row template with matching inputs, **Then** users can add and remove rows dynamically
2. **Given** a table with calculated columns defined via `data-formula` attributes, **When** users enter data in input columns, **Then** calculated column values update automatically
3. **Given** a table with aggregate calculations in `<tfoot>` using `data-agg` attributes, **When** users enter data in rows, **Then** aggregate totals (sum, avg, min, max, count) compute correctly
4. **Given** a table with complex multi-row headers using `colspan` and `rowspan`, **When** the form is rendered, **Then** header structure matches the intended visual layout
5. **Given** a grid with generated columns (e.g., days of month, time slots), **When** the designer specifies `data-col-gen` attributes, **Then** columns are automatically generated based on the specification

---

### User Story 3 - Runtime Validation and Conditional Logic (Priority: P3)

Form users need real-time validation feedback, conditional field visibility, and cross-field constraints to prevent data entry errors and guide them through complex forms.

**Why this priority**: Enhances user experience and data quality but requires basic form creation and data entry to be working first. Essential for production use but not for initial prototyping.

**Independent Test**: Can be tested by creating a form with validation rules and conditional logic, then interacting with fields and verifying validation messages appear and fields show/hide based on conditions. Delivers value by reducing data entry errors.

**Acceptance Scenarios**:

1. **Given** fields with validation rules defined via native attributes (required, pattern, min, max) and data-* mirrors, **When** users enter invalid data, **Then** validation messages appear immediately or on blur/change
2. **Given** fields with conditional visibility rules via `data-when` attributes, **When** dependent field values change, **Then** conditional fields show or hide appropriately
3. **Given** a form with cross-field constraints defined via `data-constraint` on containers, **When** users enter data that violates constraints, **Then** constraint error messages are displayed
4. **Given** fields with conditional required rules via `data-required-when`, **When** the condition becomes true, **Then** the field is marked required and validated accordingly
5. **Given** a table with row-level validation constraints, **When** users enter data that violates row constraints, **Then** the entire row is highlighted with the appropriate error message

---

### User Story 4 - Schema Extraction and Database Generation (Priority: P4)

Developers and system administrators need to automatically extract a structured schema from HTML forms and generate database tables for storing submitted form data.

**Why this priority**: Critical for production deployment and data persistence but depends on forms being properly authored with semantic annotations. This enables the backend infrastructure.

**Independent Test**: Can be tested by running a schema extraction tool against a complete HTML form and verifying the output JSON schema accurately represents all form elements, then generating SQL DDL and creating database tables. Delivers value by automating database setup.

**Acceptance Scenarios**:

1. **Given** a valid HTML form with proper data-* annotations, **When** the schema extraction process runs, **Then** a canonical JSON model is produced containing form metadata, pages, sections, and widget definitions
2. **Given** a form with data tables defined via `<table data-table>`, **When** SQL DDL generation runs, **Then** reporting tables are created with appropriate columns, data types, constraints, and indexes
3. **Given** a form with calculated fields defined via `data-formula`, **When** database tables are generated, **Then** computed columns are created using database-native generated column syntax
4. **Given** a form with validation rules (required, min, max, pattern, enum), **When** database tables are generated, **Then** appropriate constraints (NOT NULL, CHECK, unique indexes) are created
5. **Given** a form with custom indexes defined via `data-indexes`, **When** database generation runs, **Then** specified indexes (single and composite) are created on reporting tables

---

### User Story 5 - Print Fidelity and Pixel-Perfect Layout (Priority: P5)

Form designers and users need forms to print with exact fidelity to original paper forms, including pagination, column widths, headers/footers, and multi-row header structures.

**Why this priority**: Important for organizations transitioning from paper forms and needing exact visual matches, but not required for digital-only workflows. Lowest priority for MVP but essential for certain industries.

**Independent Test**: Can be tested by creating a form with print-specific annotations (page size, margins, column widths), printing to PDF, and comparing against the original paper form. Delivers value for paper form replacement scenarios.

**Acceptance Scenarios**:

1. **Given** a form with print attributes (data-print-page-size, data-print-margins-mm, data-print-orientation), **When** the form is printed or exported to PDF, **Then** the output matches specified page dimensions, margins, and orientation
2. **Given** a table with column width specifications via `data-width` on headers, **When** the form is printed, **Then** column widths match the specified measurements
3. **Given** a table with repeating header rows via `data-print-repeat-head-rows`, **When** the table spans multiple pages, **Then** headers repeat on each page
4. **Given** sections with page break controls (data-pagebreak, data-keep-together), **When** the form is printed, **Then** page breaks occur at specified locations and sections remain unbroken
5. **Given** a form with page counters (data-page-counter, data-page-total), **When** printed, **Then** each page displays correct "page X of Y" information

---

### User Story 6 - Dynamic Data Sources and Cascading Lookups (Priority: P6)

Form users need dropdown fields and autocomplete inputs that fetch options from external APIs, with support for cascading selections based on parent field values.

**Why this priority**: Enhances user experience for forms with large option sets or dependent selections, but requires backend API infrastructure. Nice-to-have for MVP, essential for production scalability.

**Independent Test**: Can be tested by creating fields with `data-fetch` attributes pointing to mock APIs, then interacting with fields and verifying options are loaded dynamically and cascade correctly. Delivers value by enabling large option sets without embedding in HTML.

**Acceptance Scenarios**:

1. **Given** a select field with `data-fetch` pointing to an API endpoint, **When** the field receives focus or user types, **Then** options are fetched from the API and populated
2. **Given** a field with `data-depends` referencing parent fields, **When** parent field values change, **Then** the dependent field's options reload based on new parent values
3. **Given** a field with `data-fetch-cache` settings, **When** the same options are requested multiple times, **Then** cached results are used within the specified TTL to reduce API calls
4. **Given** a field with URL token substitution (e.g., `{substation}`, `{search}`), **When** the fetch occurs, **Then** tokens are replaced with current field values before making the request
5. **Given** a field with `data-fetch-map` for response field mapping, **When** API returns data in a custom format, **Then** response fields are correctly mapped to option value/label pairs

---

### Edge Cases

- What happens when a form has deeply nested conditional logic that creates circular dependencies?
- How does the system handle forms with hundreds of fields or very large tables (performance limits)?
- What happens when schema extraction encounters malformed HTML or conflicting data-* attributes?
- How does the system handle validation when offline or when external validation APIs are unavailable?
- What happens when a form definition changes (versioning) after data has been submitted against an older version?
- How are forms with mixed Bengali/English text and right-to-left languages handled in both display and print?
- What happens when calculated formulas reference fields that don't exist or have invalid expressions?
- How does the system handle table row addition/deletion when aggregate calculations depend on row counts?
- What happens when print specifications exceed printer capabilities or page size limits?
- How are signature blocks handled when users lack drawing input devices or file upload capabilities?

## Requirements *(mandatory)*

### Functional Requirements

#### Form Authoring and Structure

- **FR-001**: System MUST support HTML `<form>` elements as the root container with required `data-form` attribute for unique form identification
- **FR-002**: System MUST support form metadata attributes including `data-title`, `data-version`, `data-locale`, `data-store`, and `data-tags`
- **FR-003**: System MUST support page containers using any block element with `data-page` attribute and optional `data-title`
- **FR-004**: System MUST support section containers using any block element with `data-section` attribute with optional title, numbering, level, collapsible, and collapsed attributes
- **FR-005**: System MUST enforce naming conventions for all IDs and field names using the pattern `^[a-z0-9_-]+$`

#### Widget Types and Input Controls

- **FR-006**: System MUST support native HTML input elements (input, select, textarea) as field widgets with semantic `data-type` annotations (string, text, integer, decimal, date, time, datetime, bool, enum, attachment, signature)
- **FR-007**: System MUST support field groups with layout options via `data-group` and `data-layout` attributes (columns:N, table)
- **FR-008**: System MUST support data entry tables via `<table data-table>` with column definitions in headers, row templates, and configurable row modes (infinite, finite)
- **FR-009**: System MUST support 2D grids via `<table data-grid>` with declarative row and column generation options
- **FR-010**: System MUST support checklist widgets with item containers and checkbox/select inputs
- **FR-011**: System MUST support signature widgets with configurable signature types (draw, upload, both) and associated fields (name, designation, date)
- **FR-012**: System MUST support form header widgets for document metadata display (document number, revision, effective date, organization)
- **FR-013**: System MUST support note/aside widgets with configurable styles (info, warning, note)
- **FR-014**: System MUST support radio button groups with orientation options (horizontal, vertical, grid)
- **FR-015**: System MUST support checkbox groups with minimum and maximum selection constraints
- **FR-016**: System MUST support hierarchical checklists with nested structure and configurable numbering schemes

#### Validation and Constraints

- **FR-017**: System MUST support native HTML validation attributes (required, pattern, min, max, step, minlength, maxlength) and mirror them with data-* equivalents
- **FR-018**: System MUST support conditional required fields via `data-required-when` with expression evaluation
- **FR-019**: System MUST support conditional enable/disable via `data-enable-when` with expression evaluation
- **FR-020**: System MUST support cross-field and row-level constraints via `data-constraint` attributes on containers with custom error messages
- **FR-021**: System MUST support uniqueness constraints within tables via `data-unique-by` and across forms via `data-unique-scope`
- **FR-022**: System MUST support enumeration constraints via `data-enum` with optional label mappings
- **FR-023**: System MUST support custom error messages per validation type (data-error-required, data-error-pattern, data-error-min, data-error-max)
- **FR-024**: System MUST support validation timing configuration via `data-validate-on` (input, change, blur, submit, save)
- **FR-025**: System MUST support remote/async validation via `data-validate-url` with debouncing and response parsing

#### Expressions and Calculations

- **FR-026**: System MUST support field formulas via `data-compute` attributes with expression syntax supporting arithmetic operators and field references
- **FR-027**: System MUST support conditional visibility via `data-when` attributes with expression evaluation on any container or widget
- **FR-028**: System MUST support table aggregate calculations via `data-agg` attributes (sum, avg, min, max, count) with target field binding
- **FR-029**: System MUST support expression context where table row expressions reference column names and global expressions reference field names
- **FR-030**: System MUST support expression functions including sum, avg, min, max, count, round, abs, ceil, floor
- **FR-031**: System MUST mark computed fields as readonly and recalculate automatically when dependent values change

#### Data Sources and Dynamic Content

- **FR-032**: System MUST support external data fetching via `data-fetch` attributes with HTTP method and URL specification
- **FR-033**: System MUST support fetch triggers via `data-fetch-on` (load, focus, input, change) with configurable debouncing
- **FR-034**: System MUST support URL token substitution in fetch URLs using field values (e.g., `{fieldName}`, `{search}`)
- **FR-035**: System MUST support response field mapping via `data-fetch-map` to adapt arbitrary API response structures
- **FR-036**: System MUST support fetch caching via `data-fetch-cache` with TTL, session, or no-cache options
- **FR-037**: System MUST support cascading selects via `data-depends` to reload options when parent fields change
- **FR-038**: System MUST support fallback options via inline JSON in `data-options` or script tag references

#### Schema Extraction and Metadata

- **FR-039**: System MUST extract canonical JSON schema from HTML forms including form metadata, pages, sections, and all widget definitions
- **FR-040**: System MUST capture field metadata including name, type, label, required, readonly, default, validation rules, formulas, and conditional logic
- **FR-041**: System MUST capture table metadata including columns, row mode, min/max rows, aggregates, and row templates
- **FR-042**: System MUST capture grid metadata including row and column generation rules, cell types, and constraints
- **FR-043**: System MUST extract validation rules from both native HTML attributes and data-* mirrors into schema
- **FR-044**: System MUST extract expression logic (formulas, conditionals, aggregates) into schema with dependency graphs

#### Database Generation and Mapping

- **FR-045**: System MUST generate SQL DDL for reporting tables based on `data-table` and `data-grid` widgets
- **FR-046**: System MUST map data types to appropriate relational database types including variable-length text, long text, integers, decimal numbers, dates, times, timestamps, and boolean values for supported database systems
- **FR-047**: System MUST generate standard columns for reporting tables including instance_id, page_id, section_id, widget_id, row key, and recorded_at
- **FR-048**: System MUST generate computed columns in database using native generated column syntax when `data-formula` is present
- **FR-049**: System MUST generate database constraints (NOT NULL, CHECK, unique indexes) based on validation rules
- **FR-050**: System MUST generate custom indexes based on `data-indexes` specifications (single and composite)
- **FR-051**: System MUST support natural key definitions via `data-row-key` for enforcing row uniqueness
- **FR-052**: System MUST support header field replication via `data-copy-header` to reporting tables for efficient filtering

#### Print and Pagination

- **FR-053**: System MUST support print configuration at form level including page size, margins, orientation, and scale via data-print-* attributes
- **FR-054**: System MUST support page break controls (before, after, avoid) via `data-pagebreak` on sections and elements
- **FR-055**: System MUST support keep-together and keep-with-next controls to prevent breaking sections and maintain element grouping
- **FR-056**: System MUST support repeating table headers across pages via `data-print-repeat-head-rows`
- **FR-057**: System MUST support column width specifications via `data-width` on table headers for print layout control
- **FR-058**: System MUST support cell alignment and wrapping controls via `data-align`, `data-valign`, and `data-wrap`
- **FR-059**: System MUST support page counters via `data-page-counter` and `data-page-total` that populate at print time

#### Table Row and Column Generation

- **FR-060**: System MUST support declarative row generation via `data-row-gen` with time ranges, numeric ranges, and explicit value lists
- **FR-061**: System MUST support declarative column generation via `data-col-gen` for days-of-month, time slots, and explicit codes
- **FR-062**: System MUST support context-aware column generation using field values or date specifications (e.g., `data-context-month`)

#### Internationalization and Accessibility

- **FR-063**: System MUST support multi-locale forms via `data-locale` attribute with language-specific label variants
- **FR-064**: System MUST support proper label association using native `for`/`id` or `aria-labelledby` attributes
- **FR-065**: System MUST use semantic HTML elements (section, table, thead, tbody, tfoot) for accessibility

#### Runtime Behavior

- **FR-066**: System MUST provide runtime initialization via a mount function that takes a form element
- **FR-067**: System MUST evaluate `data-compute` expressions on field changes and lock computed fields as readonly
- **FR-068**: System MUST manage dynamic row addition/removal for tables with `data-row-mode="infinite"`
- **FR-069**: System MUST calculate aggregate values into target elements specified by `data-agg` and `data-target`
- **FR-070**: System MUST enforce conditional visibility by toggling elements based on `data-when` expressions
- **FR-071**: System MUST enforce native validation plus custom rules (min/max selections, cross-field constraints)
- **FR-072**: System MUST serialize form values for save/submit operations
- **FR-073**: System MUST emit custom events for form lifecycle (formdsl:ready, formdsl:change, formdsl:submit)
- **FR-074**: System MUST mark invalid fields with `aria-invalid="true"` and `.is-invalid` class with visible error messages

#### Security and Sanitization

- **FR-075**: System MUST strictly prohibit all inline scripts in form templates and use an allowlist approach that permits only form-related HTML tags (form, input, select, textarea, label, section, div, table, thead, tbody, tfoot, tr, th, td, ol, ul, li, aside, canvas) and data-* attributes for semantic annotations
- **FR-076**: System MUST NOT expose authentication secrets or tokens in HTML markup attributes
- **FR-077**: System MUST inject authentication headers via runtime configuration, not embedded in data-fetch URLs

#### Storage and Versioning

- **FR-078**: System MUST store form definitions as HTML with extracted JSON model for versioning and diffs
- **FR-079**: System MUST store submissions with instance_id, form_id, version, timestamp, user_id, and raw data
- **FR-080**: System MUST support automatic migration of existing submission data when form definitions change, using transformation rules to map old field structures to new field structures, with rollback capability if migration fails

### Key Entities *(include if feature involves data)*

- **Form**: Root container representing a complete form with unique ID, title, version, pages, and metadata (locale, storage mode, tags)
- **Page**: Container within a form representing a logical page with ID, title, and collection of sections
- **Section**: Container within a page representing a logical grouping with ID, title, numbering scheme, level, collapsibility state
- **Widget**: Abstract base for all interactive elements; specific types include Field, Group, Table, Grid, Checklist, Signature, FormHeader, Notes, RadioGroup, CheckboxGroup, HierarchicalChecklist
- **Field**: Single input element with name, data type, label, validation rules, default value, formula, conditional logic, fetch configuration
- **Table**: Row-based data collection widget with column definitions, row template, row mode (infinite/finite), min/max rows, aggregates, print configuration
- **Grid**: 2D matrix widget with row and column generation rules, cell type, validation rules
- **Column**: Table column definition with name, data type, label, validation rules, formula, format, aggregation function
- **Validation Rule**: Constraint on field or container with type (required, pattern, min, max, unique, constraint), condition, error message, severity
- **Expression**: Formula or conditional logic string with field references, operators, and functions; evaluated in specific context (field, row, form)
- **Schema**: Extracted JSON model of form structure including all metadata, widgets, validation rules, and expressions; used for versioning and database generation
- **Submission**: Instance of form data submitted by a user with instance ID, form ID/version, timestamp, user ID, header context, raw data, and normalized table data
- **Reporting Table**: Database table generated from Table or Grid widget with columns for instance ID, page/section/widget IDs, row key, timestamp, data columns, computed columns, constraints, and indexes

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Form designers can create a basic form with 10-20 fields organized into sections in under 30 minutes using HTML knowledge
- **SC-002**: Schema extraction from a form with 50 fields and 3 tables completes in under 5 seconds and produces valid JSON
- **SC-003**: Database table generation from extracted schema completes in under 10 seconds for forms with up to 10 tables
- **SC-004**: Forms with up to 100 fields and 5 tables load and become interactive in under 2 seconds in modern browsers
- **SC-005**: Calculated fields and aggregations update within 100 milliseconds of dependent value changes
- **SC-006**: Validation feedback appears within 300 milliseconds of user input or blur events
- **SC-007**: Forms with complex layouts print to PDF with 95% or better visual fidelity compared to original paper forms (measured by pixel difference)
- **SC-008**: Column widths in printed output match specifications within 2% tolerance
- **SC-009**: Users can successfully submit valid form data with zero validation errors on first attempt 80% of the time after initial learning period
- **SC-010**: Data extraction error rate is less than 1% for forms with proper semantic annotations
- **SC-011**: Runtime formula evaluation accuracy is 100% for standard arithmetic expressions with no precision loss for decimal calculations
- **SC-012**: System supports forms with up to 500 total fields and 20 tables without performance degradation
- **SC-013**: External API data fetching completes within 1 second for 95% of requests
- **SC-014**: Forms are accessible and operable via keyboard navigation for all interactive elements
- **SC-015**: Generated database constraints prevent invalid data insertion with 100% accuracy
- **SC-016**: Table row addition/removal operations complete within 100 milliseconds for tables with up to 1000 rows
- **SC-017**: Conditional visibility logic evaluates correctly with 100% accuracy for all supported expression types
- **SC-018**: Form definitions remain backward compatible across minor version updates with zero breaking changes
- **SC-019**: Multi-locale forms correctly display labels in specified languages with no character encoding issues
- **SC-020**: System supports up to 50 concurrent users actively filling out the same form instance without performance degradation (response time under 2 seconds)

## Assumptions

- Form designers have working knowledge of HTML, CSS, and basic data structures
- Target runtime environment is modern web browsers with current web standards support
- Backend database systems are enterprise-grade relational databases with support for generated/computed columns
- Forms are primarily displayed on desktop or tablet devices with adequate screen size (mobile support is not a primary requirement)
- Users have stable internet connectivity for external data fetching and validation (offline support is not required for MVP)
- Authentication and authorization are handled by the containing application, not by the form system itself
- File uploads for attachments and signatures are handled by separate file storage infrastructure
- Form definitions are managed by trusted users (designers/developers) with appropriate access controls
- The system is deployed in an intranet environment where API endpoints are accessible without complex CORS configurations
- Print output is generated through browser print or PDF export, not through specialized PDF generation libraries
- Database schema migrations for reporting tables are handled through standard migration tools
- Form submission data volume is moderate (thousands to hundreds of thousands of submissions per form, not millions)
- Complex calculations and aggregations in forms are arithmetic in nature and do not require advanced mathematical or statistical functions
- The system will be used primarily for business forms (log sheets, checklists, reports) rather than public surveys or marketing forms
- Network latency for external API calls is reasonable (under 500ms average) for acceptable user experience
- Bengali text rendering is supported by standard web fonts available in the target environment
- The system does not need to support real-time collaborative editing of forms by multiple users simultaneously
- Form version changes are infrequent (monthly or quarterly) rather than continuous
