# HTMLDSL for Formflow

HTML-first design language for pixel-perfect forms using standard, proven web tech:
- HTML5 for structure and exact visual layout
- CSS for pixel-perfect styling and print fidelity
- data-* attributes for machine-readable schema, validation, formulas, and behavior
- JavaScript hooks for interactivity, dynamic data, and submission

This folder defines the HTMLDSL spec, examples, and mapping guidelines to storage and APIs.

## Why HTMLDSL
- Pixel-perfect: Designers control exact layout with semantic HTML + CSS.
- Zero new syntax: Leverages native HTML validation and attributes; extend via data-*.
- Direct preview: What you see in HTML is what the runtime renders.
- Easy theming/print: CSS handles enterprise styling and print media.
- Extractable schema: A parser can derive a canonical JSON data model and SQL DDL from data-*.

## Contents
- spec.md — normative specification (attributes, structure, mapping)
- examples/ — small, focused HTML examples demonstrating widgets and features

## Quick Glance
```html
<form data-form="qf-gmd-01" data-version="1.0" data-title="Log Sheet">
  <section data-page id="page-1" data-title="Page 1">
    <section data-section id="header" data-title="Header">
      <table data-widget="formheader"></table>
    </section>
    <section data-section id="main" data-title="Main">
      <label for="meter_reading">Meter Reading</label>
      <input id="meter_reading" name="meter_reading" type="number"
             data-type="integer" data-required="true" data-min="0" />

      <table data-table id="measurements" data-row-mode="infinite" data-allow-add-rows="true">
        <thead>
          <tr>
            <th data-col="timestamp" data-type="datetime" data-label="Time"></th>
            <th data-col="voltage_kv" data-type="decimal" data-label="Voltage (kV)"></th>
            <th data-col="current_a" data-type="decimal" data-label="Current (A)"></th>
            <th data-col="power_mw" data-type="decimal" data-label="Power (MW)"
                data-formula="voltage_kv * current_a / 1000"></th>
          </tr>
        </thead>
        <tbody>
          <tr data-row-template>
            <td><input name="timestamp" type="datetime-local"></td>
            <td><input name="voltage_kv" type="number" step="0.01"></td>
            <td><input name="current_a" type="number" step="0.01"></td>
            <td><input name="power_mw" type="number" step="0.001" data-computed></td>
          </tr>
        </tbody>
        <tfoot>
          <tr>
            <td colspan="3" class="text-end">Total Energy (MkWh)</td>
            <td data-agg="sum(power_mw)" data-target="#total_energy"></td>
          </tr>
        </tfoot>
      </table>

      <label for="total_energy">Total Energy</label>
      <input id="total_energy" name="total_energy" type="number" data-readonly>
    </section>
  </section>
  <!-- Submission and scripting handled by JS runtime -->
  <button type="submit" hidden></button>
  <script src="/js/formruntime-htmldsl.js"></script>
  <script>FormRuntimeHTMLDSL.mount(document.currentScript.closest('form'))</script>
  <!-- or use server-side extraction to DTO before runtime -->
</form>
```

See spec.md for full details.

