<!--
Sync Impact Report - Constitution v1.1.0
===========================================
Version change: 1.0.0 → 1.1.0 (Minor - Material expansion of Principle I)
Updated: 2025-10-12

Modified principles:
  - Principle I (GitHub Workflow): Added mandatory GitHub Issues requirement
    * All work must be tracked in GitHub Issues before starting
    * Tasks from tasks.md must be converted to GitHub Issues
    * Issue-driven development with `gh issue` CLI

Added sections:
  - Issue-Driven Development subsection under Development Workflow
  - Task-to-Issue Conversion Requirements

Removed sections: None

Templates requiring updates:
  ✅ .specify/templates/plan-template.md - No changes needed
  ✅ .specify/templates/spec-template.md - No changes needed
  ⚠️ .specify/templates/tasks-template.md - Should reference issue creation
  ⚠️ README.md - Update to mention issue-driven workflow

Follow-up TODOs:
  - Update tasks template to include issue creation step
  - Add gh CLI examples to quickstart guides

Rationale for v1.1.0:
- Material expansion of GitHub Workflow principle
- Strengthens issue tracking requirements (now mandatory before work starts)
- Adds task-to-issue conversion requirement for systematic tracking
- Backward compatible (doesn't break existing workflows, just enhances them)
-->

# FrameworkQ.Easyforms Constitution

## Core Principles

### I. GitHub Workflow & Issue-Driven Development (NON-NEGOTIABLE)

All development MUST follow GitHub workflow practices with mandatory issue tracking:

- **Issue-first**: All work MUST be tracked in GitHub Issues BEFORE starting implementation
- **Feature branches**: All work occurs on feature branches, never directly on `main`
- **Pull requests**: All changes MUST be submitted via pull request with description and context
- **Code review**: PRs require review before merge (recommend at least 1 approval)
- **Branch protection**: `main` branch protected from direct pushes and force pushes
- **Commit messages**: Descriptive commits following conventional format (e.g., `feat:`, `fix:`, `docs:`, `refactor:`)
- **Issue linking**: All PRs MUST reference related GitHub Issue(s)
- **Task-to-Issue conversion**: Tasks from tasks.md MUST be converted to GitHub Issues for tracking

**Rationale**: GitHub Issues provide centralized tracking, enable project management visibility, facilitate team coordination, create searchable history, and ensure no work starts without documented justification. Issue-driven development prevents ad-hoc changes and maintains accountability.

### II. Modular Architecture

System MUST maintain clear separation of concerns:

- **Backend**: Separate projects for Core (domain models), Parser (HTML→JSON), Database (SQL generation), Api (REST endpoints), Runtime (validation/submissions)
- **Frontend**: Modular jQuery plugins with single responsibility (formruntime, validation, expression-evaluator, table-manager, etc.)
- **Database**: Provider abstraction pattern for SQL Server and PostgreSQL support without tight coupling
- **No monoliths**: Each module independently testable with clear interfaces

**Rationale**: Modularity enables independent development, testing, and deployment of features; reduces merge conflicts; facilitates parallel team work.

### III. User Story-Driven Development

Features MUST be organized and delivered by user story:

- **Independent stories**: Each user story (P1-P6) implementable and testable independently
- **MVP first**: Prioritize P1 (Basic Forms) as minimum viable product before other stories
- **Incremental delivery**: Each story adds value without breaking previous stories
- **Story checkpoints**: Validate each story independently before proceeding to next priority

**Rationale**: Story-based development enables incremental value delivery, reduces risk, allows flexible prioritization, and ensures each feature slice is production-ready.

### IV. Design Before Code

Implementation MUST follow design phase completion:

- **Specification first**: Complete spec.md with user stories before planning
- **Research documented**: Architecture decisions captured in research.md with rationale
- **Contracts defined**: API contracts (OpenAPI) and client contracts documented before implementation
- **Data model designed**: Entity definitions, relationships, and schema documented in data-model.md
- **Plan validated**: Constitution check passes before implementation begins

**Rationale**: Design-first approach prevents rework, ensures alignment with requirements, enables parallel development, and provides clear implementation roadmap.

### V. Security and Quality

Code MUST meet security and quality standards:

- **HTML sanitization**: Strict allowlist for form templates (no inline scripts, no XSS vectors)
- **Input validation**: Dual validation (client and server) for all user inputs
- **SQL injection prevention**: Parameterized queries and validated DDL generation only
- **Authentication**: Server-side auth injection for external APIs (never expose tokens in client)
- **Structured logging**: All API calls and errors logged with correlation IDs for traceability
- **Error handling**: Consistent error response format across all endpoints

**Rationale**: Security cannot be retrofitted; quality standards enforced from start prevent technical debt and production incidents.

## Development Workflow

### GitHub Workflow Requirements

**Branch Naming**:
- Feature branches: `feature/###-description` or `###-feature-name` (e.g., `001-spec-in-in`)
- Bug fixes: `fix/###-description`
- Documentation: `docs/description`
- Hotfixes: `hotfix/description`

**Pull Request Process**:
1. Create feature branch from latest `main`
2. Make changes with clear, atomic commits
3. Push branch and open PR with description:
   - What: Summary of changes
   - Why: Rationale and context
   - Testing: How to verify changes
4. Request review from team member(s)
5. Address review feedback
6. Merge when approved (prefer squash merge for clean history)
7. Delete feature branch after merge

**Commit Message Format**:
```
<type>(<scope>): <subject>

<body>

<footer>
```

Types: `feat`, `fix`, `docs`, `refactor`, `test`, `chore`, `perf`, `ci`

Examples:
- `feat(parser): add support for grid widget parsing`
- `fix(validation): correct regex pattern escaping`
- `docs(constitution): establish GitHub workflow principle`

**PR Review Checklist**:
- [ ] Code follows project structure and conventions
- [ ] Changes align with user story acceptance criteria
- [ ] No security vulnerabilities introduced
- [ ] Constitution principles upheld
- [ ] Breaking changes documented
- [ ] Related issue linked

### Issue-Driven Development

**Before Starting Any Work**:
1. **Create GitHub Issue** describing the work:
   - Clear title summarizing the change
   - Description with context and acceptance criteria
   - Labels (bug, enhancement, documentation, etc.)
   - Milestone (if applicable)
   - Assignee

2. **Convert Tasks to Issues** (for planned features):
   ```bash
   # For each task in tasks.md, create issue
   gh issue create --title "T001: Create backend project structure" \
     --body "Task from specs/###-feature/tasks.md" \
     --label "task" \
     --milestone "Feature ###"
   ```

3. **Link Issue to Branch**:
   ```bash
   # Branch name should reference issue number
   git checkout -b feature/42-add-validation  # Issue #42
   ```

4. **Link PR to Issue**:
   - Use keywords in PR description: "Closes #42" or "Fixes #42"
   - GitHub will auto-close issue when PR merges

**Required gh CLI Commands**:
```bash
# List issues
gh issue list

# Create issue
gh issue create --title "Title" --body "Description"

# View issue
gh issue view 42

# Close issue
gh issue close 42

# Link to PR
gh pr create --title "Title" --body "Closes #42"
```

### Git Commit Hygiene

**DO**:
- Write descriptive commit messages
- Make atomic commits (one logical change per commit)
- Reference issue numbers in commits/PRs (e.g., "feat: add validation (#42)")
- Keep commits focused and reviewable
- Create GitHub Issue before starting work

**DON'T**:
- Commit directly to `main`
- Force push to shared branches
- Include large binary files without LFS
- Commit secrets or credentials
- Use `--no-verify` to skip hooks
- Start work without a GitHub Issue

## Quality Standards

### Code Quality

- **Consistency**: Follow .NET naming conventions (PascalCase for classes/methods, camelCase for locals)
- **Readability**: Clear variable names, comments for complex logic, no magic numbers
- **DRY**: Extract repeated logic into reusable functions
- **SOLID**: Favor composition over inheritance, depend on interfaces not concrete types

### Testing Standards (Optional)

Testing is OPTIONAL but recommended:

- **Unit tests**: For parsers, expression evaluators, validation logic
- **Integration tests**: For database providers, end-to-end workflows
- **Contract tests**: For API endpoint contracts
- **TDD encouraged**: Write failing tests before implementation where beneficial

### Documentation Standards

- **API documentation**: OpenAPI spec maintained for all endpoints
- **Code comments**: Complex algorithms and business logic documented
- **README updates**: Architecture diagrams and quickstart guide current
- **Change logs**: Breaking changes and migrations documented

## Governance

### Constitution Authority

- This constitution supersedes all other development practices and guidelines
- All pull requests MUST demonstrate compliance with constitution principles
- Constitution check in `plan.md` MUST pass before implementation begins
- Violations require explicit justification and approval

### Amendment Process

**Proposing Amendments**:
1. Open GitHub issue with proposed change and rationale
2. Discuss with team (minimum 3 business days for feedback)
3. Create PR with amendment using `/speckit.constitution` command
4. Requires consensus approval (all active contributors)
5. Version bump according to semver rules

**Version Semantics**:
- **MAJOR (X.0.0)**: Backward incompatible principle removal or redefinition
- **MINOR (0.X.0)**: New principle or section added
- **PATCH (0.0.X)**: Clarifications, wording improvements, typo fixes

### Compliance Review

**Regular Reviews**:
- Constitution compliance reviewed in every PR
- Quarterly retrospective on principle effectiveness
- Annual full constitution review

**Enforcement**:
- PR reviewers verify compliance before approval
- Complexity violations require justification in `plan.md` Complexity Tracking section
- Security violations block PR merge until resolved

### Guidance Files

- **Agent context**: `CLAUDE.md` provides runtime guidance for AI assistants
- **Templates**: `.specify/templates/` provide structure for specs, plans, tasks
- **Commands**: `.specify/templates/commands/` define workflow automation

**Version**: 1.1.0 | **Ratified**: 2025-10-11 | **Last Amended**: 2025-10-12
