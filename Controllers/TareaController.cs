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

    [HttpPost("crear/{boardId}")]
    public IActionResult Crear(int boardId, [FromForm] Tarea task)
    {
        tareaRepository.Create(boardId, task);
        return RedirectToAction("ListByBoard", new { id = boardId });
    }

    [HttpGet("crear/{id}")]
    public IActionResult Crear(int tableroId)
    {
        var tarea = new Tarea
        {
            IdTablero = tableroId
        };
        return View(tarea);
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

    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, [FromForm] Tarea tarea)
    {
        var task = tareaRepository.GetById(id);

        if (task.Id == 0)
        {
            return NotFound($"No se encontró la tarea con ID {id}");
        }

        tareaRepository.Update(id, tarea);
        return RedirectToAction("ListByBoard", new { id = task.IdTablero });
    }

    [HttpPost("eliminar/{id}")]
    public IActionResult Eliminar(int id)
    {
        var tarea = tareaRepository.GetById(id);

        if (tarea.Id == 0)
        {
            return NotFound();
        }

        tareaRepository.Delete(id);
        return RedirectToAction("ListByBoard", new { id = tarea.IdTablero });
    }

    [HttpGet("usuario/{id}")]
    public IActionResult Index(int id)
    {
        var tareas = tareaRepository.ListByUser(id);

        return View(tareas);
    }

    [HttpGet("tablero/{id}")]
    public IActionResult ListByBoard(int id)
    {
        var tareas = tareaRepository.ListByBoard(id);

        return View(tareas);
    }
}
