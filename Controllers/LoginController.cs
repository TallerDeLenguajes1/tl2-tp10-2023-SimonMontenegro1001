using Microsoft.AspNetCore.Mvc;
using kanban.Models;
using kanban.Repository;
namespace kanban.Controllers;

public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    private readonly IUsuarioRepository usuarioRepository;

    public LoginController(ILogger<LoginController> logger)
    {
        _logger = logger;
        usuarioRepository = new UsuarioRepository();
    }
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var sesionUsuario = HttpContext.Session.GetString("usuario");
        var sesionId = HttpContext.Session.GetString("id");

        if (!string.IsNullOrEmpty(sesionUsuario) && !string.IsNullOrEmpty(sesionId))
        {
            var sesionUser = usuarioRepository.GetById(int.Parse(sesionId));
            if (sesionUser.NombreDeUsuario == username) return RedirectToAction("Index", "Home");
        }

        var user = usuarioRepository.GetByUsername(username);

        if (user != null && user.Contrasena == password)
        {
            LoguearUsuario(user);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            return View("Index");
        }
    }

    private void LoguearUsuario(Usuario usuario)
    {
        HttpContext.Session.SetString("id", usuario.Id.ToString());
        HttpContext.Session.SetString("usuario", usuario.NombreDeUsuario);
        HttpContext.Session.SetString("rol", usuario.Rol.ToString());
    }
}