using kanban.Models;
using kanban.Repository;
using Microsoft.AspNetCore.Mvc;


namespace kanban.Controllers;

[ApiController]
[Route("tareas")]

public class TareaController : Controller
{
    private readonly ITareaRepository tareaRepository;
    private readonly ILogger<TareaController> _logger;

    public TareaController(ILogger<TareaController> logger)
    {
        _logger = logger;
        tareaRepository = new TareaRepository();
    }

    [HttpPost("crear")]
    public IActionResult Crear(int userId, [FromForm] Tarea task)
    {
        tareaRepository.Create(userId, task);
        return RedirectToAction("Index", new { id = task.IdUsuarioAsignado });
    }

    [HttpGet("crear")]
    public IActionResult Crear()
    {
        return View(new Tarea());
    }

    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, [FromForm] Tarea tarea)
    {
        var task = tareaRepository.GetById(id);

        if (task.Id == 0) return NotFound($"No se encontró la tarea con ID {id}");

        tareaRepository.Update(id, tarea);
        return RedirectToAction("Index", new { id = task.IdUsuarioAsignado });
    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
        var tarea = tareaRepository.GetById(id);

        if (tarea.Id == 0)
        {
            return NotFound($"No se encontró la tarea con ID {id}");
        }

        return View(tarea);
    }

    [HttpPost("eliminar/{id}")]
    public IActionResult Eliminar(int id)
    {
        var tarea = tareaRepository.GetById(id);

        if (tarea.Id == 0) return NotFound();

        tareaRepository.Delete(id);

        return RedirectToAction("Index", new { id = tarea.IdUsuarioAsignado });
    }

    [HttpGet("{id}")]
    public IActionResult Index(int id)
    {
        var tareas = tareaRepository.ListByUser(id);

        return View(tareas);
    }
}
