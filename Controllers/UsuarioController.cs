using Microsoft.AspNetCore.Mvc;
using kanban.Repository;
using kanban.Models;

namespace kanban.Controllers;

[ApiController]
[Route("usuarios")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioRepository usuarioRepository;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(ILogger<UsuarioController> logger)
    {
        _logger = logger;
        usuarioRepository = new UsuarioRepository();
    }

    [HttpPost]
    public ActionResult<Usuario> CreateUsuario(Usuario user)
    {
        usuarioRepository.CreateUser(user);
        return Created("ubicacion del recurso", user);
    }

    [HttpGet]
    public ActionResult<List<Usuario>> GetAllUsuarios()
    {
        List<Usuario> users = usuarioRepository.ListUsers();

        return Ok(users);
    }

    [HttpGet("{userId}", Name = "GetUsuarioById")]
    public ActionResult<Usuario> GetUsuarioById(int userId)
    {
        var user = usuarioRepository.GetUserById(userId);

        if (user.Id == 0) return NotFound("No existe el usuario");

        return Ok(user);
    }

    [HttpPut("{userId}/{NewUsername}")]
    public ActionResult<Usuario> UpdateNombreUsuario(int userId, string NewUsername)
    {
        var user = usuarioRepository.GetUserById(userId);

        if (user.Id == 0)
        {
            return NotFound($"No se encontró el usuario con ID {userId}");
        }

        user.NombreDeUsuario = NewUsername;

        usuarioRepository.UpdateUser(userId, user);

        return Ok(user);
    }

}