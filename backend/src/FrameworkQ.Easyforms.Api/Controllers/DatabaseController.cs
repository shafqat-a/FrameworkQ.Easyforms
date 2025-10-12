namespace FrameworkQ.Easyforms.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using FrameworkQ.Easyforms.Database;
using FrameworkQ.Easyforms.Core.Interfaces;
using FrameworkQ.Easyforms.Core.Models;
using System.Text.Json;

/// <summary>
/// API controller for database schema generation and migration
/// </summary>
[ApiController]
[Route("v1/database")]
public class DatabaseController : ControllerBase
{
    private readonly ILogger<DatabaseController> _logger;
    private readonly IConfiguration _configuration;

    // TODO: Inject via DI once services registered
    public DatabaseController(ILogger<DatabaseController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Generate database schema for a form
    /// POST /v1/database/generate
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateSchema([FromBody] GenerateSchemaRequest request)
    {
        _logger.LogInformation("Generating database schema for form: {FormId}", request.FormId);

        try
        {
            // Get form definition (from in-memory storage for now)
            // TODO: Get from database once persistence implemented
            var formDef = GetFormDefinition(request.FormId);
            if (formDef == null)
            {
                return NotFound(new { error = new { code = "NOT_FOUND", message = $"Form '{request.FormId}' not found" } });
            }

            // Create provider
            var provider = DatabaseProviderFactory.Create(request.Provider);

            // Generate DDL
            var generator = new DdlGenerator(provider);
            var ddlStatements = generator.GenerateFormDdl(formDef);

            if (request.DryRun)
            {
                return Ok(new
                {
                    success = true,
                    ddl = ddlStatements,
                    tablesCreated = new string[] { }
                });
            }

            // Execute DDL
            var connectionString = GetConnectionString(request.Provider);
            var tablesCreated = new List<string>();

            foreach (var ddl in ddlStatements)
            {
                await provider.ExecuteDdlAsync(ddl, connectionString);
                // Extract table name from DDL (simplified)
                var tableName = ExtractTableName(ddl);
                if (!string.IsNullOrEmpty(tableName))
                {
                    tablesCreated.Add(tableName);
                }
            }

            _logger.LogInformation("Database schema generated successfully for {FormId}. Tables: {Tables}",
                request.FormId, string.Join(", ", tablesCreated));

            return Ok(new
            {
                success = true,
                ddl = ddlStatements,
                tablesCreated = tablesCreated
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate database schema");
            return BadRequest(new
            {
                error = new
                {
                    code = "GENERATION_FAILED",
                    message = ex.Message,
                    correlationId = HttpContext.TraceIdentifier
                }
            });
        }
    }

    /// <summary>
    /// Migrate database schema
    /// POST /v1/database/migrate
    /// </summary>
    [HttpPost("migrate")]
    public async Task<IActionResult> MigrateSchema([FromBody] MigrateSchemaRequest request)
    {
        _logger.LogInformation("Migrating schema for form: {FormId} from {OldVersion} to {NewVersion}",
            request.FormId, request.FromVersion, request.ToVersion);

        try
        {
            // TODO: Get old and new schemas from database
            var provider = DatabaseProviderFactory.Create(request.Provider);
            var engine = new MigrationEngine(provider);

            // For now, return placeholder
            return Ok(new
            {
                success = true,
                transformations = new object[] { },
                migrationSql = new string[] { "-- Migration SQL would go here" }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to migrate schema");
            return BadRequest(new
            {
                error = new
                {
                    code = "MIGRATION_FAILED",
                    message = ex.Message,
                    correlationId = HttpContext.TraceIdentifier
                }
            });
        }
    }

    private FormDefinition? GetFormDefinition(string formId)
    {
        // TODO: Get from database
        // For now, get from FormsController static storage via reflection or DI
        return null;
    }

    private string GetConnectionString(string provider)
    {
        return provider.ToLower() switch
        {
            "sqlserver" => _configuration.GetConnectionString("SqlServer") ?? throw new InvalidOperationException("SQL Server connection string not configured"),
            "postgresql" => _configuration.GetConnectionString("PostgreSQL") ?? throw new InvalidOperationException("PostgreSQL connection string not configured"),
            _ => throw new ArgumentException($"Unknown provider: {provider}")
        };
    }

    private string? ExtractTableName(string ddl)
    {
        // Simple extraction: find "CREATE TABLE tablename"
        var match = System.Text.RegularExpressions.Regex.Match(ddl, @"CREATE TABLE\s+[\[\""']?(\w+)[\]\""']?", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : null;
    }
}

public class GenerateSchemaRequest
{
    public string FormId { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string Provider { get; set; } = "sqlserver";
    public bool DryRun { get; set; }
}

public class MigrateSchemaRequest
{
    public string FormId { get; set; } = string.Empty;
    public string FromVersion { get; set; } = string.Empty;
    public string ToVersion { get; set; } = string.Empty;
    public string Provider { get; set; } = "sqlserver";
    public bool DryRun { get; set; }
}
