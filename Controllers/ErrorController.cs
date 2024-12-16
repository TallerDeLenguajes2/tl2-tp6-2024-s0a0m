using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    public IActionResult Error403()
    {
        return View();
    }
}