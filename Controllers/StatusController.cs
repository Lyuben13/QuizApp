using Microsoft.AspNetCore.Mvc;

namespace QuizApp.Controllers;

public class StatusController : Controller
{
    [Route("Status/{code:int}")]
    public IActionResult Index(int code)
    {
        if (code == 404) return View("NotFound");

        Response.StatusCode = code;
        ViewBag.StatusCode = code;
        return View("HttpError");
    }
}
