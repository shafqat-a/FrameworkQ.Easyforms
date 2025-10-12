# Benchmark Forms - Power Grid Company of Bangladesh

This directory contains 6 converted benchmark forms that validate the HTMLDSL Form System's production readiness.

## The 6 Benchmark Forms

| Form ID | Title | Complexity | Status |
|---------|-------|------------|--------|
| QF-GMD-17 | Surveillance Visit of Sub-Station | Simple | ⏳ Pending |
| QF-GMD-06 | Consolidated Performance Report | Moderate | ⏳ Pending |
| QF-GMD-22 | Transformer Inspection | Moderate | ⏳ Pending |
| QF-GMD-14 | Monthly Shift Duty Roster | Moderate | ⏳ Pending |
| QF-GMD-19 | Daily Inspection (Bengali) | Complex | ⏳ Pending |
| QF-GMD-01 | Log Sheet | Very Complex | ⏳ Pending |

## Validation Checklist

For each form:

- [ ] Visual fidelity: 95%+ match with original
- [ ] Functional: All calculations work
- [ ] Print: PDF matches original layout
- [ ] Schema: Extracts without errors
- [ ] E2E: Submit and query workflow complete

## How to Use

### View Forms
Open any `.html` file in a web browser to see the rendered form.

### Test Forms
All forms are connected to the HTMLDSL API at `http://localhost:5000`.

### Upload to System
```bash
curl -X POST http://localhost:5000/v1/forms \
  -F "htmlFile=@qf-gmd-##-name.html"
```

### Submit Test Data
```bash
curl -X POST http://localhost:5000/v1/submissions \
  -H "Content-Type: application/json" \
  -d @test-data/qf-gmd-##-sample.json
```

## Original Screenshots

Original paper forms are in `/docs/benchmark/`:
- Screenshot 2025-10-01 at 6.16.21 AM.png - QF-GMD-06
- Screenshot 2025-10-01 at 6.19.25 AM.png - QF-GMD-14
- Screenshot 2025-10-01 at 6.20.23 AM.png - QF-GMD-01
- Screenshot 2025-10-01 at 6.21.25 AM.png - QF-GMD-17
- Screenshot 2025-10-01 at 6.21.51 AM.png - QF-GMD-19
- Screenshot 2025-10-01 at 6.22.06 AM.png - QF-GMD-22

## Success Criteria

- ✅ All 6 forms converted
- ✅ 95%+ visual fidelity
- ✅ 100% functional accuracy
- ✅ 100% schema extraction success
- ✅ Production-ready validation

## Notes

These forms represent real-world enterprise use cases from Bangladesh Power Grid Company's Quality Management System. Successful conversion proves the HTMLDSL system can handle complex regulatory and operational forms.
