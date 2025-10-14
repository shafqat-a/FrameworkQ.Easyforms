using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace FrameworkQ.Easyforms.Web.Controllers;

[Route("ui/database")] 
public class DatabaseUiController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DatabaseUiController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("generate")] 
    public async Task<IActionResult> Generate(string formId, string provider = "sqlserver", bool dryRun = true)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var payload = JsonSerializer.Serialize(new { FormId = formId, Provider = provider, DryRun = dryRun });
        var resp = await client.PostAsync("v1/database/generate", new StringContent(payload, System.Text.Encoding.UTF8, "application/json"));
        var content = await resp.Content.ReadAsStringAsync();
        ViewData["ResponseJson"] = content;
        ViewData["FormId"] = formId;
        return View("Index");
    }
}

