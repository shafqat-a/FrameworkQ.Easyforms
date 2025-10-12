# FrameworkQ.Easyforms Development Guidelines

**Auto-generated from feature plans**
**Last Updated**: 2025-10-12
**Session**: Complete implementation + benchmark validation

---

## 🧠 Context Loading

**IMPORTANT**: Always read project memory first:

```bash
# 1. Read project understanding
cat .memory/brain.md

# 2. Read current state (what's done, what's next)
cat .memory/state.md

# 3. Read conversation history (decisions, patterns)
cat .memory/talk.md
```

**Memory Location**: `.memory/` directory contains:
- `brain.md` - Complete project understanding, architecture, capabilities
- `state.md` - What's been done, what needs to be done, current status
- `talk.md` - Conversation history, decisions, patterns, user preferences

---

## Active Technologies

### Feature 001: HTMLDSL Form System
- **Backend**: C# / .NET 9.0
  - ASP.NET Core Web API
  - AngleSharp (HTML parsing)
  - Serilog (structured logging)
  - Microsoft.Data.SqlClient (SQL Server)
  - Npgsql (PostgreSQL)

- **Frontend**: jQuery 3.6+ (pure, no build)
  - Expression evaluator
  - Validation module
  - Table manager
  - Data fetch module

- **Database**: SQL Server (primary), PostgreSQL (supported)

### Feature 002: Benchmark Forms
- HTML5, CSS3
- Bengali fonts (Google Fonts: Noto Sans Bengali)
- Uses existing HTMLDSL system (no code changes)

---

## Project Structure

```
FrameworkQ.Easyforms/
├── .memory/                    # Session context (READ FIRST!)
│   ├── brain.md               # Project understanding
│   ├── state.md               # Current status
│   └── talk.md                # Conversation history
├── backend/
│   ├── FrameworkQ.Easyforms.sln
│   └── src/
│       ├── FrameworkQ.Easyforms.Core/
│       ├── FrameworkQ.Easyforms.Parser/
│       ├── FrameworkQ.Easyforms.Database/
│       ├── FrameworkQ.Easyforms.Runtime/
│       └── FrameworkQ.Easyforms.Api/
├── frontend/src/
│   ├── js/                     # jQuery modules
│   └── css/                    # Stylesheets
├── templates/
│   ├── examples/               # 5 demo forms
│   └── benchmark/              # 6 real-world forms
├── specs/                      # Feature specifications
├── .specify/                   # SpecKit templates
└── docs/                       # Original docs
```

---

## Constitution (v1.1.0) - MANDATORY

**BEFORE starting ANY work**:
```bash
# 1. Create GitHub Issue
gh issue create --title "Task" --body "Description"

# 2. Create branch with issue reference
git checkout -b feature/42-task-name

# 3. Commit with issue reference
git commit -m "feat: task (#42)"

# 4. PR with closure
gh pr create --title "Title" --body "Closes #42"
```

See: `.specify/memory/constitution.md`

---

## Quick Commands

### Build & Run
```bash
cd backend && dotnet build FrameworkQ.Easyforms.sln
cd src/FrameworkQ.Easyforms.Api && dotnet run
# API: http://localhost:5000
```

### SpecKit Workflow
```bash
/speckit.specify "feature"    # Create spec
/speckit.plan                 # Plan implementation
/speckit.tasks                # Generate tasks
/speckit.implement            # Execute
```

---

## Important Patterns

**Office/Substation Selector**: See `templates/benchmark/PATTERN-office-substation-selector.md`
**Signature Textboxes**: Use Field widgets, no custom widget
**Calculated Columns**: `data-compute="formula"`
**Aggregates**: `data-agg="sum(col)"`

---

## Status

**Build**: ✅ PASSING
**API**: ✅ RUNNING
**PRs**: 2 merged (#1, #14)
**Production**: ✅ VALIDATED

---

## Before Each Session

1. Read `.memory/brain.md` - Understand project
2. Read `.memory/state.md` - Know status
3. Read `.memory/talk.md` - Review decisions
4. Check `git status` and `gh pr list`

## Before Quitting

1. Update `.memory/state.md` with latest
2. Update `.memory/talk.md` with decisions
3. Commit changes
4. Stop API server if running

---

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->
