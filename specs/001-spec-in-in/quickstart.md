# Quickstart Guide: HTMLDSL Form System

**Feature**: 001-spec-in-in | **Date**: 2025-10-11

## Overview

This guide provides a quick introduction to building, deploying, and using the HTMLDSL Form System. Follow these steps to go from zero to a working form system in under 30 minutes.

---

## Prerequisites

- .NET Core SDK (LTS version)
- Node.js and npm (for frontend tooling, optional)
- SQL Server or PostgreSQL database
- Modern web browser (Chrome, Firefox, Safari, or Edge)
- Basic knowledge of HTML, CSS, and C#

---

## Step 1: Setup Development Environment

### Clone the Repository

```bash
git clone https://github.com/your-org/FrameworkQ.Easyforms.git
cd FrameworkQ.Easyforms
```

### Install Backend Dependencies

```bash
cd backend/src
dotnet restore
```

### Configure Database Connection

Edit `backend/src/FrameworkQ.Easyforms.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=localhost;Database=EasyformsDb;Trusted_Connection=True;",
    "PostgreSQL": "Host=localhost;Database=easyforms_db;Username=postgres;Password=yourpassword"
  },
  "DatabaseProvider": "sqlserver",
  "AllowedOrigins": ["http://localhost:3000"]
}
```

### Create Database

**SQL Server:**
```sql
CREATE DATABASE EasyformsDb;
```

**PostgreSQL:**
```bash
createdb easyforms_db
```

---

## Step 2: Build and Run Backend

### Build Projects

```bash
cd backend/src
dotnet build
```

### Run Tests (Optional)

```bash
cd backend/tests
dotnet test
```

### Start API Server

```bash
cd backend/src/FrameworkQ.Easyforms.Api
dotnet run
```

The API should now be running at `http://localhost:5000`.

Verify by visiting: `http://localhost:5000/health`

---

## Step 3: Serve Frontend Files

### Option A: Simple HTTP Server (Development)

```bash
cd frontend/src
npx http-server -p 3000
```

Frontend now available at `http://localhost:3000`.

### Option B: Configure API to Serve Static Files

Edit `Program.cs`:

```csharp
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "../../frontend/src")),
    RequestPath = ""
});
```

Access forms at: `http://localhost:5000/index.html`

---

## Step 4: Create Your First Form

### 4.1 Create HTML Template

Create `templates/my-first-form.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Employee Feedback Form</title>
    <link rel="stylesheet" href="/css/forms.css">
</head>
<body>
    <form data-form="employee-feedback" data-title="Employee Feedback Form" data-version="1.0">

        <section data-page id="page-1" data-title="Main">

            <!-- Header Section -->
            <section data-section id="header" data-title="Header Information">
                <div data-group data-layout="columns:2">
                    <label for="employee_name">Employee Name</label>
                    <input id="employee_name" name="employee_name" type="text"
                           data-type="string" data-required="true">

                    <label for="department">Department</label>
                    <select id="department" name="department" data-type="enum" data-required="true">
                        <option value="">-- Select --</option>
                        <option value="engineering">Engineering</option>
                        <option value="sales">Sales</option>
                        <option value="hr">Human Resources</option>
                    </select>

                    <label for="review_date">Review Date</label>
                    <input id="review_date" name="review_date" type="date"
                           data-type="date" data-required="true">
                </div>
            </section>

            <!-- Ratings Section -->
            <section data-section id="ratings" data-title="Performance Ratings">
                <table data-table id="performance" data-row-mode="finite" data-min-rows="4" data-max-rows="4">
                    <thead>
                        <tr>
                            <th data-col="criteria" data-type="string" data-label="Criteria" data-width="40%">Criteria</th>
                            <th data-col="rating" data-type="integer" data-label="Rating (1-5)" data-min="1" data-max="5" data-required="true">Rating</th>
                            <th data-col="comments" data-type="text" data-label="Comments">Comments</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr data-row-template>
                            <td><input name="criteria" type="text" value="Communication Skills" readonly></td>
                            <td><input name="rating" type="number" min="1" max="5"></td>
                            <td><textarea name="comments" rows="2"></textarea></td>
                        </tr>
                        <tr data-row-template>
                            <td><input name="criteria" type="text" value="Technical Skills" readonly></td>
                            <td><input name="rating" type="number" min="1" max="5"></td>
                            <td><textarea name="comments" rows="2"></textarea></td>
                        </tr>
                        <tr data-row-template>
                            <td><input name="criteria" type="text" value="Teamwork" readonly></td>
                            <td><input name="rating" type="number" min="1" max="5"></td>
                            <td><textarea name="comments" rows="2"></textarea></td>
                        </tr>
                        <tr data-row-template>
                            <td><input name="criteria" type="text" value="Leadership" readonly></td>
                            <td><input name="rating" type="number" min="1" max="5"></td>
                            <td><textarea name="comments" rows="2"></textarea></td>
                        </tr>
                    </tbody>
                    <tfoot>
                        <tr>
                            <td>Average Rating</td>
                            <td data-agg="avg(rating)" data-target="#avg_rating">
                                <input id="avg_rating" type="number" step="0.1" readonly>
                            </td>
                            <td></td>
                        </tr>
                    </tfoot>
                </table>
            </section>

            <!-- Overall Comments -->
            <section data-section id="overall" data-title="Overall Assessment">
                <label for="strengths">Key Strengths</label>
                <textarea id="strengths" name="strengths" rows="3" data-type="text"></textarea>

                <label for="improvements">Areas for Improvement</label>
                <textarea id="improvements" name="improvements" rows="3" data-type="text"></textarea>

                <label for="recommendation">Recommendation</label>
                <select id="recommendation" name="recommendation" data-type="enum" data-required="true">
                    <option value="">-- Select --</option>
                    <option value="promotion">Promotion</option>
                    <option value="lateral_move">Lateral Move</option>
                    <option value="additional_training">Additional Training</option>
                    <option value="performance_plan">Performance Improvement Plan</option>
                </select>
            </section>

        </section>

        <!-- Form Actions -->
        <div style="margin-top: 20px;">
            <button type="button" id="save-draft" class="btn-secondary">Save Draft</button>
            <button type="submit" class="btn-primary">Submit</button>
        </div>
    </form>

    <!-- Error Summary -->
    <div data-error-summary></div>

    <!-- Scripts -->
    <script src="/js/jquery-3.6.0.min.js"></script>
    <script src="/js/formruntime.js"></script>
    <script src="/js/expression-evaluator.js"></script>
    <script src="/js/validation.js"></script>
    <script src="/js/table-manager.js"></script>
    <script src="/js/widgets/table.js"></script>

    <script>
        $(document).ready(function() {
            // Initialize form runtime
            var formRuntime = FormRuntimeHTMLDSL.mount('form[data-form]', {
                apiBaseUrl: 'http://localhost:5000/v1',
                validateOn: 'change',
                showInlineErrors: true
            });

            // Save draft handler
            $('#save-draft').on('click', function() {
                formRuntime.saveDraft()
                    .then(function(result) {
                        alert('Draft saved successfully! ID: ' + result.instanceId);
                    })
                    .catch(function(error) {
                        alert('Failed to save draft: ' + error.message);
                    });
            });

            // Submit handler
            $('form').on('submit', function(e) {
                e.preventDefault();

                formRuntime.submit({ status: 'submitted' })
                    .then(function(result) {
                        alert('Form submitted successfully! ID: ' + result.instanceId);
                        // Redirect or clear form
                        formRuntime.reset(true);
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

## Step 5: Upload Form to System

### 5.1 Upload via API

```bash
curl -X POST http://localhost:5000/v1/forms \
  -H "Content-Type: multipart/form-data" \
  -F "htmlFile=@templates/my-first-form.html"
```

**Response:**

```json
{
  "id": "employee-feedback",
  "title": "Employee Feedback Form",
  "version": "1.0",
  "schema": { /* extracted JSON schema */ }
}
```

### 5.2 Verify Form Uploaded

```bash
curl http://localhost:5000/v1/forms/employee-feedback
```

---

## Step 6: Generate Database Schema

### 6.1 Generate Schema via API

```bash
curl -X POST http://localhost:5000/v1/database/generate \
  -H "Content-Type: application/json" \
  -d '{
    "formId": "employee-feedback",
    "version": "1.0",
    "provider": "sqlserver",
    "dryRun": false
  }'
```

**Response:**

```json
{
  "success": true,
  "tablesCreated": [
    "employee_feedback_page_1_ratings_performance"
  ],
  "ddl": [
    "CREATE TABLE employee_feedback_page_1_ratings_performance (...)"
  ]
}
```

### 6.2 Verify Tables Created

**SQL Server:**
```sql
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME LIKE 'employee_feedback%';
```

**PostgreSQL:**
```sql
SELECT tablename FROM pg_tables
WHERE tablename LIKE 'employee_feedback%';
```

---

## Step 7: Fill Out and Submit Form

### 7.1 Open Form in Browser

Navigate to: `http://localhost:3000/templates/my-first-form.html`

### 7.2 Fill Form Fields

- Enter employee name: "John Doe"
- Select department: "Engineering"
- Enter review date: "2025-10-11"
- Rate performance criteria (1-5 for each)
- Add comments
- Select recommendation

### 7.3 Submit Form

Click "Submit" button. The form data will be:
1. Validated client-side
2. Sent to backend API
3. Validated server-side
4. Stored in `form_instances` table
5. Reporting table rows inserted

### 7.4 Verify Submission

```bash
curl http://localhost:5000/v1/query/submissions?formId=employee-feedback
```

**Response:**

```json
{
  "data": [
    {
      "instanceId": "uuid-here",
      "formId": "employee-feedback",
      "submittedAt": "2025-10-11T10:00:00Z",
      "status": "submitted"
    }
  ],
  "pagination": { /* pagination info */ }
}
```

---

## Step 8: Query Reporting Data

### 8.1 Query Performance Ratings Table

```bash
curl "http://localhost:5000/v1/query/reporting/employee_feedback_page_1_ratings_performance"
```

**Response:**

```json
{
  "data": [
    {
      "instance_id": "uuid",
      "row_index": 0,
      "criteria": "Communication Skills",
      "rating": 4,
      "comments": "Good verbal and written communication"
    },
    {
      "instance_id": "uuid",
      "row_index": 1,
      "criteria": "Technical Skills",
      "rating": 5,
      "comments": "Excellent technical knowledge"
    }
    /* ... more rows ... */
  ]
}
```

### 8.2 Direct Database Query

**SQL Server:**
```sql
SELECT * FROM employee_feedback_page_1_ratings_performance
WHERE instance_id = 'uuid-here';
```

---

## Step 9: Update Form (Schema Migration)

### 9.1 Modify HTML Template

Add a new field to `my-first-form.html`:

```html
<label for="employee_id">Employee ID</label>
<input id="employee_id" name="employee_id" type="text"
       data-type="string" data-required="true" data-pattern="^EMP[0-9]{4}$">
```

### 9.2 Upload Updated Form

```bash
curl -X PUT http://localhost:5000/v1/forms/employee-feedback \
  -H "Content-Type: multipart/form-data" \
  -F "htmlFile=@templates/my-first-form.html"
```

### 9.3 Migrate Database Schema

```bash
curl -X POST http://localhost:5000/v1/database/migrate \
  -H "Content-Type: application/json" \
  -d '{
    "formId": "employee-feedback",
    "fromVersion": "1.0",
    "toVersion": "1.1",
    "provider": "sqlserver",
    "dryRun": false
  }'
```

**Response:**

```json
{
  "success": true,
  "transformations": [
    {
      "type": "add",
      "field": "employee_id",
      "toType": "string"
    }
  ],
  "migrationSql": [
    "ALTER TABLE form_instances ADD employee_id NVARCHAR(255);"
  ]
}
```

---

## Step 10: Testing

### 10.1 Run Backend Tests

```bash
cd backend/tests
dotnet test --logger "console;verbosity=detailed"
```

### 10.2 Run Frontend Tests (if configured)

```bash
cd frontend/tests
npm test
```

### 10.3 Manual Testing Checklist

- [ ] Form loads without errors
- [ ] All fields are visible and interactive
- [ ] Client-side validation triggers correctly
- [ ] Computed fields update automatically
- [ ] Table rows can be added/removed (if applicable)
- [ ] Form submits successfully
- [ ] Data appears in database tables
- [ ] Reporting queries return correct data
- [ ] Print view renders correctly (Ctrl+P or Cmd+P)

---

## Common Issues and Solutions

### Issue: API Not Starting

**Solution:** Check port 5000 is not in use
```bash
# Windows
netstat -ano | findstr :5000

# Linux/Mac
lsof -i :5000
```

### Issue: Database Connection Failed

**Solution:** Verify connection string and database exists
```bash
# SQL Server
sqlcmd -S localhost -Q "SELECT @@VERSION"

# PostgreSQL
psql -h localhost -U postgres -c "SELECT version();"
```

### Issue: Form Not Parsing

**Solution:** Validate HTML structure
- Ensure `data-form` attribute is present
- Check all data-* attributes are correctly spelled
- Verify HTML is well-formed (no unclosed tags)

### Issue: jQuery Runtime Not Loading

**Solution:** Check browser console for errors
- Verify jQuery is loaded before formruntime.js
- Check file paths are correct
- Ensure CORS is configured if frontend/backend on different ports

### Issue: Validation Not Working

**Solution:**
- Check `data-required` attribute is present
- Verify validation messages are defined
- Ensure validation scripts are loaded
- Check browser console for JavaScript errors

---

## Next Steps

1. **Explore Examples**: Check `templates/examples/` for more complex form samples
2. **Read Documentation**: See [data-model.md](./data-model.md) and [contracts/](./contracts/) for detailed specs
3. **Customize Styling**: Edit `frontend/src/css/forms.css` for custom look and feel
4. **Add Custom Widgets**: Extend jQuery runtime with custom widget plugins
5. **Set Up Production**: Configure reverse proxy (nginx/IIS), SSL, and authentication
6. **Monitor Performance**: Set up logging and monitoring (Serilog + Seq/ELK)

---

## Additional Resources

- **API Documentation**: See [contracts/api-spec.yaml](./contracts/api-spec.yaml)
- **Client Runtime**: See [contracts/client-runtime.md](./contracts/client-runtime.md)
- **Architecture Decisions**: See [research.md](./research.md)
- **Data Model**: See [data-model.md](./data-model.md)

---

## Support

For issues or questions:
- Check GitHub issues
- Review documentation in `/specs/001-spec-in-in/`
- Contact development team

---

**Congratulations!** You now have a working HTMLDSL Form System. Start building your forms!
