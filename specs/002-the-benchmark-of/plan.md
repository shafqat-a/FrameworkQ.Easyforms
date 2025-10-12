# Implementation Plan: Benchmark Forms Conversion and Validation

**Branch**: `002-the-benchmark-of` | **Date**: 2025-10-12 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/002-the-benchmark-of/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Convert 6 real-world enterprise forms from Power Grid Company of Bangladesh (QF-GMD-06, -14, -01, -17, -19, -22) from paper screenshots to HTMLDSL format. Validate visual fidelity (95%+ match), functional correctness (calculations, enums, hierarchical numbering), print output, schema extraction, and end-to-end workflow. This benchmark validates the HTMLDSL system's production readiness for complex enterprise forms.

**Technical Approach**: Manual HTML conversion from screenshots using existing HTMLDSL system (no new code required). Focus on accurate markup with data-* attributes, proper column widths, Bengali font configuration, and print CSS optimization. Validation through side-by-side comparison, functional testing, and schema extraction verification.

## Technical Context

**Language/Version**: HTML5, CSS3, (uses existing .NET Core backend and jQuery frontend from feature 001)

**Primary Dependencies**:
- Existing HTMLDSL system (feature 001) - all US1-US6 capabilities
- Bengali web fonts (Google Fonts: Hind Siliguri, Noto Sans Bengali, or Nikosh)
- No additional backend dependencies
- No additional frontend dependencies

**Storage**: Uses existing system (SQL Server/PostgreSQL via providers) - no changes

**Testing**: Manual visual comparison, functional testing in browser, PDF comparison

**Target Platform**: Web browsers (Chrome, Firefox, Safari, Edge) - same as existing system

**Project Type**: Content/templates - HTML form creation using existing system

**Performance Goals**:
- Form conversion: <8 hours per form (manual HTML authoring)
- Visual fidelity: 95%+ match with originals
- All success criteria from existing system apply

**Constraints**:
- Must not modify existing HTMLDSL system code (use existing capabilities only)
- Must achieve 95%+ visual fidelity to original paper forms
- Must support Bengali Unicode text rendering
- Must fit on A4/Letter paper when printed
- Wide tables must render on 1920px screens

**Scale/Scope**:
- Exactly 6 forms to convert (QF-GMD-06, -14, -01, -17, -19, -22)
- Forms range from simple (1-page checklists) to complex (multi-page wide tables)
- Total fields across all 6 forms: ~300-400 fields
- No new system features required

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Status**: ✅ PASS

**Constitution Compliance**:
- ✅ **GitHub Workflow**: Feature branch created (002-the-benchmark-of), will follow PR process
- ✅ **Modular Architecture**: No code changes required (uses existing modules), creates template content only
- ✅ **User Story-Driven**: 6 user stories defined (conversion, visual, print, functional, schema, end-to-end)
- ✅ **Design Before Code**: Spec created first, now planning, then implementation (HTML conversion)
- ✅ **Security and Quality**: Uses existing sanitization, validation, no security changes

**No violations**: This is a benchmark/validation feature that creates HTML templates using the existing system. No new code or architecture changes required.

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

**No code changes required**. This feature creates HTML templates only.

```
templates/benchmark/                  # New directory for converted forms
├── qf-gmd-06-performance-report.html
├── qf-gmd-14-shift-roster.html
├── qf-gmd-01-log-sheet.html
├── qf-gmd-17-surveillance-checklist.html
├── qf-gmd-19-daily-inspection-bengali.html
├── qf-gmd-22-transformer-inspection.html
├── README.md                          # Benchmark documentation
└── test-data/                         # Sample data for testing
    ├── qf-gmd-06-sample.json
    ├── qf-gmd-14-sample.json
    ├── qf-gmd-01-sample.json
    ├── qf-gmd-17-sample.json
    ├── qf-gmd-19-sample.json
    └── qf-gmd-22-sample.json

docs/benchmark/                        # Existing - contains 6 screenshots
├── Screenshot 2025-10-01 at 6.16.21 AM.png  # QF-GMD-06
├── Screenshot 2025-10-01 at 6.19.25 AM.png  # QF-GMD-14
├── Screenshot 2025-10-01 at 6.20.23 AM.png  # QF-GMD-01
├── Screenshot 2025-10-01 at 6.21.25 AM.png  # QF-GMD-17
├── Screenshot 2025-10-01 at 6.21.51 AM.png  # QF-GMD-19
└── Screenshot 2025-10-01 at 6.22.06 AM.png  # QF-GMD-22

frontend/src/css/                      # May add Bengali font imports
└── bengali-fonts.css                  # Optional: Bengali font configuration
```

**Structure Decision**: Template-only feature. Creates 6 HTML files in `templates/benchmark/` directory with corresponding test data. Uses existing backend/frontend code from feature 001 without modifications.

## Complexity Tracking

*Fill ONLY if Constitution Check has violations that must be justified*

**Status**: No violations

**Post-Design Re-Evaluation**: ✅ PASS

This is a benchmark/validation feature that creates HTML templates using the existing HTMLDSL system. No code changes required.

Constitution compliance:
- ✅ **GitHub Workflow**: Feature branch, PR process
- ✅ **Modular**: Uses existing modules without changes
- ✅ **User Story-Driven**: 6 stories for systematic validation
- ✅ **Design First**: Spec → Research → Plan complete
- ✅ **Quality**: Validation through manual QA and automated testing

This feature proves the system works as designed by converting real-world enterprise forms.
