using Microsoft.AspNetCore.Mvc;

namespace OlimpicGamesProject.Controllers;

public class OlympicGamesController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}