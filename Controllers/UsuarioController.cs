using Microsoft.AspNetCore.Mvc;
using kanban.Repository;
using kanban.Controllers.Helpers;
using kanban.Models;
using kanban.ViewModels;

namespace kanban.Controllers;

[ApiController]
[Route("usuarios")]
public class UsuarioController : Controller
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(ILogger<UsuarioController> logger, IUsuarioRepository usuarioRepository)
    {
        _logger = logger;
        _usuarioRepository = usuarioRepository;
    }

    [HttpPost("crear")]
    public IActionResult Crear([FromForm] CrearUsuarioViewModel crearUsuarioModel)
    {
        if (!ModelState.IsValid) return RedirectToAction("Index", "Home");
        var user = new Usuario
        {
            NombreDeUsuario = crearUsuarioModel.NombreDeUsuario,
            Contrasena = crearUsuarioModel.Contrasena,
            Rol = crearUsuarioModel.Rol
        };
        _usuarioRepository.Create(user);
        return RedirectToAction("Index");
    }

    [HttpGet("crear")]
    public IActionResult Crear()
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            return View(new CrearUsuarioViewModel());
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            List<Usuario> users = _usuarioRepository.List();

            List<ListarUsuariosViewModel> ListaUsuariosModel = new();

            foreach (var user in users)
            {
                var newUser = new ListarUsuariosViewModel
                {
                    NombreDeUsuario = user.NombreDeUsuario,
                    Id = user.Id
                };
                ListaUsuariosModel.Add(newUser);
            }

            return View(ListaUsuariosModel);
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpPost("eliminar/{id}")]
    public IActionResult Eliminar(int id)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {

            var board = _usuarioRepository.GetById(id);

            if (board.Id == 0) return NotFound($"No existe el usuario con ID {id}");

            _usuarioRepository.Delete(id);

            return RedirectToAction("Index");
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {

            var board = _usuarioRepository.GetById(id);

            if (board.Id == 0)
            {
                return NotFound($"No se encontró el usuario con ID {id}");
            }
            var modificarUsuarioModel = new ModificarUsuarioViewModel
            {
                Id = board.Id,
                NombreDeUsuario = board.NombreDeUsuario,
                Contrasena = board.Contrasena,
                Rol = board.Rol,
            };
            return View(modificarUsuarioModel);
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, [FromForm] ModificarUsuarioViewModel modificarUsuarioModel)
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Home");
        if (!ModelState.IsValid) return RedirectToAction("Index", "Home");
        var existingBoard = _usuarioRepository.GetById(id);

        if (existingBoard.Id == 0)
        {
            return NotFound($"No se encontró el tablero con ID {id}");
        }

        var newUser = new Usuario
        {
            Id = modificarUsuarioModel.Id,
            NombreDeUsuario = modificarUsuarioModel.NombreDeUsuario,
            Contrasena = modificarUsuarioModel.Contrasena,
            Rol = modificarUsuarioModel.Rol,
        };

        _usuarioRepository.Update(id, newUser);

        return RedirectToAction("Index");
    }

}