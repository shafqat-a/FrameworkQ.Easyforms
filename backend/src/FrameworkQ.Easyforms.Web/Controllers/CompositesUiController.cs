using Microsoft.AspNetCore.Mvc;

namespace FrameworkQ.Easyforms.Web.Controllers;

[Route("ui/composites")]
public class CompositesUiController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View();

    [HttpGet("basic")]
    public IActionResult Basic() => View();

    [HttpGet("events")]
    public IActionResult Events() => View();

    [HttpGet("designer-runtime")]
    public IActionResult DesignerRuntime() => View();

    [HttpGet("state")]
    public IActionResult State() => View();

    [HttpGet("smart-table")]
    public IActionResult SmartTable() => View();

    [HttpGet("beginner")]
    public IActionResult Beginner() => View();
}
