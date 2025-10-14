# Data Storage Requirements - Benchmark Forms

**Generated**: 2025-10-12
**System**: FrameworkQ.Easyforms
**Forms Analyzed**: 6

---

## Executive Summary

| Form ID | Title | Fields | Tables | Grids | Total Columns |
|---------|-------|--------|--------|-------|---------------|
| qf-gmd-17 | Surveillance Visit of Sub-Station | 5 | 1 | 0 | 4 |
| qf-gmd-06 | Consolidated statement of Sub-Station &  | 10 | 2 | 0 | 21 |
| qf-gmd-22 | Inspection & Maintenance of Power Transf | 17 | 1 | 0 | 5 |
| qf-gmd-14 | Monthly Shift Duty Roster | 9 | 1 | 0 | 33 |
| qf-gmd-19 | Sub-Station Daily Inspection Check Sheet | 2 | 3 | 0 | 2 |
| qf-gmd-01 | Log Sheet | 3 | 1 | 0 | 14 |

---

## QF-GMD-17: Surveillance Visit of Sub-Station

### Header Fields

| Field Name | Type | Label | Required | Est. Storage |
|------------|------|-------|----------|--------------|
| `office_name` | enum | Name of the Office / Sub-station: * | ✓ | 255 chars |
| `substation_id` | string | Sub-Station Identification No: |  | 255 chars |
| `reference_file` | string | Reference File: |  | 255 chars |
| `inspector_name` | string | Name & Designation: |  | 255 chars |
| `inspection_date` | datetime | Date & time inspection: |  | 8 bytes |

### Table: `None` (4 columns)

**Columns:**

| Column | Type | Label | Est. Storage |
|--------|------|-------|--------------|
| `sl_no` | None | Sl. No. | 255 chars |
| `item` | None | Item | 255 chars |
| `observation` | None | Observation | 255 chars |
| `remarks` | None | Remarks | 255 chars |

**Database Table:** `qf-gmd-17_page1_None`
**Total Columns:** 9 (5 system + 4 data)

---

## QF-GMD-06: Consolidated statement of Sub-Station & Transmission Line Performance

### Header Fields

| Field Name | Type | Label | Required | Est. Storage |
|------------|------|-------|----------|--------------|
| `substation` | enum | Substation: | ✓ | 255 chars |
| `substation_id_no` | string | Substation Identification No: |  | 255 chars |
| `reference_file` | string | Reference File: |  | 255 chars |
| `month` | string | Month: | ✓ | 255 chars |
| `substation_avail_pct` | integer |  |  | 4 bytes |
| `remarks_avail_sub` | string |  |  | 255 chars |
| `line_avail_pct` | integer |  |  | 4 bytes |
| `remarks_avail_line` | string |  |  | 255 chars |
| `reviewed_by_gmt1` | string | Reviewed by (GMT-1): |  | 255 chars |
| `approved_by_dt` | string | Approved by (DT): |  | 255 chars |

### Table: `None` (10 columns)

**Columns:**

| Column | Type | Label | Est. Storage |
|--------|------|-------|--------------|
| `sl_no` | None | Sl.No. | 255 chars |
| `capacity` | None | Total Sub-stationcapacity(MVA) | 255 chars |
| `energy_interruption` | None | Amount ofEnergyInterruption(MkWh) | 255 chars |
| `remarks` | None | Remarks | 255 chars |
| `forced` | None | Forced | 255 chars |
| `scheduled` | None | Scheduled | 255 chars |
| `total` | None | Total | 255 chars |
| `upto_30min` | None | Upto 30minutes | 255 chars |
| `upto_1hr` | None | Upto 01hour | 255 chars |
| `more_1hr` | None | More than01 hour | 255 chars |

**Aggregate Functions:**

- `sum(forced)` → `None`
- `sum(scheduled)` → `None`
- `sum(total)` → `None`
- `sum(upto_30min)` → `None`
- `sum(upto_1hr)` → `None`
- `sum(more_1hr)` → `None`
- `sum(energy_interruption)` → `None`

**Database Table:** `qf-gmd-06_page1_None`
**Total Columns:** 15 (5 system + 10 data)

### Table: `None` (11 columns)

**Columns:**

| Column | Type | Label | Est. Storage |
|--------|------|-------|--------------|
| `sl_no` | None | Sl.No. | 255 chars |
| `line_length` | None | Total lengthofTransmissionline(Ckt. Km.) | 255 chars |
| `sections` | None | No. of SectionofTransmissionline | 255 chars |
| `energy_interruption_line` | None | Amount ofEnergyInterruption(MkWh) | 255 chars |
| `remarks_line` | None | Remarks | 255 chars |
| `forced_line` | None | Forced | 255 chars |
| `scheduled_line` | None | Scheduled | 255 chars |
| `total_line` | None | Total | 255 chars |
| `upto_30min_line` | None | Upto 30minutes | 255 chars |
| `upto_1hr_line` | None | Upto 01hour | 255 chars |
| `more_1hr_line` | None | Morethan01hour | 255 chars |

**Aggregate Functions:**

- `sum(forced_line)` → `None`
- `sum(scheduled_line)` → `None`
- `sum(total_line)` → `None`
- `sum(upto_30min_line)` → `None`
- `sum(upto_1hr_line)` → `None`
- `sum(more_1hr_line)` → `None`
- `sum(energy_interruption_line)` → `None`

**Database Table:** `qf-gmd-06_page1_None`
**Total Columns:** 16 (5 system + 11 data)

---

## QF-GMD-22: Inspection & Maintenance of Power Transformer

### Header Fields

| Field Name | Type | Label | Required | Est. Storage |
|------------|------|-------|----------|--------------|
| `division` | string |  |  | 255 chars |
| `substation` | enum |  |  | 255 chars |
| `identification_no` | string |  |  | 255 chars |
| `inspection_date` | date |  | ✓ | 3 bytes |
| `bay_location` | string |  |  | 255 chars |
| `reference_file` | string |  |  | 255 chars |
| `make` | string |  |  | 255 chars |
| `type` | string |  |  | 255 chars |
| `sl_r` | string |  |  | 255 chars |
| `sl_y` | string |  |  | 255 chars |
| `sl_b` | string |  |  | 255 chars |
| `year` | string |  |  | 255 chars |
| `schedule_yes` | bool | Yes |  | 1 byte |
| `emergency_yes` | bool | Yes |  | 1 byte |
| `special_yes` | bool | Yes |  | 1 byte |
| `inspector_signature` | string |  |  | 255 chars |
| `approver_signature` | string |  |  | 255 chars |

### Table: `None` (5 columns)

**Columns:**

| Column | Type | Label | Est. Storage |
|--------|------|-------|--------------|
| `sl_no` | None | Sl. No | 255 chars |
| `item` | None | Item | 255 chars |
| `condition` | None | Condition | 255 chars |
| `action_taken` | None | Action Taken | 255 chars |
| `reference` | None | Reference | 255 chars |

**Database Table:** `qf-gmd-22_page1_None`
**Total Columns:** 10 (5 system + 5 data)

---

## QF-GMD-14: Monthly Shift Duty Roster

### Header Fields

| Field Name | Type | Label | Required | Est. Storage |
|------------|------|-------|----------|--------------|
| `grid_circle` | string |  |  | 255 chars |
| `gmd` | string |  |  | 255 chars |
| `substation` | enum |  |  | 255 chars |
| `roster_date` | date |  |  | 3 bytes |
| `reference_file` | string |  |  | 255 chars |
| `substation_id` | string |  |  | 255 chars |
| `junior_assistant_manager` | string |  |  | 255 chars |
| `deputy_manager` | string |  |  | 255 chars |
| `manager` | string |  |  | 255 chars |

### Table: `None` (33 columns)

**Columns:**

| Column | Type | Label | Est. Storage |
|--------|------|-------|--------------|
| `sl_no` | None | Sl.No. | 255 chars |
| `name` | None | Name | 255 chars |
| `day_1` | None | 1 | 255 chars |
| `day_2` | None | 2 | 255 chars |
| `day_3` | None | 3 | 255 chars |
| `day_4` | None | 4 | 255 chars |
| `day_5` | None | 5 | 255 chars |
| `day_6` | None | 6 | 255 chars |
| `day_7` | None | 7 | 255 chars |
| `day_8` | None | 8 | 255 chars |
| `day_9` | None | 9 | 255 chars |
| `day_10` | None | 10 | 255 chars |
| `day_11` | None | 11 | 255 chars |
| `day_12` | None | 12 | 255 chars |
| `day_13` | None | 13 | 255 chars |
| `day_14` | None | 14 | 255 chars |
| `day_15` | None | 15 | 255 chars |
| `day_16` | None | 16 | 255 chars |
| `day_17` | None | 17 | 255 chars |
| `day_18` | None | 18 | 255 chars |
| `day_19` | None | 19 | 255 chars |
| `day_20` | None | 20 | 255 chars |
| `day_21` | None | 21 | 255 chars |
| `day_22` | None | 22 | 255 chars |
| `day_23` | None | 23 | 255 chars |
| `day_24` | None | 24 | 255 chars |
| `day_25` | None | 25 | 255 chars |
| `day_26` | None | 26 | 255 chars |
| `day_27` | None | 27 | 255 chars |
| `day_28` | None | 28 | 255 chars |

**Database Table:** `qf-gmd-14_page1_None`
**Total Columns:** 38 (5 system + 33 data)

---

## QF-GMD-19: Sub-Station Daily Inspection Check Sheet

### Header Fields

| Field Name | Type | Label | Required | Est. Storage |
|------------|------|-------|----------|--------------|
| `reviewed_by` | string |  |  | 255 chars |
| `approved_by` | string |  |  | 255 chars |

### Table: `None` (2 columns)

**Columns:**

| Column | Type | Label | Est. Storage |
|--------|------|-------|--------------|
| `item_bn` | None | আইটেম | 255 chars |
| `condition_bn` | None | চালু / বন্ধ | 255 chars |

**Database Table:** `qf-gmd-19_page1_None`
**Total Columns:** 7 (5 system + 2 data)

### Table: `None` (0 columns)

**Database Table:** `qf-gmd-19_page1_None`
**Total Columns:** 5 (5 system + 0 data)

### Table: `None` (0 columns)

**Database Table:** `qf-gmd-19_page1_None`
**Total Columns:** 5 (5 system + 0 data)

---

## QF-GMD-01: Log Sheet

### Header Fields

| Field Name | Type | Label | Required | Est. Storage |
|------------|------|-------|----------|--------------|
| `signature_shift_a` | string |  |  | 255 chars |
| `signature_shift_b` | string |  |  | 255 chars |
| `station_in_charge` | string |  |  | 255 chars |

### Table: `None` (14 columns)

**Columns:**

| Column | Type | Label | Est. Storage |
|--------|------|-------|--------------|
| `tr1_kv` | None | kV | 255 chars |
| `tr1_mw` | None | MW | 255 chars |
| `tr1_mvar` | None | MVAR | 255 chars |
| `tr1_ampere` | None | Ampere | 255 chars |
| `tr1_tap` | None | TAP | 255 chars |
| `tr1_winding_oil` | None | WindingOilTemp | 255 chars |
| `tr1_temp_pf` | None | TempPF | 255 chars |
| `tr2_kv` | None | kV | 255 chars |
| `tr2_mw` | None | MW | 255 chars |
| `tr2_mvar` | None | MVAR | 255 chars |
| `tr2_ampere` | None | Ampere | 255 chars |
| `tr2_tap` | None | TAP | 255 chars |
| `tr2_winding_oil` | None | WindingOilTemp | 255 chars |
| `tr2_temp_pf` | None | TempPF | 255 chars |

**Database Table:** `qf-gmd-01_page1_None`
**Total Columns:** 19 (5 system + 14 data)

---

## Database Schema Summary

### Core Tables (Fixed)

1. **`forms`** - Form definitions
   - Columns: id, title, version, locales, html_source, schema_json, created_at, updated_at
   - Storage: ~50KB per form (includes HTML + JSON schema)

2. **`form_instances`** - Form submissions
   - Columns: instance_id, form_id, form_version, submitted_at, submitted_by, status, raw_data
   - Storage: ~10KB per submission (JSONB compressed)

### Reporting Tables (Dynamic)

Generated per table/grid widget:

**qf-gmd-17**: 1 reporting table(s)
**qf-gmd-06**: 2 reporting table(s)
**qf-gmd-22**: 1 reporting table(s)
**qf-gmd-14**: 1 reporting table(s)
**qf-gmd-19**: 3 reporting table(s)
**qf-gmd-01**: 1 reporting table(s)

**Total Reporting Tables:** 9

### Storage Estimates (per 1000 submissions)

| Form | Avg Rows/Submission | Est. Storage/1K | Notes |
|------|---------------------|-----------------|-------|
| qf-gmd-17 | 10 | 1953 KB | - |
| qf-gmd-06 | 10 | 10254 KB | 2 tables |
| qf-gmd-22 | 10 | 2441 KB | - |
| qf-gmd-14 | 10 | 16113 KB | - |
| qf-gmd-19 | 10 | 977 KB | 3 tables |
| qf-gmd-01 | 10 | 6836 KB | - |

---

## Data Type Distribution

| Data Type | Count | Percentage |
|-----------|-------|------------|
| None | 79 | 63.2% |
| string | 34 | 27.2% |
| enum | 4 | 3.2% |
| bool | 3 | 2.4% |
| integer | 2 | 1.6% |
| date | 2 | 1.6% |
| datetime | 1 | 0.8% |

---

## Recommendations

### Indexing Strategy

1. **Primary Keys:** All reporting tables should have composite PK (instance_id, row_index)
2. **Foreign Keys:** instance_id → form_instances.instance_id
3. **Search Indexes:**
   - Date fields (for temporal queries)
   - Enum fields with high cardinality (office_name, substation_name)
   - Frequently filtered columns

### Partitioning

- Consider partitioning reporting tables by year/month for forms with high submission volume
- Archive old submissions to separate tables/database

### Optimization

- Use appropriate VARCHAR lengths (not always 255)
- Consider COMPUTED columns for frequently queried calculations
- Use table compression for historical data
- Implement data retention policies

