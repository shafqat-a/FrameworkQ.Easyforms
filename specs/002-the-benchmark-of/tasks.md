# Tasks: Benchmark Forms Conversion and Validation

**Input**: Design documents from `/specs/002-the-benchmark-of/`
**Prerequisites**: plan.md, spec.md, research.md, quickstart.md

**Tests**: Testing is validation-based (visual comparison, functional testing, schema verification)

**Organization**: Tasks are grouped by user story (6 forms × 6 validation aspects)

**Constitution v1.1.0 Compliance**: All tasks MUST be converted to GitHub Issues before implementation begins.

## Format: `[ID] [P?] [Story] Description`
- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1-US6)
- **[Issue]**: Each task should become a GitHub Issue

## ⚠️ **IMPORTANT: Issue-Driven Development**

Per Constitution v1.1.0, before starting ANY task:

```bash
# Create GitHub Issues from this task list
gh issue create --title "T001: Setup benchmark directory structure" \
  --body "From specs/002-the-benchmark-of/tasks.md - Setup Phase" \
  --label "task,benchmark" \
  --milestone "002-benchmark-forms"

# Repeat for all 200 tasks, or create issues incrementally per phase
```

---

## Phase 1: Setup (Infrastructure)

**Purpose**: Prepare directories and resources for benchmark conversion

- [ ] T001 [P] Create `templates/benchmark/` directory structure
- [ ] T002 [P] Create `templates/benchmark/test-data/` directory for sample JSON
- [ ] T003 [P] Create Bengali font CSS file at `frontend/src/css/bengali-fonts.css` with Google Fonts imports
- [ ] T004 [P] Copy 6 screenshots from `docs/benchmark/` to `templates/benchmark/originals/` for reference
- [ ] T005 Create `templates/benchmark/README.md` with benchmark overview and validation checklist

---

## Phase 2: Foundation (No Prerequisites Needed)

**Purpose**: This feature uses existing HTMLDSL system - no foundational tasks required

**Status**: ✅ Foundation complete (from feature 001)

All HTMLDSL capabilities (US1-US6) are already implemented and tested.

---

## Phase 3: User Story 1 - Form Conversion (Priority: P1) 🎯

**Goal**: Convert all 6 paper forms to HTMLDSL format with accurate structure and data-* attributes

**Independent Test**: Each converted HTML file can be opened in browser and compared with original screenshot

**Constitution Requirement**: Create 6 GitHub Issues (one per form) before starting conversion

### QF-GMD-17: Surveillance Checklist (Simplest - Start Here)

- [ ] T006 [US1] [Issue #TBD] Create `templates/benchmark/qf-gmd-17-surveillance-checklist.html` with form skeleton (header, hierarchical checklist structure)
- [ ] T007 [US1] Convert hierarchical checklist items with decimal numbering (1.0, 1.1, 2.0, etc.) using `<ol data-widget="hierarchicalchecklist">`
- [ ] T008 [US1] Add observation enum fields (Good/Acceptable/Poor, Yes/No, Healthy/Defective) with proper data-enum attributes
- [ ] T009 [US1] Add signature blocks (Reviewed by, Approved by) at form footer
- [ ] T010 [US1] Test rendering in browser and compare with original screenshot

### QF-GMD-06: Performance Report (Moderate Complexity)

- [ ] T011 [US1] [Issue #TBD] Create `templates/benchmark/qf-gmd-06-performance-report.html` with form skeleton
- [ ] T012 [US1] Convert (A) Sub-Station Performance table with columns: Total Sub-station capacity, Forced, Scheduled, Total (calculated), interruption categories, Energy Interruption (MkWh), Remarks
- [ ] T013 [US1] Convert (B) Line Performance table with similar structure to (A)
- [ ] T014 [US1] Add calculated "Total" columns using data-compute="forced + scheduled"
- [ ] T015 [US1] Add aggregate sum rows in footer using data-agg="sum(column)"
- [ ] T016 [US1] Convert (A) and (B) Availability tables
- [ ] T017 [US1] Add signature blocks
- [ ] T018 [US1] Test rendering and calculations

### QF-GMD-22: Transformer Inspection (Moderate Complexity)

- [ ] T019 [US1] [Issue #TBD] Create `templates/benchmark/qf-gmd-22-transformer-inspection.html` with form skeleton
- [ ] T020 [US1] Convert header fields (Division, Substation, Date, Make, Type, Schedule checkboxes)
- [ ] T021 [US1] Convert nested inspection checklist with condition fields (Clean/Not Cleaned, Good/Defective, etc.)
- [ ] T022 [US1] Add checkbox groups for Yes/No, Good/Not Good conditions
- [ ] T023 [US1] Add action taken and reference text fields
- [ ] T024 [US1] Test nested checkbox structure and enum dropdowns

### QF-GMD-14: Shift Roster (Moderate-High Complexity)

- [ ] T025 [US1] [Issue #TBD] Create `templates/benchmark/qf-gmd-14-shift-roster.html` with form skeleton and landscape orientation
- [ ] T026 [US1] Convert header fields (Grid Circle, GMD, Date, Month, Substation)
- [ ] T027 [US1] Create 31-column grid table with day columns (1-31) using individual `<th data-col="day_N">` elements
- [ ] T028 [US1] Add shift code enum dropdowns in each day cell (A, B, C, G, F, Ad with data-enum)
- [ ] T029 [US1] Add notes section below grid explaining shift codes
- [ ] T030 [US1] Add signature blocks (Junior Assistant Manager, Deputy Manager, Manager)
- [ ] T031 [US1] Set landscape print orientation with data-print-orientation="landscape"
- [ ] T032 [US1] Test grid rendering and shift code selection

### QF-GMD-19: Daily Inspection Bengali (High Complexity)

- [ ] T033 [US1] [Issue #TBD] Create `templates/benchmark/qf-gmd-19-daily-inspection-bengali.html` with Bengali font imports
- [ ] T034 [US1] Add Bengali meta charset and font-family CSS
- [ ] T035 [US1] Convert form title and headers with Bengali text (গ্রীড উপকেন্দ্রে স্থাপিত যন্ত্রপাতির প্রাত্যহিক চেক লিস্ট)
- [ ] T036 [US1] Convert shift timing labels in Bengali (দৈনিক সকাল ০৬:০০ টা - সন্ধ্যা - ০৭:০০ টার মধ্যে পরীক্ষা)
- [ ] T037 [US1] Create inspection checklist table with Bengali labels and bilingual field labels
- [ ] T038 [US1] Add condition/observation columns with Bengali enum values (তাপমাত্রা, ভাল / খারাপ, etc.)
- [ ] T039 [US1] Test Bengali text rendering with Noto Sans Bengali font
- [ ] T040 [US1] Verify mixed Bengali/English content displays correctly

### QF-GMD-01: Log Sheet (Highest Complexity)

- [ ] T041 [US1] [Issue #TBD] Create `templates/benchmark/qf-gmd-01-log-sheet.html` with landscape orientation
- [ ] T042 [US1] Create extremely wide table with multi-row headers (2-3 header rows with colspan/rowspan)
- [ ] T043 [US1] Add Transformer TR #1 column group with subcolumns (kV side, Winding Oil, TAP Temp, etc.)
- [ ] T044 [US1] Add Transformer TR #2, TR #3, TR #4 column groups (repeat structure)
- [ ] T045 [US1] Add time-axis rows (7:00, 8:00, 9:00...22:00) in first column
- [ ] T046 [US1] Add calculated "Total MW" column using formulas from transformer readings
- [ ] T047 [US1] Add OLTC counter readings sections
- [ ] T048 [US1] Add signature blocks for each shift (A shift, B shift, C shift)
- [ ] T049 [US1] Set precise column widths using data-width attributes (in mm)
- [ ] T050 [US1] Configure print settings (landscape, repeat headers, column widths)
- [ ] T051 [US1] Test extremely wide table rendering and horizontal scroll
- [ ] T052 [US1] Test print output with header repetition

**Checkpoint**: ✅ All 6 forms converted to HTML with proper HTMLDSL markup

---

## Phase 4: User Story 2 - Visual Fidelity Verification (Priority: P2)

**Goal**: Verify all 6 forms match original screenshots with 95%+ visual fidelity

**Independent Test**: Side-by-side comparison checklist for each form

**Constitution Requirement**: Create GitHub Issue for visual fidelity verification

- [ ] T053 [P] [US2] [Issue #TBD] Create visual fidelity checklist template in `templates/benchmark/visual-fidelity-checklist.md`
- [ ] T054 [US2] Verify QF-GMD-17 visual fidelity (header, checklist structure, signature blocks) - document % match
- [ ] T055 [US2] Verify QF-GMD-06 visual fidelity (tables, column widths, calculated fields) - document % match
- [ ] T056 [US2] Verify QF-GMD-22 visual fidelity (nested checkboxes, table structure) - document % match
- [ ] T057 [US2] Verify QF-GMD-14 visual fidelity (31-column grid, landscape layout) - document % match
- [ ] T058 [US2] Verify QF-GMD-19 visual fidelity (Bengali text, mixed content) - document % match
- [ ] T059 [US2] Verify QF-GMD-01 visual fidelity (wide table, multi-row headers) - document % match
- [ ] T060 [US2] Calculate overall visual fidelity score across all 6 forms (target: 95%+)
- [ ] T061 [US2] Document deviations and justifications in visual-fidelity-report.md

**Checkpoint**: ✅ All forms achieve 95%+ visual fidelity or deviations are documented

---

## Phase 5: User Story 3 - Print Output Validation (Priority: P3)

**Goal**: Verify print-to-PDF output matches original paper forms

**Independent Test**: Print each form and compare PDF with screenshot

**Constitution Requirement**: Create GitHub Issue for print validation

- [ ] T062 [P] [US3] [Issue #TBD] Create print validation checklist in `templates/benchmark/print-validation-checklist.md`
- [ ] T063 [US3] Print QF-GMD-17 to PDF and verify layout, page breaks, signature block positioning
- [ ] T064 [US3] Print QF-GMD-06 to PDF and verify table structure, column widths, totals visibility
- [ ] T065 [US3] Print QF-GMD-22 to PDF and verify checkbox structure, page breaks
- [ ] T066 [US3] Print QF-GMD-14 to PDF in landscape and verify all 31 columns fit on single page
- [ ] T067 [US3] Print QF-GMD-19 to PDF and verify Bengali text renders correctly (no boxes/question marks)
- [ ] T068 [US3] Print QF-GMD-01 to PDF in landscape and verify multi-page output with header repetition
- [ ] T069 [US3] Calculate overall print fidelity score across all 6 forms (target: 95%+)
- [ ] T070 [US3] Document print issues and resolutions in print-validation-report.md

**Checkpoint**: ✅ All forms print correctly with 95%+ fidelity

---

## Phase 6: User Story 4 - Functional Validation (Priority: P4)

**Goal**: Verify all interactive elements work correctly (calculations, enums, numbering)

**Independent Test**: Fill each form with test data and verify functionality

**Constitution Requirement**: Create GitHub Issue for functional testing

- [ ] T071 [P] [US4] [Issue #TBD] Create test data JSON files in `templates/benchmark/test-data/` for all 6 forms
- [ ] T072 [US4] Test QF-GMD-17 hierarchical numbering displays correctly (1.0, 1.1, 2.0...)
- [ ] T073 [US4] Test QF-GMD-17 observation enums enforce valid values only
- [ ] T074 [US4] Test QF-GMD-06 calculated "Total" columns (Forced + Scheduled) auto-compute
- [ ] T075 [US4] Test QF-GMD-06 aggregate sum rows in footer update dynamically
- [ ] T076 [US4] Test QF-GMD-22 checkbox functionality and condition dropdown validation
- [ ] T077 [US4] Test QF-GMD-14 shift code enums (A/B/C/G/F/Ad) enforce valid values
- [ ] T078 [US4] Test QF-GMD-19 Bengali text input and display (no encoding issues)
- [ ] T079 [US4] Test QF-GMD-01 calculated power (MW) = voltage * current / 1000
- [ ] T080 [US4] Test QF-GMD-01 aggregate totals across all transformer readings
- [ ] T081 [US4] Document functional test results in functional-validation-report.md (100% accuracy target)

**Checkpoint**: ✅ All interactive elements function correctly with 100% accuracy

---

## Phase 7: User Story 5 - Schema Extraction Validation (Priority: P5)

**Goal**: Verify schema extraction and SQL DDL generation for all 6 forms

**Independent Test**: Upload forms, extract schemas, generate DDL, verify correctness

**Constitution Requirement**: Create GitHub Issue for schema validation

- [ ] T082 [P] [US5] [Issue #TBD] Upload QF-GMD-17 and verify schema JSON has all fields, enums, hierarchical structure
- [ ] T083 [P] [US5] Upload QF-GMD-06 and verify schema JSON has calculated columns, aggregates, table structure
- [ ] T084 [P] [US5] Upload QF-GMD-22 and verify schema JSON captures checkbox groups and nested tables
- [ ] T085 [P] [US5] Upload QF-GMD-14 and verify schema JSON has all 31 day columns and shift enums
- [ ] T086 [P] [US5] Upload QF-GMD-19 and verify schema JSON preserves Bengali labels (data-label-bn)
- [ ] T087 [P] [US5] Upload QF-GMD-01 and verify schema JSON has all transformer column groups and calculated fields
- [ ] T088 [US5] Generate SQL DDL for all 6 forms using `/v1/database/generate` endpoint (dry-run mode)
- [ ] T089 [US5] Verify DDL includes computed columns for all calculated fields
- [ ] T090 [US5] Verify DDL includes enum constraints for shift codes and observation values
- [ ] T091 [US5] Verify DDL includes proper indexes and foreign keys
- [ ] T092 [US5] Document schema extraction results in schema-validation-report.md (100% success target)

**Checkpoint**: ✅ All forms extract schemas correctly with zero parsing errors

---

## Phase 8: User Story 6 - End-to-End Workflow (Priority: P6)

**Goal**: Complete full workflow for all 6 forms (upload, fill, submit, query)

**Independent Test**: Full workflow test for each form

**Constitution Requirement**: Create GitHub Issue for E2E testing

- [ ] T093 [P] [US6] [Issue #TBD] Test QF-GMD-17 E2E: Upload → Fill checklist → Submit → Query → Verify data
- [ ] T094 [P] [US6] Test QF-GMD-06 E2E: Upload → Fill performance tables → Submit with calculated totals → Query reporting table
- [ ] T095 [P] [US6] Test QF-GMD-22 E2E: Upload → Fill inspection checklist → Submit → Verify checkboxes stored
- [ ] T096 [P] [US6] Test QF-GMD-14 E2E: Upload → Fill shift roster grid → Submit 31 days data → Query reporting table
- [ ] T097 [P] [US6] Test QF-GMD-19 E2E: Upload → Fill Bengali form → Submit → Verify Bengali text persists
- [ ] T098 [P] [US6] Test QF-GMD-01 E2E: Upload → Fill log sheet → Submit wide table data → Query all transformer readings
- [ ] T099 [US6] Verify zero data loss across all 6 form submissions
- [ ] T100 [US6] Document E2E test results in e2e-validation-report.md

**Checkpoint**: ✅ All 6 forms complete full workflow successfully

---

## Phase 9: Validation Reports and Documentation

**Purpose**: Create comprehensive validation documentation

**Constitution Requirement**: Create GitHub Issue for documentation

- [ ] T101 [P] [Issue #TBD] Create `templates/benchmark/validation-summary.md` with consolidated results from all validation reports
- [ ] T102 [P] Create visual comparison screenshots showing original vs. converted for each form
- [ ] T103 [P] Calculate and document overall benchmark success metrics (visual fidelity %, functional accuracy %, schema success %, E2E pass rate)
- [ ] T104 Update `templates/benchmark/README.md` with final results and instructions for running validations
- [ ] T105 Create demo video or screenshot walkthrough showing all 6 forms working (optional)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately after creating GitHub Issues
- **Foundation (Phase 2)**: ✅ Already complete (uses feature 001)
- **User Story 1 (Conversion)**: Depends on Setup completion
  - Forms can be converted in parallel (6 different files)
  - Suggested order: QF-GMD-17 (simplest) → QF-GMD-06, QF-GMD-22, QF-GMD-14, QF-GMD-19 → QF-GMD-01 (most complex)
- **User Story 2 (Visual)**: Depends on US1 completion (all forms converted)
- **User Story 3 (Print)**: Depends on US1 completion (can run parallel with US2)
- **User Story 4 (Functional)**: Depends on US1 completion (can run parallel with US2/US3)
- **User Story 5 (Schema)**: Depends on US1 completion (can run parallel with US2/US3/US4)
- **User Story 6 (E2E)**: Depends on US1, US4, US5 completion
- **Documentation (Phase 9)**: Depends on all user stories completing

### Form Conversion Order (Within US1)

**Recommended sequence** (simple to complex):
1. QF-GMD-17 (4-6 hours) - Learn hierarchical checklists
2. QF-GMD-06 (6-8 hours) - Learn calculated columns and aggregates
3. QF-GMD-22 (6-8 hours) - Learn nested checkbox tables
4. QF-GMD-14 (6-8 hours) - Learn days-of-month grids
5. QF-GMD-19 (8-10 hours) - Learn Bengali text handling
6. QF-GMD-01 (10-12 hours) - Most complex, apply all learnings

**Parallel option** (if multiple developers):
- Dev A: QF-GMD-17, QF-GMD-06
- Dev B: QF-GMD-22, QF-GMD-14
- Dev C: QF-GMD-19, QF-GMD-01

### Parallel Opportunities

- **All Setup tasks (T001-T005)** can run in parallel
- **All form conversions within US1 (T006-T052)** can run in parallel if multiple developers
- **All validation tasks in US2-US6** can run in parallel once forms are converted
- **Visual, Print, Functional, Schema validations (US2-US5)** can all run concurrently

---

## Parallel Example: User Story 1 (Form Conversion)

With 3 developers, convert forms simultaneously:

```bash
# Dev A: Simple forms
Task T006-T010: QF-GMD-17 (Surveillance Checklist)
Task T011-T018: QF-GMD-06 (Performance Report)

# Dev B: Moderate forms
Task T019-T024: QF-GMD-22 (Transformer Inspection)
Task T025-T032: QF-GMD-14 (Shift Roster)

# Dev C: Complex forms
Task T033-T040: QF-GMD-19 (Daily Inspection Bengali)
Task T041-T052: QF-GMD-01 (Log Sheet)
```

---

## Implementation Strategy

### MVP First (Start with QF-GMD-17)

1. Complete Phase 1: Setup (T001-T005) - 1-2 hours
2. Convert QF-GMD-17 (T006-T010) - 4-6 hours
3. **STOP and VALIDATE**:
   - Visual check (US2)
   - Print test (US3)
   - Functional test (US4)
   - Schema extraction (US5)
   - E2E workflow (US6)
4. If successful, proves system works for hierarchical checklists
5. Continue with remaining forms

### Incremental Delivery

1. **Phase 1**: Setup → Foundation ready
2. **Phase 2**: Convert QF-GMD-17 → Validate → Benchmark 1/6 complete
3. **Phase 3**: Convert QF-GMD-06 → Validate → Benchmark 2/6 complete
4. **Phase 4**: Convert QF-GMD-22 → Validate → Benchmark 3/6 complete
5. **Phase 5**: Convert QF-GMD-14 → Validate → Benchmark 4/6 complete
6. **Phase 6**: Convert QF-GMD-19 → Validate → Benchmark 5/6 complete
7. **Phase 7**: Convert QF-GMD-01 → Validate → Benchmark 6/6 complete ✅
8. **Phase 8**: Consolidate reports → Final validation summary

Each form proves a different aspect of the system.

### Parallel Team Strategy

With 3 developers (after Setup):

1. **Week 1**: All forms converted (T006-T052) - parallel work
2. **Week 2**: All validations (T053-T100) - parallel testing
3. **Week 3**: Documentation and reporting (T101-T105)

Total: 2-3 weeks with 3 developers, or 6-8 weeks with 1 developer

---

## GitHub Issues Integration (Constitution v1.1.0)

**BEFORE starting implementation**, convert tasks to GitHub Issues:

### Option A: Create All Issues Upfront

```bash
# Create milestone
gh issue milestone create "002-benchmark-forms" \
  --description "Convert and validate 6 benchmark forms"

# Create issues for all tasks
for i in {1..105}; do
  # Extract task from tasks.md and create issue
  # Tag with labels: task, benchmark, US1-US6
done
```

### Option B: Create Issues Incrementally

```bash
# Create issues for Setup phase
gh issue create --title "Setup: Create benchmark directory structure" \
  --body "Tasks T001-T005 from specs/002-the-benchmark-of/tasks.md" \
  --label "task,setup,benchmark"

# Create issues for each form conversion
gh issue create --title "Convert QF-GMD-17 Surveillance Checklist" \
  --body "Tasks T006-T010: Convert surveillance checklist..." \
  --label "task,conversion,US1,benchmark"

# Continue for each phase
```

### Issue Tracking During Implementation

```bash
# Start work on issue
gh issue develop 42 --checkout  # Creates branch and checks out

# Work on task, commit with issue reference
git commit -m "feat: convert QF-GMD-17 checklist (#42)"

# When complete, task stays in tasks.md but issue shows progress
gh issue comment 42 --body "Completed T006-T010"
```

---

## Notes

- **[P] tasks** = different files, no dependencies, can run in parallel
- **[Story] label** maps task to specific user story for traceability
- **[Issue #TBD]** = GitHub Issue number to be assigned when created
- **Each form conversion is independently testable**
- **No code changes to HTMLDSL system** - uses existing capabilities
- **Focus on accurate HTML authoring and validation**
- All file paths are absolute
- Commit after each form conversion or validation milestone

---

## Task Summary

- **Total Tasks**: 105
- **Setup Phase**: 5 tasks (T001-T005)
- **Foundation Phase**: 0 tasks (uses existing system)
- **User Story 1 (Conversion)**: 47 tasks (T006-T052)
  - QF-GMD-17: 5 tasks
  - QF-GMD-06: 8 tasks
  - QF-GMD-22: 6 tasks
  - QF-GMD-14: 8 tasks
  - QF-GMD-19: 8 tasks
  - QF-GMD-01: 12 tasks (most complex)
- **User Story 2 (Visual)**: 9 tasks (T053-T061)
- **User Story 3 (Print)**: 9 tasks (T062-T070)
- **User Story 4 (Functional)**: 11 tasks (T071-T081)
- **User Story 5 (Schema)**: 11 tasks (T082-T092)
- **User Story 6 (E2E)**: 8 tasks (T093-T100)
- **Documentation**: 5 tasks (T101-T105)

**Parallel Opportunities**: 60+ tasks can run in parallel across forms and validation types

**Estimated Effort**:
- Solo developer: 50-65 hours (6-8 weeks)
- 3 developers: 20-25 hours (2-3 weeks)
- 6 developers: 10-15 hours (1-2 weeks)

**GitHub Issues to Create**: 105 issues (or group into ~20-30 logical issues covering multiple tasks)

---

**Next Steps**:

1. **Create GitHub Issues** per Constitution v1.1.0
2. **Start with Setup** (T001-T005)
3. **Convert simplest form first** (QF-GMD-17)
4. **Validate before moving to next form**
5. **Work through all 6 forms systematically**

The benchmark will prove the HTMLDSL system is production-ready! 🚀
