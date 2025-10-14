using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace FrameworkQ.Easyforms.Web.Controllers;

[Route("ui/submissions")] 
public class SubmissionsUiController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    public SubmissionsUiController(IHttpClientFactory httpClientFactory) { _httpClientFactory = httpClientFactory; }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? formId = null, string? status = null)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var url = "v1/query/submissions";
        var qs = new List<string>();
        if (!string.IsNullOrEmpty(formId)) qs.Add($"formId={Uri.EscapeDataString(formId)}");
        if (!string.IsNullOrEmpty(status)) qs.Add($"status={Uri.EscapeDataString(status)}");
        if (qs.Count > 0) url += "?" + string.Join("&", qs);
        var resp = await client.GetAsync(url);
        var json = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var data = doc.RootElement.GetProperty("data");
        ViewData["SubmissionsJson"] = data.GetRawText();
        return View();
    }

    [HttpGet("{instanceId}")]
    public async Task<IActionResult> Details(Guid instanceId)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var resp = await client.GetAsync($"v1/submissions/{instanceId}");
        if (!resp.IsSuccessStatusCode) return NotFound();
        var json = await resp.Content.ReadAsStringAsync();
        ViewData["SubmissionJson"] = json;
        return View();
    }

    [HttpGet("create")] 
    public async Task<IActionResult> Create()
    {
        // Load forms for selection
        var client = _httpClientFactory.CreateClient("ApiClient");
        var resp = await client.GetAsync("v1/forms");
        var json = await resp.Content.ReadAsStringAsync();
        ViewData["FormsListJson"] = json;
        return View();
    }
}
