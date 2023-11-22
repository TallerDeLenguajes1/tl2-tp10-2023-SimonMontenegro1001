using Microsoft.AspNetCore.Mvc;
using kanban.Repository;
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
        return View(new Usuario());
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<Usuario> users = usuarioRepository.List();

        return View(users);
    }

    [HttpPost("eliminar/{id}")]
    public IActionResult Eliminar(int id)
    {
        var board = usuarioRepository.GetById(id);

        if (board.Id == 0) return NotFound($"No existe el usuario con ID {id}");

        usuarioRepository.Delete(id);

        return RedirectToAction("Index");
    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
        var board = usuarioRepository.GetById(id);

        if (board.Id == 0)
        {
            return NotFound($"No se encontró el usuario con ID {id}");
        }
        return View(board);
    }

    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, [FromForm] Usuario newUser)
    {
        var existingBoard = usuarioRepository.GetById(id);

        if (existingBoard.Id == 0)
        {
            return NotFound($"No se encontró el tablero con ID {id}");
        }

        usuarioRepository.Update(id, newUser);

        return RedirectToAction("Index");
    }

}