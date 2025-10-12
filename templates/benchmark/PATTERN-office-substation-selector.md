# Pattern: Office/Substation Cascading Selector

## Overview

This pattern implements a cascading dropdown for Office/Sub-station selection using existing HTMLDSL Field widgets with `data-fetch` and `data-depends` attributes (US6 capabilities).

**No new widget type required** - leverages existing system.

## HTML Pattern

```html
<div data-group id="office-selector-group" data-layout="columns:2">
    <!-- Primary: Office/Substation Dropdown -->
    <div style="grid-column: 1 / -1;">
        <label for="office_name">Name of the Office / Sub-station: *</label>
        <select id="office_name"
                name="office_name"
                data-type="enum"
                data-fetch="GET:/api/proxy/fetch?endpoint=/api/offices"
                data-fetch-on="focus,load"
                data-fetch-map="value:id,label:name"
                data-fetch-cache="ttl:30m"
                required>
            <option value="">-- Select Office/Sub-station --</option>
        </select>
    </div>

    <!-- Dependent: Substation ID (auto-fills based on office) -->
    <div>
        <label for="substation_id">Sub-Station Identification No:</label>
        <input id="substation_id"
               name="substation_id"
               type="text"
               data-depends="#office_name"
               data-fetch="GET:/api/proxy/fetch?endpoint=/api/substation-id?office={office_name}"
               data-fetch-on="change">
    </div>

    <!-- Optional: Reference File -->
    <div>
        <label for="reference_file">Reference File:</label>
        <input id="reference_file"
               name="reference_file"
               type="text">
    </div>
</div>
```

## Required Scripts

```html
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script src="../../frontend/src/js/data-fetch.js"></script>
<script src="../../frontend/src/js/formruntime.js"></script>
```

## How It Works

1. **User focuses on office dropdown** → `data-fetch-on="focus"` triggers
2. **API call via proxy** → `GET /api/proxy/fetch?endpoint=/api/offices`
3. **Response mapped** → `data-fetch-map="value:id,label:name"`
4. **Options populated** → Dropdown shows available offices
5. **User selects office** → Change event fires
6. **Dependent field triggered** → `data-depends="#office_name"` detects change
7. **Substation ID fetched** → Token `{office_name}` replaced with selected value
8. **Auto-populated** → Substation ID field updates

## API Requirements

### `/api/offices` Endpoint

**Response format**:
```json
[
  {"id": "gmd-dhaka", "name": "GMD, Dhaka"},
  {"id": "gmd-chittagong", "name": "GMD, Chittagong"},
  {"id": "sub-dhanmondi", "name": "Sub-station, Dhanmondi"},
  {"id": "sub-gulshan", "name": "Sub-station, Gulshan"}
]
```

### `/api/substation-id` Endpoint

**Request**: `GET /api/substation-id?office=gmd-dhaka`

**Response format**:
```json
[
  {"id": "SS-DHA-001", "name": "SS-DHA-001"}
]
```

Or for auto-fill (single value):
```json
{"value": "SS-DHA-001"}
```

## For Demo/Testing (Without API)

Add this JavaScript to populate with mock data:

```javascript
// Demo data population (remove in production)
setTimeout(function() {
    $('#office_name').append('<option value="gmd-dhaka">GMD, Dhaka</option>');
    $('#office_name').append('<option value="gmd-chittagong">GMD, Chittagong</option>');
    $('#office_name').append('<option value="sub-dhanmondi">Sub-station, Dhanmondi</option>');
    $('#office_name').append('<option value="sub-gulshan">Sub-station, Gulshan</option>');
}, 500);
```

## Reusability

This pattern can be used in **all 6 benchmark forms**:
- ✅ QF-GMD-17 (implemented)
- 📝 QF-GMD-06 (needs office/substation)
- 📝 QF-GMD-01 (needs office/substation)
- 📝 QF-GMD-14 (needs GMD/Grid Circle)
- 📝 QF-GMD-19 (needs office/substation)
- 📝 QF-GMD-22 (needs Division/Substation)

## Customization

### Different Field Names
Just change `name` attributes:
```html
<select name="division">  <!-- Instead of office_name -->
<select name="grid_circle">  <!-- Instead of office_name -->
```

### Different API Endpoints
```html
data-fetch="GET:/api/proxy/fetch?endpoint=/api/divisions"
data-fetch="GET:/api/proxy/fetch?endpoint=/api/grid-circles"
```

### Three-Level Cascade
```html
<!-- Level 1: Division -->
<select name="division" data-fetch="GET:/api/divisions"></select>

<!-- Level 2: Office (depends on division) -->
<select name="office"
        data-depends="#division"
        data-fetch="GET:/api/offices?division={division}"></select>

<!-- Level 3: Substation (depends on office) -->
<select name="substation"
        data-depends="#office"
        data-fetch="GET:/api/substations?office={office}"></select>
```

## Benefits of Option A (Field-Based Approach)

✅ **Zero code changes** - works with existing HTMLDSL system
✅ **Proven pattern** - data-fetch implemented and tested in US6
✅ **Flexible** - can cascade 2, 3, or more levels
✅ **Fast** - implement in minutes, not hours
✅ **Constitution compliant** - no PR needed for new widget type
✅ **Reusable** - copy/paste pattern across forms
✅ **Testable** - already has tests from US6

## When to Upgrade to Custom Widget (Option B/C)

Consider creating a dedicated widget type if:
- Used in 10+ different forms
- Needs complex validation rules specific to office/substation
- Requires special schema extraction logic
- Team wants explicit `data-widget="office-substation-selector"` in HTML

For now, **Option A is perfect** - it's working, tested, and requires no code changes!

## Example in Action

See: `templates/benchmark/qf-gmd-17-surveillance-checklist.html`
- Line 83-95: Office dropdown with data-fetch
- Line 98-108: Substation ID with data-depends
- Line 527: data-fetch.js script included
- Line 541-555: Demo data population
