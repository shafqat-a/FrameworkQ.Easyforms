using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace FrameworkQ.Easyforms.Web.Controllers;

[Route("ui/forms")] 
public class FormsUiController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public FormsUiController(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var resp = await client.GetAsync("v1/forms");
        var json = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var forms = doc.RootElement.GetProperty("data");
        ViewData["FormsJson"] = forms.GetRawText();
        return View();
    }

    [HttpGet("upload")]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost("upload")] 
    public async Task<IActionResult> Upload(IFormFile htmlFile)
    {
        if (htmlFile == null || htmlFile.Length == 0)
        {
            ModelState.AddModelError("htmlFile", "Please select an HTML file.");
            return View();
        }

        var client = _httpClientFactory.CreateClient("ApiClient");
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(htmlFile.OpenReadStream()), "htmlFile", htmlFile.FileName);
        var resp = await client.PostAsync("v1/forms", content);
        if (!resp.IsSuccessStatusCode)
        {
            var err = await resp.Content.ReadAsStringAsync();
            ModelState.AddModelError("htmlFile", $"Upload failed: {err}");
            return View();
        }
        var body = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var id = doc.RootElement.GetProperty("id").GetString();
        return RedirectToAction(nameof(Details), new { formId = id });
    }

    [HttpGet("{formId}")]
    public async Task<IActionResult> Details(string formId)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var resp = await client.GetAsync($"v1/forms/{formId}");
        if (!resp.IsSuccessStatusCode)
        {
            return NotFound();
        }
        var json = await resp.Content.ReadAsStringAsync();
        ViewData["FormJson"] = json;
        return View();
    }

    [HttpGet("{formId}/preview")] 
    public async Task<IActionResult> Preview(string formId)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var resp = await client.GetAsync($"v1/forms/{formId}/html");
        if (!resp.IsSuccessStatusCode)
        {
            return NotFound();
        }
        var html = await resp.Content.ReadAsStringAsync();
        ViewData["FormId"] = formId;
        ViewData["ApiBaseForRuntime"] = "/ui-backend"; // use same-origin proxy
        ViewData["FormHtml"] = html; 
        return View();
    }
}

