using System.Diagnostics;
using kanban.Controllers.Helpers;
using Microsoft.AspNetCore.Mvc;
using kanban.Models;

namespace kanban.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (LoginHelper.IsLogged(HttpContext)) return View();
        return RedirectToAction("Index", "Login");
    }

    public IActionResult Privacy()
    {
        if (LoginHelper.IsLogged(HttpContext)) return View();
        return RedirectToAction("Index", "Login");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        try
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en el endpoint Error de HomeController: {ex.Message}");
            return RedirectToAction("Error", "Home");
        }
    }

}
