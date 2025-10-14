using Microsoft.AspNetCore.Mvc;

namespace FrameworkQ.Easyforms.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

