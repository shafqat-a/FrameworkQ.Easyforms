# Quickstart: Benchmark Forms Conversion

**Feature**: 002-the-benchmark-of | **Date**: 2025-10-12

## Overview

This guide provides step-by-step instructions for converting the 6 benchmark forms from Power Grid Company of Bangladesh into HTMLDSL format and validating them against the original paper forms.

---

## Prerequisites

- ✅ HTMLDSL system running (feature 001 implemented)
- ✅ API server running at http://localhost:5000
- ✅ Original form screenshots in `docs/benchmark/`
- ✅ Text editor or IDE for HTML authoring
- ✅ Web browser (Chrome, Firefox, Safari, Edge)
- ✅ PDF viewer for print output validation

---

## Form Conversion Workflow

### Step 1: Analyze Original Form

For each form in `docs/benchmark/`:

1. Open screenshot in image viewer
2. Identify form structure:
   - Header block (document no, revision, effective date)
   - Sections and subsections
   - Tables (simple vs. complex multi-row headers)
   - Calculated columns (formulas)
   - Signature blocks
   - Special features (Bengali text, enums, hierarchical numbering)

3. Document field inventory:
   - Count total fields
   - List field names
   - Identify data types
   - Note validation rules
   - List formulas

### Step 2: Create HTML Skeleton

Create file in `templates/benchmark/qf-gmd-##-name.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>[Form Title]</title>
    <link href="https://fonts.googleapis.com/css2?family=Noto+Sans+Bengali:wght@400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="../../frontend/src/css/forms.css">
    <link rel="stylesheet" href="../../frontend/src/css/print.css" media="print">
</head>
<body>
    <form data-form="qf-gmd-##" data-title="[Form Title]" data-version="1.0"
          data-print-page-size="A4" data-print-orientation="portrait">

        <!-- Form Header -->
        <section data-section id="header" data-keep-together>
            <!-- Quality Management System table -->
        </section>

        <!-- Main Content -->
        <section data-page id="page-1">
            <!-- Form sections here -->
        </section>

        <!-- Signature Block -->
        <section data-section id="signatures" data-keep-together>
            <!-- Signature lines -->
        </section>

    </form>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="../../frontend/src/js/expression-evaluator.js"></script>
    <script src="../../frontend/src/js/table-manager.js"></script>
    <script src="../../frontend/src/js/widgets/table.js"></script>
    <script src="../../frontend/src/js/validation.js"></script>
    <script src="../../frontend/src/js/formruntime.js"></script>
    <script>
        $(document).ready(function() {
            FormRuntimeHTMLDSL.mount('form[data-form]', {
                apiBaseUrl: 'http://localhost:5000',
                debug: true
            });
        });
    </script>
</body>
</html>
```

### Step 3: Convert Form Header

All forms have standard header structure:

```html
<table style="width: 100%; border: 1px solid #000;">
    <tr>
        <td rowspan="2" style="width: 30%; text-align: center; font-weight: bold;">
            POWER GRID COMPANY OF BANGLADESH LTD.
        </td>
        <td style="width: 40%;">Document No: QF-GMD-##</td>
        <td>Revision No: ##</td>
    </tr>
    <tr>
        <td>Effective Date: DD/MM/YY</td>
        <td>Page: <span data-page-counter></span> of <span data-page-total></span></td>
    </tr>
</table>
```

### Step 4: Convert Main Tables

#### For Simple Tables (QF-GMD-17, QF-GMD-22)

```html
<table data-table id="checklist">
    <thead>
        <tr>
            <th data-col="item">Item</th>
            <th data-col="observation">Observation</th>
            <th data-col="remarks">Remarks</th>
        </tr>
    </thead>
    <tbody>
        <tr data-row-template>
            <td><input name="item" type="text" readonly></td>
            <td>
                <select name="observation" data-enum="Good|Acceptable|Poor">
                    <option value="">-</option>
                    <option value="Good">Good</option>
                    <option value="Acceptable">Acceptable</option>
                    <option value="Poor">Poor</option>
                </select>
            </td>
            <td><input name="remarks" type="text"></td>
        </tr>
    </tbody>
</table>
```

#### For Tables with Calculations (QF-GMD-06)

```html
<table data-table id="performance">
    <thead>
        <tr>
            <th data-col="forced">Forced</th>
            <th data-col="scheduled">Scheduled</th>
            <th data-col="total" data-formula="forced + scheduled">Total</th>
        </tr>
    </thead>
    <tbody>
        <tr data-row-template>
            <td><input name="forced" type="number" min="0"></td>
            <td><input name="scheduled" type="number" min="0"></td>
            <td><input name="total" data-compute="forced + scheduled" readonly></td>
        </tr>
    </tbody>
    <tfoot>
        <tr>
            <td colspan="2">Total</td>
            <td data-agg="sum(total)" data-target="#grand_total"></td>
        </tr>
    </tfoot>
</table>
```

#### For Days-of-Month Grid (QF-GMD-14)

```html
<table data-table id="roster" data-row-mode="finite">
    <thead>
        <tr>
            <th data-col="name">Name</th>
            <th data-col="day_1">1</th>
            <th data-col="day_2">2</th>
            <!-- ... repeat for days 3-31 ... -->
            <th data-col="day_31">31</th>
        </tr>
    </thead>
    <tbody>
        <tr data-row-template>
            <td><input name="name" type="text"></td>
            <td><select name="day_1" data-enum="A|B|C|G|F|Ad"><option>-</option><option>A</option>...</select></td>
            <!-- ... repeat for all days ... -->
        </tr>
    </tbody>
</table>
```

### Step 5: Add Bengali Text (QF-GMD-19)

```html
<label lang="bn">ট্রান্সফরমারের তাপমাত্রা</label>
<label>Transformer Temperature</label>

<th data-col="condition" data-label-bn="অবস্থা" data-label="Condition">
    অবস্থা / Condition
</th>
```

### Step 6: Add Signature Blocks

```html
<section data-section id="signatures" data-pagebreak="avoid">
    <div style="margin-top: 20mm;">
        <table style="width: 100%;">
            <tr>
                <td style="width: 50%; text-align: center;">
                    <div style="margin-top: 15mm; border-top: 1px solid #000;">
                        Reviewed by (GMT-1):
                    </div>
                </td>
                <td style="width: 50%; text-align: center;">
                    <div style="margin-top: 15mm; border-top: 1px solid #000;">
                        Approved by (DT):
                    </div>
                </td>
            </tr>
        </table>
    </div>
</section>
```

---

## Validation Steps

### Visual Validation

1. Open converted HTML in browser
2. Open original screenshot side-by-side
3. Compare:
   - [ ] Header layout matches
   - [ ] Table structure matches
   - [ ] Column widths proportional
   - [ ] Fonts and sizes similar
   - [ ] Spacing and padding consistent
   - [ ] Signature blocks positioned correctly
4. Measure deviations (should be <5%)

### Functional Validation

1. Fill out form with test data
2. Verify:
   - [ ] Calculated columns auto-compute
   - [ ] Aggregates update in footers
   - [ ] Enum dropdowns show correct values
   - [ ] Hierarchical numbering displays correctly
   - [ ] Bengali text renders correctly
3. Check browser console for errors

### Schema Validation

```bash
# Upload form
curl -X POST http://localhost:5000/v1/forms \
  -F "htmlFile=@templates/benchmark/qf-gmd-##-name.html"

# Verify schema extraction
curl http://localhost:5000/v1/forms/qf-gmd-##/schema | jq

# Check for:
# - All fields present
# - Correct data types
# - Formulas captured
# - Enums captured
```

### Print Validation

1. Open form in browser
2. Press Ctrl+P (Windows) or Cmd+P (Mac)
3. Compare print preview with original screenshot:
   - [ ] Page breaks correct
   - [ ] Headers repeat on multi-page forms
   - [ ] Column widths match
   - [ ] Signature blocks on correct page
   - [ ] Overall layout matches (95%+)
4. Save as PDF and measure fidelity

### End-to-End Validation

```bash
# 1. Upload form (already done above)

# 2. Submit test data
curl -X POST http://localhost:5000/v1/submissions \
  -H "Content-Type: application/json" \
  -d @templates/benchmark/test-data/qf-gmd-##-sample.json

# 3. Query submissions
curl "http://localhost:5000/v1/query/submissions?formId=qf-gmd-##" | jq

# 4. Retrieve specific submission
curl http://localhost:5000/v1/submissions/{instanceId} | jq

# Verify:
# - All fields saved
# - Calculated values correct
# - Table rows preserved
# - No data loss
```

---

## Form-Specific Guidelines

### QF-GMD-06 (Performance Report)
- Two main tables: (A) Sub-Station Performance, (B) Line Performance
- Calculated "Total" = Forced + Scheduled
- Aggregate totals in footer
- Availability percentages (may need manual calculation or formula)

### QF-GMD-14 (Shift Roster)
- 31 columns (days of month)
- Landscape orientation
- Shift codes: A, B, C, G (Govt Holiday), F (Weekly Off), Ad (Additional Duty)
- Notes section below grid

### QF-GMD-01 (Log Sheet)
- Extremely wide (4 transformers × 7-8 readings each)
- Multi-row headers with colspan/rowspan
- Hourly rows (7:00 - 22:00 or 24-hour)
- Signature blocks per shift (A, B, C shifts)
- Landscape, may need multiple pages

### QF-GMD-17 (Surveillance Checklist)
- Hierarchical decimal numbering (1.0, 1.1, 2.0, 2.1, etc.)
- Observation enums (Good/Acceptable/Poor, Yes/No, Healthy/Defective)
- Mix of table and hierarchical list structures

### QF-GMD-19 (Daily Inspection - Bengali)
- All labels in Bengali with English translations
- Use lang="bn" attribute
- Multiple shift sections (Shift B: 14:00-22:00)
- Bengali Unicode text throughout

### QF-GMD-22 (Transformer Inspection)
- Complex nested checkbox tables
- Condition fields (Clean/Not Cleaned, Good/Defective, etc.)
- Hierarchical equipment sections
- Mix of checkboxes, dropdowns, and text fields

---

## Success Metrics

Track progress for each form:

| Form | Visual | Functional | Print | Schema | E2E | Overall |
|------|--------|------------|-------|--------|-----|---------|
| QF-GMD-06 | □ | □ | □ | □ | □ | __ % |
| QF-GMD-14 | □ | □ | □ | □ | □ | __ % |
| QF-GMD-01 | □ | □ | □ | □ | □ | __ % |
| QF-GMD-17 | □ | □ | □ | □ | □ | __ % |
| QF-GMD-19 | □ | □ | □ | □ | □ | __ % |
| QF-GMD-22 | □ | □ | □ | □ | □ | __ % |

**Target**: All 6 forms with 95%+ overall score

---

## Estimated Timeline

- **QF-GMD-17** (Surveillance Checklist): 4-6 hours (simplest)
- **QF-GMD-06** (Performance Report): 6-8 hours (moderate complexity)
- **QF-GMD-22** (Transformer Inspection): 6-8 hours (nested checkboxes)
- **QF-GMD-14** (Shift Roster): 6-8 hours (31-column grid)
- **QF-GMD-19** (Daily Inspection Bengali): 8-10 hours (Bengali text)
- **QF-GMD-01** (Log Sheet): 10-12 hours (most complex, extremely wide)

**Total**: 40-52 hours for all 6 forms

---

## Troubleshooting

### Issue: Bengali text shows boxes/question marks

**Solution**: Ensure Bengali fonts loaded
```html
<link href="https://fonts.googleapis.com/css2?family=Noto+Sans+Bengali&display=swap" rel="stylesheet">
<style>
    body { font-family: 'Noto Sans Bengali', sans-serif; }
</style>
```

### Issue: Wide table scrolls horizontally (QF-GMD-01)

**Expected**: This is correct for screen view. For print, use landscape:
```html
<form data-print-orientation="landscape">
```

### Issue: Calculated column not updating

**Solution**: Check formula syntax and field names
```html
<!-- Ensure field names match formula -->
<input name="forced" type="number">
<input name="scheduled" type="number">
<input name="total" data-compute="forced + scheduled" readonly>
```

### Issue: Table headers not repeating on print

**Solution**: Add print attribute
```html
<table data-table data-print-repeat-head-rows="1">
```

### Issue: Hierarchical numbering not showing

**Solution**: Check CSS counters (already in forms.css) and HTML structure
```html
<ol data-widget="hierarchicalchecklist" data-numbering="decimal">
    <li>Item 1.0</li>
    <li>Item 2.0
        <ol>
            <li>Item 2.1</li>
        </ol>
    </li>
</ol>
```

---

## Next Steps

After converting all 6 forms:

1. Create validation report documenting fidelity scores
2. Generate test data for each form
3. Run end-to-end tests
4. Document any HTMLDSL system limitations discovered
5. Create PR with all 6 converted forms

---

**Good luck with the conversions!** These forms will validate the system's production readiness.
