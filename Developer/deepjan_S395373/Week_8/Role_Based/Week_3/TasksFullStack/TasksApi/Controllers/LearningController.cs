using Microsoft.AspNetCore.Mvc;

namespace TasksApi.Controllers;

public class LearningController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Week 8 MVC Learning";
        ViewData["Architecture"] = "Controller receives the route, prepares data, and selects a Razor View.";
        return View();
    }
}
