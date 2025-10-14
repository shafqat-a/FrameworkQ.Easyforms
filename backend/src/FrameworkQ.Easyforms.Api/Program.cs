using FrameworkQ.Easyforms.Api.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "FrameworkQ.Easyforms.Api")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "FrameworkQ.Easyforms API",
        Version = "v1",
        Description = "HTMLDSL Form System REST API"
    });
});

// Configure CORS
var corsOrigins = builder.Configuration.GetSection("CorsPolicy:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Storage root
var dataRoot = Path.Combine(AppContext.BaseDirectory, "App_Data");
Directory.CreateDirectory(dataRoot);

// Register services (parser, schema, stores)
builder.Services.AddSingleton<FrameworkQ.Easyforms.Core.Interfaces.IFormParser, FrameworkQ.Easyforms.Parser.HtmlParser>();
builder.Services.AddSingleton<FrameworkQ.Easyforms.Core.Interfaces.ISchemaExtractor, FrameworkQ.Easyforms.Parser.SchemaBuilder>();
builder.Services.AddSingleton<FrameworkQ.Easyforms.Api.Storage.IFormStore>(sp => new FrameworkQ.Easyforms.Api.Storage.FileFormStore(dataRoot));
builder.Services.AddSingleton<FrameworkQ.Easyforms.Api.Storage.ISubmissionStore>(sp => new FrameworkQ.Easyforms.Api.Storage.FileSubmissionStore(dataRoot));

var app = builder.Build();

// Configure middleware pipeline
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FrameworkQ.Easyforms API v1");
    });
}

app.UseCors("DefaultCorsPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}));

try
{
    Log.Information("Starting FrameworkQ.Easyforms API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
