# Client Runtime Contract: jQuery FormRuntimeHTMLDSL

**Feature**: 001-spec-in-in | **Date**: 2025-10-11
**Phase**: 1 (Design)

## Overview

This document defines the client-side runtime contract for the HTMLDSL Form System. The runtime is implemented in pure jQuery and provides initialization, state management, validation, expression evaluation, and dynamic behavior for HTML form templates.

---

## Initialization

### FormRuntimeHTMLDSL.mount()

Initializes the runtime on a form element.

```javascript
/**
 * Mount the form runtime on a form element
 * @param {HTMLFormElement|jQuery|string} formElement - Form element, jQuery object, or selector
 * @param {Object} options - Configuration options
 * @returns {FormRuntime} - Runtime instance
 */
FormRuntimeHTMLDSL.mount(formElement, options);
```

**Parameters:**

| Name | Type | Required | Description |
|------|------|----------|-------------|
| formElement | HTMLFormElement\|jQuery\|string | Yes | Form to initialize |
| options | Object | No | Configuration options |

**Options:**

```javascript
{
    apiBaseUrl: '/api',                 // Base URL for API calls
    autoSave: false,                    // Auto-save drafts
    autoSaveInterval: 30000,            // Auto-save interval (ms)
    validateOn: 'change',               // Validation trigger: 'input'|'change'|'blur'|'submit'
    showInlineErrors: true,             // Show inline error messages
    showErrorSummary: true,             // Show error summary panel
    errorSummarySelector: '[data-error-summary]',
    fetchTimeout: 5000,                 // External fetch timeout (ms)
    fetchRetries: 2,                    // Number of retries for failed fetches
    debug: false                        // Enable debug logging
}
```

**Example:**

```javascript
$(document).ready(function() {
    var formRuntime = FormRuntimeHTMLDSL.mount('#my-form', {
        apiBaseUrl: '/api',
        validateOn: 'change',
        autoSave: true,
        autoSaveInterval: 60000
    });
});
```

---

## Public API Methods

### formRuntime.getValue()

Get current form data as JSON.

```javascript
/**
 * Get current form values
 * @param {boolean} includeComputed - Include computed field values
 * @returns {Object} - Form data object
 */
formRuntime.getValue(includeComputed = true);
```

**Returns:**

```javascript
{
    "form_id": "qf-gmd-01",
    "form_version": "1.0",
    "field_name": "value",
    "table_id": [
        {"col1": "value1", "col2": "value2"},
        {"col1": "value3", "col2": "value4"}
    ]
}
```

---

### formRuntime.setValue()

Set form values programmatically.

```javascript
/**
 * Set form values
 * @param {Object} data - Data object matching form structure
 * @param {boolean} triggerChange - Fire change events
 */
formRuntime.setValue(data, triggerChange = true);
```

**Example:**

```javascript
formRuntime.setValue({
    "substation": "Sub-1",
    "month": "2025-10",
    "measurements": [
        {"forced": 10, "scheduled": 5},
        {"forced": 8, "scheduled": 3}
    ]
});
```

---

### formRuntime.validate()

Trigger validation on entire form or specific field.

```javascript
/**
 * Validate form or field
 * @param {string} fieldName - Optional field name to validate (validates all if omitted)
 * @returns {ValidationResult}
 */
formRuntime.validate(fieldName);
```

**Returns:**

```javascript
{
    valid: true|false,
    errors: {
        "field_name": ["Error message 1", "Error message 2"],
        "table_id.row_0.col": ["Row-level error"]
    }
}
```

---

### formRuntime.submit()

Submit form data to server.

```javascript
/**
 * Submit form
 * @param {Object} options - Submission options
 * @returns {Promise<SubmissionResult>}
 */
formRuntime.submit(options);
```

**Options:**

```javascript
{
    status: 'submitted',        // 'draft' or 'submitted'
    validate: true,             // Run validation before submit
    url: '/api/submissions'     // Override default submission URL
}
```

**Returns Promise:**

```javascript
{
    success: true,
    instanceId: "uuid",
    submittedAt: "2025-10-11T10:00:00Z"
}
```

**Example:**

```javascript
formRuntime.submit({status: 'submitted'})
    .then(function(result) {
        console.log('Submitted:', result.instanceId);
    })
    .catch(function(error) {
        console.error('Submission failed:', error);
    });
```

---

### formRuntime.saveDraft()

Save current state as draft.

```javascript
/**
 * Save draft
 * @returns {Promise<SubmissionResult>}
 */
formRuntime.saveDraft();
```

---

### formRuntime.reset()

Reset form to initial state.

```javascript
/**
 * Reset form
 * @param {boolean} clearData - Clear all data (default: restore initial values)
 */
formRuntime.reset(clearData = false);
```

---

### formRuntime.addTableRow()

Add row to a table widget.

```javascript
/**
 * Add table row
 * @param {string} tableId - Table widget ID
 * @param {Object} rowData - Initial row data
 * @param {number} index - Insert position (default: append)
 */
formRuntime.addTableRow(tableId, rowData, index);
```

**Example:**

```javascript
formRuntime.addTableRow('measurements', {
    forced: 0,
    scheduled: 0
});
```

---

### formRuntime.removeTableRow()

Remove row from table widget.

```javascript
/**
 * Remove table row
 * @param {string} tableId - Table widget ID
 * @param {number} index - Row index to remove
 */
formRuntime.removeTableRow(tableId, index);
```

---

### formRuntime.destroy()

Destroy runtime and cleanup.

```javascript
/**
 * Destroy runtime
 */
formRuntime.destroy();
```

---

## Events

The runtime emits custom events on the form element that can be listened to:

### formdsl:ready

Fired when runtime initialization is complete.

```javascript
$form.on('formdsl:ready', function(event, runtime) {
    console.log('Form runtime ready');
});
```

---

### formdsl:change

Fired when any field value changes.

```javascript
$form.on('formdsl:change', function(event, data) {
    console.log('Field changed:', data.fieldName, data.newValue);
});
```

**Event Data:**

```javascript
{
    fieldName: "string",
    oldValue: any,
    newValue: any,
    element: HTMLElement
}
```

---

### formdsl:validation

Fired when validation runs.

```javascript
$form.on('formdsl:validation', function(event, result) {
    if (!result.valid) {
        console.log('Validation errors:', result.errors);
    }
});
```

---

### formdsl:submit

Fired when form is submitted.

```javascript
$form.on('formdsl:submit', function(event, data) {
    console.log('Submitting:', data);
    // Return false to cancel submission
});
```

---

### formdsl:submit:success

Fired on successful submission.

```javascript
$form.on('formdsl:submit:success', function(event, result) {
    console.log('Submission successful:', result.instanceId);
});
```

---

### formdsl:submit:error

Fired on submission failure.

```javascript
$form.on('formdsl:submit:error', function(event, error) {
    console.error('Submission failed:', error);
});
```

---

### formdsl:table:row:add

Fired when table row is added.

```javascript
$form.on('formdsl:table:row:add', function(event, data) {
    console.log('Row added to', data.tableId, 'at index', data.index);
});
```

---

### formdsl:table:row:remove

Fired when table row is removed.

```javascript
$form.on('formdsl:table:row:remove', function(event, data) {
    console.log('Row removed from', data.tableId, 'at index', data.index);
});
```

---

## Expression Evaluation

The runtime evaluates expressions in `data-compute`, `data-when`, and `data-agg` attributes.

### Supported Operators

- **Arithmetic**: `+`, `-`, `*`, `/`, `%`
- **Comparison**: `==`, `!=`, `<`, `>`, `<=`, `>=`
- **Logical**: `&&`, `||`, `!`
- **Grouping**: `(`, `)`

### Supported Functions

| Function | Description | Example |
|----------|-------------|---------|
| `sum(col)` | Sum of table column | `sum(forced)` |
| `avg(col)` | Average of column | `avg(voltage)` |
| `min(col)` | Minimum value | `min(temperature)` |
| `max(col)` | Maximum value | `max(temperature)` |
| `count()` | Row count | `count()` |
| `round(n, decimals)` | Round number | `round(3.14159, 2)` |
| `abs(n)` | Absolute value | `abs(-5)` |
| `ceil(n)` | Ceiling | `ceil(3.2)` |
| `floor(n)` | Floor | `floor(3.8)` |

### Field References

- **Same scope**: Use field name directly (e.g., `forced + scheduled`)
- **Global scope from row**: Use `ctx.` prefix (e.g., `ctx.substation`)

### Examples

```html
<!-- Computed field -->
<input name="total" data-compute="forced + scheduled" data-readonly>

<!-- Conditional visibility -->
<div data-when="voltage_kv > 0">
    <label>Current (A)</label>
    <input name="current_a">
</div>

<!-- Table aggregate -->
<td data-agg="sum(total)" data-target="#grand_total"></td>
```

---

## Validation

### Native Validation Attributes

The runtime respects native HTML5 validation:

- `required`
- `pattern`
- `min`, `max`
- `step`
- `minlength`, `maxlength`

### Custom Validation Attributes

- `data-required="true|false"`
- `data-required-when="expression"` - Conditional required
- `data-pattern="regex"`
- `data-min="number"`
- `data-max="number"`
- `data-constraint="expression"` - Cross-field constraint
- `data-constraint-message="Custom error message"`

### Error Message Attributes

- `data-error` - Generic error message
- `data-error-required` - Required field error
- `data-error-pattern` - Pattern mismatch error
- `data-error-min` - Below minimum error
- `data-error-max` - Above maximum error

### Error Display

Errors are displayed using Bootstrap-compatible classes:

```html
<div class="form-group">
    <label for="voltage">Voltage (kV)</label>
    <input id="voltage" name="voltage" class="form-control is-invalid">
    <div class="invalid-feedback">Voltage must be between 0 and 500</div>
</div>
```

---

## External Data Fetching

### data-fetch Attribute

Fetches options from external APIs:

```html
<select id="breaker" name="breaker"
        data-fetch="GET:/api/proxy/fetch?endpoint=/api/breakers?substation={substation}"
        data-fetch-on="focus"
        data-fetch-map="value:id,label:name"
        data-fetch-cache="ttl:10m">
</select>
```

### Attributes

| Attribute | Description | Example |
|-----------|-------------|---------|
| `data-fetch` | HTTP method and URL | `GET:/api/endpoint` |
| `data-fetch-on` | Trigger event | `load|focus|input|change` |
| `data-min-chars` | Min chars for autocomplete | `1` |
| `data-fetch-debounce` | Debounce delay | `300ms` |
| `data-fetch-map` | Response field mapping | `value:id,label:name` |
| `data-fetch-cache` | Cache strategy | `ttl:10m|session|none` |
| `data-depends` | Parent field IDs | `#substation,#feeder` |

### Token Substitution

URLs can contain tokens that are replaced with field values:

- `{fieldName}` - Value of field with name="fieldName"
- `{search}` - Current search text (for autocomplete)

---

## Table Management

### Dynamic Row Addition

For tables with `data-row-mode="infinite"`:

```html
<table data-table id="measurements" data-row-mode="infinite" data-allow-add-rows="true">
    <thead>
        <tr>
            <th data-col="timestamp">Time</th>
            <th data-col="value">Value</th>
        </tr>
    </thead>
    <tbody>
        <tr data-row-template>
            <td><input name="timestamp" type="time"></td>
            <td><input name="value" type="number"></td>
        </tr>
    </tbody>
    <tfoot>
        <tr>
            <td colspan="2">
                <button type="button" class="btn-add-row">Add Row</button>
            </td>
        </tr>
    </tfoot>
</table>
```

The runtime automatically:
- Clones row template
- Updates input names with row index
- Binds add/remove row buttons
- Recalculates aggregates

---

## Print Support

### CSS @media print

The runtime generates print styles from data attributes:

```html
<form data-form="qf-gmd-01"
      data-print-page-size="A4"
      data-print-margins-mm="10,10,10,10"
      data-print-orientation="portrait">
    <!-- Form content -->
</form>
```

Generates:

```css
@media print {
    @page {
        size: A4 portrait;
        margin: 10mm;
    }
}
```

---

## Error Handling

### Client-Side Errors

The runtime emits errors via events:

```javascript
$form.on('formdsl:error', function(event, error) {
    console.error('Runtime error:', error.code, error.message);
});
```

### Server-Side Errors

API errors are returned in standard format:

```javascript
{
    "error": {
        "code": "VALIDATION_ERROR",
        "message": "Form validation failed",
        "correlationId": "abc123",
        "details": {
            "field_name": ["Error message"]
        }
    }
}
```

---

## Browser Compatibility

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

**Required Polyfills for Older Browsers:**

- Promise (for IE11)
- Fetch API (for IE11)

---

## Example: Complete Form Initialization

```html
<!DOCTYPE html>
<html>
<head>
    <title>HTMLDSL Form Example</title>
    <link rel="stylesheet" href="/css/forms.css">
    <link rel="stylesheet" href="/css/print.css" media="print">
    <script src="/js/jquery-3.6.0.min.js"></script>
    <script src="/js/formruntime.js"></script>
</head>
<body>
    <form id="my-form" data-form="qf-gmd-01" data-title="Log Sheet" data-version="1.0">
        <section data-page id="page-1" data-title="Main">
            <section data-section id="header">
                <label for="substation">Substation</label>
                <input id="substation" name="substation" required>

                <label for="date">Date</label>
                <input id="date" name="date" type="date" required>
            </section>

            <section data-section id="measurements">
                <table data-table id="readings" data-row-mode="infinite" data-allow-add-rows="true">
                    <thead>
                        <tr>
                            <th data-col="time">Time</th>
                            <th data-col="voltage">Voltage (kV)</th>
                            <th data-col="current">Current (A)</th>
                            <th data-col="power" data-formula="voltage * current / 1000">Power (MW)</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr data-row-template>
                            <td><input name="time" type="time"></td>
                            <td><input name="voltage" type="number" step="0.1"></td>
                            <td><input name="current" type="number" step="0.1"></td>
                            <td><input name="power" type="number" step="0.001" readonly></td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan="3">Total Power</td>
                            <td data-agg="sum(power)" data-target="#total_power"></td>
                        </tr>
                    </tfoot>
                </table>
            </section>
        </section>

        <button type="button" id="save-draft">Save Draft</button>
        <button type="submit">Submit</button>
    </form>

    <div data-error-summary></div>

    <script>
        $(document).ready(function() {
            var formRuntime = FormRuntimeHTMLDSL.mount('#my-form', {
                apiBaseUrl: '/api',
                validateOn: 'change',
                autoSave: true,
                autoSaveInterval: 60000
            });

            // Event handlers
            formRuntime.$form.on('formdsl:ready', function() {
                console.log('Form ready');
            });

            formRuntime.$form.on('formdsl:change', function(e, data) {
                console.log('Field changed:', data.fieldName);
            });

            // Save draft button
            $('#save-draft').on('click', function() {
                formRuntime.saveDraft()
                    .then(function(result) {
                        alert('Draft saved: ' + result.instanceId);
                    })
                    .catch(function(error) {
                        alert('Save failed: ' + error.message);
                    });
            });

            // Submit button
            $('#my-form').on('submit', function(e) {
                e.preventDefault();
                formRuntime.submit()
                    .then(function(result) {
                        alert('Submitted: ' + result.instanceId);
                        window.location.href = '/submissions/' + result.instanceId;
                    })
                    .catch(function(error) {
                        alert('Submission failed: ' + error.message);
                    });
            });
        });
    </script>
</body>
</html>
```

---

**Next Steps**: Use this contract as specification for implementing the jQuery runtime modules.
