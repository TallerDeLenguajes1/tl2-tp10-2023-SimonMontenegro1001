using Microsoft.AspNetCore.Mvc;
using kanban.Repository;
using kanban.Controllers.Helpers;
using kanban.Models;

namespace kanban.Controllers;

[ApiController]
[Route("usuarios")]
public class UsuarioController : Controller
{
    private readonly IUsuarioRepository usuarioRepository;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(ILogger<UsuarioController> logger)
    {
        _logger = logger;
        usuarioRepository = new UsuarioRepository();
    }

    [HttpPost("crear")]
    public IActionResult Crear([FromForm] Usuario user)
    {
        usuarioRepository.Create(user);
        return RedirectToAction("Index");
    }

    [HttpGet("crear")]
    public IActionResult Crear()
    {
        if (LoginHelper.IsLogged(HttpContext))
        {

            return View(new Usuario());
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (LoginHelper.IsLogged(HttpContext))
        {

            List<Usuario> users = usuarioRepository.List();

            return View(users);
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpPost("eliminar/{id}")]
    public IActionResult Eliminar(int id)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {

            var board = usuarioRepository.GetById(id);

            if (board.Id == 0) return NotFound($"No existe el usuario con ID {id}");

            usuarioRepository.Delete(id);

            return RedirectToAction("Index");
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {

            var board = usuarioRepository.GetById(id);

            if (board.Id == 0)
            {
                return NotFound($"No se encontró el usuario con ID {id}");
            }
            return View(board);
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, [FromForm] Usuario newUser)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {

            var existingBoard = usuarioRepository.GetById(id);

            if (existingBoard.Id == 0)
            {
                return NotFound($"No se encontró el tablero con ID {id}");
            }

            usuarioRepository.Update(id, newUser);

            return RedirectToAction("Index");
        }
        return RedirectToAction("Index", "Login");
    }

}