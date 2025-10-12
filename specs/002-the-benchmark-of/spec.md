# Feature Specification: Benchmark Forms Conversion and Validation

**Feature Branch**: `002-the-benchmark-of`
**Created**: 2025-10-12
**Status**: Draft
**Input**: User description: "The benchmark of the system is if the 6 forms in the docs/benchmark can be converted into html properly and run with the system properly to visualize as is."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Form Conversion from Paper to HTML (Priority: P1)

Form developers need to convert the 6 existing paper-based benchmark forms (QF-GMD-06, QF-GMD-14, QF-GMD-01, QF-GMD-17, QF-GMD-19, QF-GMD-22) from Power Grid Company of Bangladesh into HTMLDSL format, preserving exact visual layout, structure, and functionality.

**Why this priority**: This validates that the HTMLDSL system can handle real-world, complex enterprise forms. If these 6 benchmark forms can be successfully converted, it proves the system is production-ready for similar use cases.

**Independent Test**: Take each paper form screenshot → Create HTML version with data-* attributes → Upload to system → Compare rendered output with original screenshot → Verify 95%+ visual match and all fields functional.

**Acceptance Scenarios**:

1. **Given** the QF-GMD-06 form screenshot (Consolidated Performance Report), **When** a developer creates HTML with multi-row header tables, calculated totals, and signature blocks, **Then** the rendered form matches the original layout with all calculation fields working
2. **Given** the QF-GMD-14 form screenshot (Monthly Shift Duty Roster), **When** a developer creates HTML with days-of-month grid (1-31 columns) and shift code enums (A/B/C/G/F/Ad), **Then** the grid generates correctly for any month and shift codes are selectable
3. **Given** the QF-GMD-01 form screenshot (Log Sheet), **When** a developer creates HTML with extremely wide multi-row headers and hourly time-axis rows, **Then** the table spans properly and prints across pages with repeating headers
4. **Given** the QF-GMD-17 form screenshot (Surveillance Visit Checklist), **When** a developer creates HTML with hierarchical numbered checklist (decimal numbering 1.0, 1.1, 2.0, etc.), **Then** the checklist structure renders with proper nesting and observation fields
5. **Given** the QF-GMD-19 form screenshot (Daily Inspection in Bengali), **When** a developer creates HTML with Bengali text labels and bilingual content, **Then** the form displays Bengali characters correctly and maintains table structure
6. **Given** the QF-GMD-22 form screenshot (Transformer Inspection), **When** a developer creates HTML with complex nested checkbox tables and condition fields, **Then** all checkboxes function and nested table structure is preserved

---

### User Story 2 - Visual Fidelity Verification (Priority: P2)

Quality assurance teams need to verify that all 6 converted HTML forms render with pixel-perfect fidelity to the original paper forms, including headers, tables, fonts, spacing, and signature blocks.

**Why this priority**: Visual fidelity is critical for regulatory compliance and user acceptance in replacing paper forms. Forms must look identical to avoid retraining staff.

**Independent Test**: Open each converted HTML form in browser → Compare side-by-side with original screenshot → Measure visual differences → Verify <5% deviation in layout, spacing, and structure.

**Acceptance Scenarios**:

1. **Given** converted HTML forms, **When** rendered in browser at 100% zoom, **Then** form headers match original size, spacing, and alignment within 2% tolerance
2. **Given** converted table-based forms, **When** viewed on screen, **Then** column widths, row heights, and cell borders match original proportions
3. **Given** forms with signature blocks, **When** rendered, **Then** signature lines, labels, and spacing match original positions
4. **Given** forms with Bengali text, **When** displayed, **Then** Bengali characters render correctly with proper fonts and no encoding issues
5. **Given** all 6 forms, **When** printed to PDF, **Then** output matches original paper form layout with 95%+ visual fidelity (measured by pixel comparison or visual inspection)

---

### User Story 3 - Print Output Validation (Priority: P3)

Users need to print all 6 converted forms to PDF and verify they match the original paper forms for archival and regulatory purposes.

**Why this priority**: Print output validation ensures forms can replace paper versions for official records, audits, and compliance documentation.

**Independent Test**: Print each converted HTML form to PDF → Compare with original paper form → Verify page breaks, headers repeat, column widths exact, signatures preserved.

**Acceptance Scenarios**:

1. **Given** QF-GMD-01 (Log Sheet with wide table), **When** printed to PDF, **Then** table headers repeat on each page and column widths match original specifications
2. **Given** QF-GMD-14 (Monthly Roster with 31 columns), **When** printed in landscape orientation, **Then** all 31 day columns fit on single page without wrapping
3. **Given** forms with multi-page content, **When** printed, **Then** page breaks occur at appropriate locations and content is not split mid-table-row
4. **Given** forms with signature blocks, **When** printed, **Then** signature lines and approval sections appear at correct positions on the page
5. **Given** all 6 forms, **When** printed at actual size (100% scale), **Then** forms fit on A4 or Letter paper as originally designed

---

### User Story 4 - Functional Validation (Priority: P4)

Form users need all interactive elements (calculated fields, aggregates, enums, hierarchical numbering) to function correctly in the converted HTML forms.

**Why this priority**: Functional correctness ensures data integrity and reduces manual calculation errors when forms are used in production.

**Independent Test**: Open each form → Enter test data → Verify calculated columns auto-compute → Verify aggregates sum correctly → Verify enum constraints enforced → Verify hierarchical numbering displays.

**Acceptance Scenarios**:

1. **Given** QF-GMD-06 with calculated "Total" column (Forced + Scheduled), **When** user enters values in Forced and Scheduled columns, **Then** Total column auto-calculates correctly
2. **Given** QF-GMD-01 Log Sheet with transformer measurements, **When** user enters voltage and current values, **Then** power (MW) calculates as voltage * current / 1000
3. **Given** QF-GMD-14 Roster with shift codes, **When** user selects shift code from dropdown (A/B/C/G/F/Ad), **Then** only valid codes are selectable and persist on save
4. **Given** QF-GMD-17 Surveillance Checklist, **When** user completes hierarchical items, **Then** numbering displays correctly (1.0, 1.1, 1.2, 2.0, etc.)
5. **Given** forms with aggregate calculations, **When** multiple rows entered, **Then** footer totals (sum, average) update dynamically

---

### User Story 5 - Schema Extraction Validation (Priority: P5)

System administrators need to verify that schema extraction works correctly for all 6 benchmark forms, producing accurate JSON schemas and SQL DDL.

**Why this priority**: Schema extraction accuracy ensures database tables are created correctly for storing form submission data.

**Independent Test**: Upload each converted HTML form → Extract schema JSON → Generate SQL DDL → Verify schema includes all fields, correct data types, calculated columns, constraints.

**Acceptance Scenarios**:

1. **Given** all 6 converted forms uploaded to system, **When** schema extraction runs, **Then** JSON schemas are produced with no parsing errors
2. **Given** extracted schemas, **When** SQL DDL generation runs, **Then** reporting tables are created with correct column names, data types, and computed column formulas
3. **Given** forms with complex table structures, **When** schema extracted, **Then** all columns captured including multi-row header groupings
4. **Given** forms with calculated fields, **When** DDL generated, **Then** computed columns use database-native syntax (PERSISTED for SQL Server, GENERATED ALWAYS AS for PostgreSQL)
5. **Given** forms with hierarchical structures, **When** schema extracted, **Then** nested item relationships are preserved in JSON

---

### User Story 6 - End-to-End Workflow Validation (Priority: P6)

End users need to complete the full workflow (view form, fill data, submit, query results) for all 6 benchmark forms to validate production readiness.

**Why this priority**: End-to-end validation confirms all system components work together for complete business workflows.

**Independent Test**: For each form: Open in browser → Fill with sample data → Submit → Query submission → Verify data stored correctly and retrievable.

**Acceptance Scenarios**:

1. **Given** all 6 forms available in system, **When** user opens any form in browser, **Then** form loads in under 2 seconds and all fields are interactive
2. **Given** user fills out QF-GMD-06 performance report, **When** user clicks submit, **Then** data is saved with all calculated values and aggregates
3. **Given** user fills out QF-GMD-14 roster for a month, **When** submitted, **Then** all 31 day columns with shift assignments are stored in reporting table
4. **Given** submitted forms, **When** user queries submissions by form ID, **Then** all submissions are returned with correct metadata (date, status, user)
5. **Given** submitted forms with table data, **When** user queries reporting tables, **Then** row-level data is retrievable with all calculated columns populated

---

### Edge Cases

- How are forms with extremely wide tables (30+ columns) handled on narrow screens?
- What happens when Bengali font is not available on user's system?
- How are multi-page forms with page breaks handled when displayed on screen vs. print?
- What happens when calculated column formulas reference non-existent fields in malformed HTML?
- How are signature blocks handled when users need to draw or upload signatures?
- What happens when day-of-month grid is generated for February (28/29 days) vs. months with 30/31 days?
- How are hierarchical checklist item numbering conflicts resolved if manually specified?
- What happens when forms have merged cells with complex colspan/rowspan that don't align properly?
- How are enum validation errors displayed when invalid shift codes are entered?
- What happens when print page size is insufficient for very wide tables?

## Requirements *(mandatory)*

### Functional Requirements

#### Form Conversion and HTML Creation

- **FR-001**: System MUST support conversion of QF-GMD-06 (Consolidated Performance Report) with multi-section tables, calculated total columns, and availability percentage fields
- **FR-002**: System MUST support conversion of QF-GMD-14 (Monthly Shift Duty Roster) with 31-column day-of-month grid and shift code enums (A, B, C, G, F, Ad)
- **FR-003**: System MUST support conversion of QF-GMD-01 (Log Sheet) with extremely wide multi-row headers, multiple transformer column groups, and hourly time-axis rows
- **FR-004**: System MUST support conversion of QF-GMD-17 (Surveillance Visit Checklist) with hierarchical decimal numbering (1.0, 1.1, 2.0, etc.) and observation enums (Good/Acceptable/Poor, Yes/No, Healthy/Defective)
- **FR-005**: System MUST support conversion of QF-GMD-19 (Daily Inspection in Bengali) with bilingual labels, Bengali text rendering, and mixed language content
- **FR-006**: System MUST support conversion of QF-GMD-22 (Transformer Inspection) with nested checkbox tables, condition/observation fields, and complex hierarchical structure

#### Visual Fidelity Requirements

- **FR-007**: Converted forms MUST preserve original header structure including document number, revision number, effective date, and page numbering
- **FR-008**: Converted forms MUST preserve table structures including merged cells, column groups, and multi-row headers
- **FR-009**: Converted forms MUST preserve exact column widths specified in original forms for proper alignment
- **FR-010**: Converted forms MUST preserve signature blocks with labeled signature lines for reviewers and approvers
- **FR-011**: Converted forms MUST render Bengali text correctly using appropriate fonts (SutonnyMJ, Nikosh, or web-safe Bengali fonts)
- **FR-012**: Converted forms MUST display form headers, titles, and organizational branding matching original layout

#### Calculation and Formula Requirements

- **FR-013**: System MUST correctly implement calculated "Total" columns as sum of "Forced" and "Scheduled" interruptions (QF-GMD-06)
- **FR-014**: System MUST correctly implement power (MW) calculation as voltage (kV) * current (A) / 1000 (QF-GMD-01)
- **FR-015**: System MUST correctly implement availability percentage calculations from interruption duration and capacity data
- **FR-016**: System MUST correctly compute aggregate totals (sum) for energy interruption columns (MkWh)
- **FR-017**: System MUST correctly compute aggregate averages for performance metrics

#### Grid and Column Generation

- **FR-018**: System MUST generate 1-31 day columns for monthly roster based on actual days in the specified month (QF-GMD-14)
- **FR-019**: System MUST generate hourly time slots (7:00-22:00 or 24-hour) for log sheet time-axis rows (QF-GMD-01)
- **FR-020**: System MUST support shift code enum values (A, B, C, G, F, Ad) with proper validation
- **FR-021**: System MUST support observation enum values (Good/Acceptable/Poor, Yes/No, Healthy/Defective, Clean/Not Cleaned, OK/Cracked, etc.)

#### Hierarchical and Structured Content

- **FR-022**: System MUST support hierarchical checklist numbering with automatic decimal numbering (1.0, 1.1, 1.2, 2.0, 2.1, etc.) for QF-GMD-17
- **FR-023**: System MUST support nested item structures with parent-child relationships in checklists
- **FR-024**: System MUST support mixed hierarchical and tabular content within same form section

#### Print and Export Requirements

- **FR-025**: Converted forms MUST print to PDF maintaining exact layout and proportions
- **FR-026**: Multi-page forms MUST implement proper page breaks to avoid splitting table rows
- **FR-027**: Forms with wide tables MUST support landscape orientation printing
- **FR-028**: Table headers MUST repeat on each printed page for multi-page tables
- **FR-029**: Signature blocks MUST appear at correct positions on printed output (typically end of form)
- **FR-030**: Page counters ("Page X of Y") MUST display correctly on printed forms

#### Internationalization Requirements

- **FR-031**: System MUST support Bengali (Bangla) text rendering for labels, headings, and instructions
- **FR-032**: System MUST support mixed English and Bengali content within same form
- **FR-033**: System MUST preserve text directionality for Bengali content (left-to-right for Bengali, same as English)
- **FR-034**: System MUST use web-safe fonts that support Bengali Unicode characters

#### Signature and Approval Requirements

- **FR-035**: Converted forms MUST include signature blocks with labels for specific roles (Reviewed by GMT-1, Approved by DT, Assistant Manager, Deputy Manager, Manager)
- **FR-036**: Signature blocks MUST support handwritten signature capture or upload
- **FR-037**: Signature blocks MUST include date fields for signature date/time
- **FR-038**: Forms MUST support multiple signature blocks for different approval levels

#### Data Validation and Constraints

- **FR-039**: System MUST enforce enum constraints for shift codes, observation values, and condition selections
- **FR-040**: System MUST validate calculated fields match expected formulas before submission
- **FR-041**: System MUST validate required fields are completed before allowing form submission
- **FR-042**: System MUST validate data types (numbers for capacity/measurements, dates for inspection dates, text for remarks)

### Key Entities *(include if feature involves data)*

- **Benchmark Form**: One of the 6 reference forms (QF-GMD-06, -14, -01, -17, -19, -22) from Power Grid Company with original layout, structure, fields, calculations
- **Form Conversion**: Process of transforming paper form screenshot into HTMLDSL markup with data-* attributes
- **Visual Fidelity Metric**: Percentage match between rendered HTML form and original paper form (target: 95%+)
- **Benchmark Test Case**: Test scenario for each of the 6 forms covering conversion, rendering, calculation validation, schema extraction, print output
- **Form Header**: Standard header block appearing on all benchmark forms (Quality Management System, Document No, Revision No, Effective Date, Page number)
- **Performance Table**: Data entry table for sub-station or transmission line performance metrics with interruption counts and energy loss calculations
- **Shift Roster Grid**: 2D grid with employee names as rows and days-of-month (1-31) as columns, cells contain shift codes
- **Hierarchical Checklist**: Nested inspection checklist with decimal numbering and observation/condition fields
- **Signature Block**: Approval section with labeled lines for reviewer/approver name, designation, date, and signature
- **Calculated Column**: Table column with formula (e.g., Total = Forced + Scheduled, Power = Voltage * Current / 1000)
- **Aggregate Row**: Footer row displaying sum, average, or count across table columns
- **Time-Axis Row**: Generated rows representing hourly time slots (e.g., 7:00, 8:00, 9:00...22:00)
- **Days-of-Month Column**: Generated columns representing days 1-31 based on month context
- **Shift Code Enum**: Enumerated values (A, B, C, G, F, Ad) representing shift types (A shift, B shift, C shift, Govt Holiday, Weekly Off, Additional Duty)
- **Observation Enum**: Enumerated values for inspection observations (Good, Acceptable, Poor, Yes, No, Healthy, Defective, Clean, Not Cleaned, etc.)

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All 6 benchmark forms are successfully converted to HTMLDSL format within 8 hours of development effort per form
- **SC-002**: Converted forms achieve 95% or greater visual fidelity when compared with original paper forms (measured by pixel difference or visual inspection checklist)
- **SC-003**: All converted forms upload to system without parsing errors (100% success rate)
- **SC-004**: Schema extraction produces valid JSON for all 6 forms with zero parsing failures
- **SC-005**: All calculated columns (totals, power calculations, percentages) produce mathematically correct results with 100% accuracy
- **SC-006**: All aggregate functions (sum, average) in footer rows compute correctly with 100% accuracy
- **SC-007**: Forms with Bengali text display correctly with zero character encoding errors
- **SC-008**: Print output for all 6 forms matches original paper layout with 95%+ fidelity
- **SC-009**: Forms load and become interactive in browser within 2 seconds for forms with up to 100 fields
- **SC-010**: Wide tables (30+ columns) render without horizontal scrolling on 1920px wide screens
- **SC-011**: Multi-page forms print with correct page breaks and no split rows across pages
- **SC-012**: Generated day-of-month grids (QF-GMD-14) correctly show 28-31 columns based on actual month
- **SC-013**: Hierarchical checklist numbering (QF-GMD-17) displays correctly with decimal format (1.0, 1.1, 2.0, etc.)
- **SC-014**: All enum field validations (shift codes, observation values) enforce constraints with 100% accuracy
- **SC-015**: Signature blocks capture signature data (draw or upload) and associate with correct approval role
- **SC-016**: Form submission workflow completes successfully for all 6 forms with zero data loss
- **SC-017**: Query API returns submitted data for all 6 forms with correct field values
- **SC-018**: Reporting tables contain all row-level data from table widgets with computed columns populated
- **SC-019**: Forms remain usable and data remains accessible after schema version updates
- **SC-020**: Team achieves 90%+ confidence in system's ability to handle any similar enterprise form based on benchmark success

## Assumptions

- The 6 benchmark form screenshots in `docs/benchmark/` represent the complete set of forms to be converted
- Form screenshots are sufficiently high resolution to read all field labels, column headers, and instructions
- Original forms are designed for A4 paper size (some may be landscape orientation)
- Bengali text in QF-GMD-19 uses standard Unicode Bengali characters (not legacy fonts)
- Shift codes (A, B, C, G, F, Ad) are consistently used across Power Grid Company forms
- Observation enums (Good/Acceptable/Poor, etc.) are standard values used in inspection checklists
- Signature blocks require manual signature (draw/upload) but do not require digital signature (PKI)
- Forms are filled out by trained staff familiar with the domain (power grid operations, maintenance)
- Visual fidelity measurement is done by manual side-by-side comparison (automated pixel comparison is nice-to-have)
- Print output validation is done on standard office printers (no specialized high-resolution printing)
- Converted forms will be accessed via desktop browsers (1920x1080 or higher resolution)
- Forms do not require real-time collaborative editing (one user fills form at a time)
- Data submission volume is moderate (dozens to hundreds of submissions per form per month)
- Forms are used in intranet environment (no public internet access required)
- Existing HTMLDSL system capabilities (from US1-US6) are already implemented and working
- No new widget types are required beyond what HTMLDSL already supports
- Form conversion is manual process (developer creates HTML from screenshot), not automated OCR/image-to-HTML
- Bengali fonts are available via web fonts or system fonts (no custom font hosting required)
- Approval workflow (reviewed by, approved by) is for record-keeping only, not enforced by system
