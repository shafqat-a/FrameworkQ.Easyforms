using Microsoft.AspNetCore.Mvc;

namespace FrameworkQ.Easyforms.Web.Controllers;

[Route("ui/designer")] 
public class DesignerUiController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View();
}

