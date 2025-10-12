# Refactoring Guide: Remove Inline Styles

## Status

✅ **Single CSS file created**: `benchmark-forms.css`
✅ **QF-GMD-17**: CSS link updated (styles removed from `<head>`)
⚠️ **Remaining**: Remove inline `style=""` attributes from all 6 forms

## What Needs to Be Done

Replace all inline `style=""` attributes with CSS classes from `benchmark-forms.css`.

### **Common Inline Style Replacements**

| Inline Style | Replace With | CSS Class |
|--------------|--------------|-----------|
| `style="width: 100%;"` | (remove) | Default for inputs |
| `style="text-align: center;"` | `class="center-text"` | `.center-text` |
| `style="text-align: right;"` | `class="right-text"` | `.right-text` |
| `style="font-weight: bold;"` | `<strong>` or `<b>` | Semantic HTML |
| `style="margin-top: 20px;"` | `class="signature-space"` | `.signature-space` |
| `style="grid-column: 1 / -1;"` | `class="field-full-width"` | `.field-full-width` |
| `style="border-bottom: 1px solid #000; ..."` | `class="signature-input"` | `.signature-input` |

### **Form-Specific Replacements**

#### **All Forms - Header Table**
```html
<!-- Before -->
<td rowspan="2" style="width: 20%; text-align: center; font-weight: bold;">

<!-- After -->
<td rowspan="2" class="header-qms">
```

#### **All Forms - Signature Inputs**
```html
<!-- Before -->
<input style="width: 80%; border-bottom: 1px solid #000; border-top: none; border-left: none; border-right: none; text-align: center;">

<!-- After -->
<input class="signature-input">
```

#### **QF-GMD-06, QF-GMD-14 - Layout Grids**
```html
<!-- Before -->
<div data-group data-layout="columns:2" style="margin-top: 20px;">

<!-- After -->
<div data-group data-layout="columns:2" class="field-group-2col">
```

### **Search & Replace Patterns**

Use these regex patterns in your editor:

```regex
# Remove width: 100% from inputs
style="width: 100%;"
→ (delete)

# Replace centered text
style="text-align: center;"
→ class="center-text"

# Replace signature inputs
style="width: 80%; border-bottom: 1px solid #000; border-top: none; border-left: none; border-right: none; text-align: center;"
→ class="signature-input"

# Remove dotted underlines (handled in CSS)
style="width: [^"]*; border: none; border-bottom: 1px dotted #[0-9a-f]{3,6};"
→ class="dotted-underline"
```

## Files to Update

All 6 forms need inline styles removed:

- [ ] `qf-gmd-17-surveillance-checklist.html` - HEAD updated ✅, BODY needs style removal
- [ ] `qf-gmd-06-performance-report.html` - Needs full update
- [ ] `qf-gmd-22-transformer-inspection.html` - Needs full update
- [ ] `qf-gmd-14-shift-roster.html` - Needs full update
- [ ] `qf-gmd-19-daily-inspection-bengali.html` - Needs full update
- [ ] `qf-gmd-01-log-sheet.html` - Needs full update

## Quick Fix Script (Optional)

```bash
# For each form, replace common patterns
for file in templates/benchmark/qf-gmd-*.html; do
    # Remove width: 100%
    sed -i '' 's/ style="width: 100%;"//g' "$file"

    # Replace text-align: center
    sed -i '' 's/style="text-align: center;"/class="center-text"/g' "$file"

    # Update CSS link
    sed -i '' 's|../../frontend/src/css/forms.css|benchmark-forms.css|g' "$file"
    sed -i '' 's|../../frontend/src/css/print.css||g' "$file"
done
```

## Validation After Refactoring

1. **Visual Check**: Forms should look identical (CSS provides all styling)
2. **Functional Check**: All interactions still work
3. **Print Check**: Print preview unchanged
4. **Upload Check**: Schema extraction still works

## Benefits

✅ **Single source of truth** for styling
✅ **Easier maintenance** - update CSS once, affects all forms
✅ **Cleaner HTML** - semantic and readable
✅ **Better performance** - CSS cached by browser
✅ **Compliance** - Separation of concerns (content vs. presentation)
