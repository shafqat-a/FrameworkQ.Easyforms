# Research & Decisions: Benchmark Forms Conversion

**Feature**: 002-the-benchmark-of | **Date**: 2025-10-12
**Phase**: 0 (Research)

## Overview

This document captures research and decisions for converting 6 Power Grid Company of Bangladesh benchmark forms to HTMLDSL format. Since the HTMLDSL system is already implemented (feature 001), this focuses on conversion strategies and best practices.

---

## 1. Bengali Font Selection

### Decision
**Google Fonts: Noto Sans Bengali** as primary, with **Hind Siliguri** as fallback

### Rationale
- Noto Sans Bengali: Comprehensive Unicode coverage, professionally designed by Google
- Hind Siliguri: Good readability for Bengali text, lighter weight
- Both are free, web-safe, and CDN-hosted (no custom font files needed)
- Support full Bengali character set including conjuncts
- Compatible with all modern browsers

### Alternatives Considered
- **SutonnyMJ**: Legacy font, requires custom hosting, not Unicode-compliant
- **Nikosh**: Good option but heavier file size than Noto Sans
- **System fonts**: Inconsistent across platforms

### Implementation
```html
<link href="https://fonts.googleapis.com/css2?family=Noto+Sans+Bengali:wght@400;700&family=Hind+Siliguri:wght@400;700&display=swap" rel="stylesheet">
```

```css
body, form {
    font-family: 'Noto Sans Bengali', 'Hind Siliguri', sans-serif;
}
```

---

## 2. Form Conversion Strategy

### Decision
**Manual HTML authoring** with systematic conversion process per form

### Rationale
- Highest accuracy and fidelity (human can match original exactly)
- Full control over data-* attribute placement
- Can optimize for readability and maintainability
- Allows proper testing during conversion

### Conversion Process (Per Form)
1. Analyze screenshot structure (header, sections, tables, signatures)
2. Identify field types and validation rules
3. Identify calculated columns and formulas
4. Create HTML skeleton with form header
5. Convert tables section-by-section
6. Add data-* attributes for semantics
7. Test in browser against screenshot
8. Adjust CSS for exact match
9. Upload to API and verify schema extraction
10. Test print output

### Alternatives Considered
- **OCR/Image-to-HTML**: Too error-prone for complex tables, requires manual cleanup anyway
- **Template generation**: No suitable template for these specific forms

---

## 3. Wide Table Handling (QF-GMD-01 Log Sheet)

### Decision
**Horizontal scrolling** for screen view + **landscape print** with optimized column widths

### Rationale
- Log sheet has 30+ columns (4 transformers × 7-8 readings each)
- Cannot reasonably fit on portrait A4 without making text unreadable
- Horizontal scroll acceptable for data entry (users expect this for wide tables)
- Print to landscape A4 with precise column widths matches original

### Implementation
```html
<form data-print-orientation="landscape" data-print-page-size="A4">
  <table data-table data-print-repeat-head-rows="2">
    <colgroup>
      <col style="width: 15mm;"> <!-- Time column -->
      <col style="width: 12mm;"> <!-- Each reading column -->
      <!-- Repeat for all columns -->
    </colgroup>
    ...
  </table>
</form>
```

```css
@media screen {
    table[data-table] {
        overflow-x: auto;
        display: block;
    }
}
```

### Alternatives Considered
- **Responsive columns**: Breaks visual fidelity, doesn't match original
- **Font size reduction**: Makes form unreadable
- **Split into multiple tables**: Breaks original structure

---

## 4. Days-of-Month Grid Generation (QF-GMD-14)

### Decision
**Static 31-column table** with CSS to hide unused days based on month

### Rationale
- Simple, predictable, no JavaScript date logic needed
- Always shows maximum possible columns (1-31)
- CSS can hide columns beyond actual month days via `data-context-month`
- Fallback: show all 31 columns if month not specified

### Implementation
```html
<table data-grid id="roster" data-context-month="2025-02">
  <thead>
    <tr>
      <th>Name</th>
      <th data-day="1">1</th>
      <th data-day="2">2</th>
      <!-- ... -->
      <th data-day="31">31</th>
    </tr>
  </thead>
</table>
```

```javascript
// Hide days beyond month end (e.g., Feb 29, 30, 31)
var daysInMonth = new Date(year, month + 1, 0).getDate();
$('th[data-day], td[data-day]').each(function() {
    var day = parseInt($(this).attr('data-day'));
    if (day > daysInMonth) {
        $(this).hide();
    }
});
```

### Alternatives Considered
- **Server-side generation**: Requires API call, more complex
- **Dynamic column generation**: Already supported but static is simpler for benchmark

---

## 5. Hierarchical Checklist Numbering (QF-GMD-17)

### Decision
**CSS counters** with decimal format (1.0, 1.1, 2.0)

### Rationale
- CSS counters automatically number items
- Supports nested levels (1.0 → 1.1 → 1.1.1)
- No JavaScript needed
- Matches original numbering scheme exactly

### Implementation
```html
<ol data-widget="hierarchicalchecklist" data-numbering="decimal">
  <li data-key="transformer">Transformer
    <ol>
      <li data-key="silica-gel">Silica gel <input type="checkbox"></li>
      <li data-key="oil-level">Oil level <input type="checkbox"></li>
    </ol>
  </li>
  <li data-key="circuit-breaker">Circuit Breaker
    <ol>
      <li>Physical Condition <input type="checkbox"></li>
    </ol>
  </li>
</ol>
```

```css
ol[data-numbering="decimal"] {
    counter-reset: item;
    list-style: none;
}

ol[data-numbering="decimal"] > li {
    counter-increment: item;
}

ol[data-numbering="decimal"] > li::before {
    content: counter(item) ".0 ";
}

ol[data-numbering="decimal"] > li ol > li::before {
    content: counters(item, ".") " ";
}
```

---

## 6. Multi-Row Table Headers (All Forms)

### Decision
**Native HTML colspan/rowspan** with precise width specifications

### Rationale
- HTML supports complex headers natively
- Exact control over cell merging
- Print-friendly (browsers handle correctly)
- No JavaScript manipulation needed

### Implementation
```html
<thead>
  <tr>
    <th rowspan="2">Time</th>
    <th colspan="3">Transformer TR #1</th>
    <th colspan="3">Transformer TR #2</th>
  </tr>
  <tr>
    <th data-col="tr1_voltage" data-width="15mm">kV</th>
    <th data-col="tr1_current" data-width="15mm">A</th>
    <th data-col="tr1_temp" data-width="15mm">Temp</th>
    <th data-col="tr2_voltage" data-width="15mm">kV</th>
    <th data-col="tr2_current" data-width="15mm">A</th>
    <th data-col="tr2_temp" data-width="15mm">Temp</th>
  </tr>
</thead>
```

---

## 7. Validation Strategy for Benchmark Forms

### Decision
**Progressive validation**: Start with visual/structural, then functional, then data

### Rationale
- Ensures form looks right before testing functionality
- Easier to debug when issues are isolated
- Matches natural QA workflow

### Validation Sequence (Per Form)
1. **Visual**: Side-by-side screenshot comparison (95%+ match)
2. **Structural**: Upload to API, verify schema extraction (zero errors)
3. **Functional**: Test calculations, enums, numbering (100% accuracy)
4. **Print**: Print to PDF, compare with original (95%+ match)
5. **Data**: Submit sample data, query back (zero data loss)
6. **End-to-end**: Complete workflow from open to query (all steps pass)

---

## Summary of Decisions

| Area | Decision | Rationale |
|------|----------|-----------|
| Bengali Fonts | Noto Sans Bengali + Hind Siliguri | Web-safe, Unicode-compliant, free |
| Conversion Method | Manual HTML authoring | Highest accuracy and control |
| Wide Tables | Horizontal scroll + landscape print | Practical for 30+ columns |
| Day-of-Month Grid | Static 31 columns | Simple, predictable |
| Hierarchical Numbering | CSS counters | Automatic, print-friendly |
| Multi-Row Headers | Native colspan/rowspan | Standard HTML, print-compatible |
| Validation Strategy | Progressive (visual → functional → data) | Systematic, easier debugging |

---

**Next Steps**: Proceed to Phase 1 to create conversion guide (quickstart.md) and test data specifications.
