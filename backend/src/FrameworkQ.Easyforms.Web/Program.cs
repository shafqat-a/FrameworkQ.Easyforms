using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// MVC + Views
builder.Services.AddControllersWithViews();

// Configuration for API base URL
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl") ?? "http://localhost:5000";

// Typed HttpClient to call the backend API
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

// UI routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Same-origin proxy for frontend runtime → backend API
// POST /ui-backend/v1/submissions → {API}/v1/submissions
app.MapPost("/ui-backend/v1/submissions", async (HttpContext ctx, IHttpClientFactory httpFactory) =>
{
    using var reader = new StreamReader(ctx.Request.Body);
    var body = await reader.ReadToEndAsync();

    var client = httpFactory.CreateClient("ApiClient");
    var req = new HttpRequestMessage(HttpMethod.Post, "v1/submissions")
    {
        Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
    };
    var resp = await client.SendAsync(req);
    var content = await resp.Content.ReadAsStringAsync();
    ctx.Response.StatusCode = (int)resp.StatusCode;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsync(content);
});

// GET /ui-backend/v1/proxy/fetch → {API}/v1/proxy/fetch
app.MapGet("/ui-backend/v1/proxy/fetch", async (HttpContext ctx, IHttpClientFactory httpFactory) =>
{
    var qs = ctx.Request.QueryString.HasValue ? ctx.Request.QueryString.Value : string.Empty;
    var client = httpFactory.CreateClient("ApiClient");
    var resp = await client.GetAsync("v1/proxy/fetch" + qs);
    var content = await resp.Content.ReadAsStringAsync();
    ctx.Response.StatusCode = (int)resp.StatusCode;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsync(content);
});

// GET /ui-backend/v1/submissions/{instanceId} → {API}/v1/submissions/{instanceId}
app.MapGet("/ui-backend/v1/submissions/{instanceId}", async (Guid instanceId, IHttpClientFactory httpFactory) =>
{
    var client = httpFactory.CreateClient("ApiClient");
    var resp = await client.GetAsync($"v1/submissions/{instanceId}");
    var content = await resp.Content.ReadAsStringAsync();
    return Results.Content(content, "application/json", System.Text.Encoding.UTF8, (int)resp.StatusCode);
});

// PUT /ui-backend/v1/submissions/{instanceId}/composites → {API}/v1/submissions/{instanceId}/composites
app.MapPut("/ui-backend/v1/submissions/{instanceId}/composites", async (Guid instanceId, HttpContext ctx, IHttpClientFactory httpFactory) =>
{
    using var reader = new StreamReader(ctx.Request.Body);
    var body = await reader.ReadToEndAsync();
    var client = httpFactory.CreateClient("ApiClient");
    var req = new HttpRequestMessage(HttpMethod.Put, $"v1/submissions/{instanceId}/composites")
    {
        Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
    };
    var resp = await client.SendAsync(req);
    var content = await resp.Content.ReadAsStringAsync();
    ctx.Response.StatusCode = (int)resp.StatusCode;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsync(content);
});

app.Run();
