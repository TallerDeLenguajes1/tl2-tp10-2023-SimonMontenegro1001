using kanban.Models;
using kanban.ViewModels;
using kanban.Repository;
using kanban.Controllers.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace kanban.Controllers;

[ApiController]
[Route("tareas")]

public class TareaController : Controller
{
    private readonly ITareaRepository _tareaRepository;
    private readonly ITableroRepository _tableroRepository;
    private readonly ILogger<TareaController> _logger;

    public TareaController(ILogger<TareaController> logger, ITableroRepository tableroRepository, ITareaRepository tareaRepository)
    {
        _logger = logger;
        _tableroRepository = tableroRepository;
        _tareaRepository = tareaRepository;
    }


    [HttpPost("crear/{boardId}")]
    public IActionResult Crear(int boardId, [FromForm] CrearTareaViewModel tareaViewModel)
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid) return RedirectToAction("Index", "Home");
        var task = new Tarea
        {
            Id = tareaViewModel.Id,
            IdTablero = tareaViewModel.IdTablero,
            Nombre = tareaViewModel.Nombre,
            Descripcion = tareaViewModel.Descripcion,
            Estado = tareaViewModel.Estado,
            Color = tareaViewModel.Color,
            IdUsuarioAsignado = tareaViewModel.IdUsuarioAsignado
        };
        _tareaRepository.Create(boardId, task);
        return RedirectToAction("ListByBoard", new { id = boardId });
    }

    [HttpGet("crear/{id}")]
    public IActionResult Crear(int tableroId)
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        var tarea = new Tarea
        {
            IdTablero = tableroId
        };

        var crearTareaModel = new CrearTareaViewModel
        {
            Id = tarea.Id,
            IdTablero = tarea.IdTablero,
            Nombre = tarea.Nombre,
            Descripcion = tarea.Descripcion,
            Estado = tarea.Estado,
            Color = tarea.Color,
            IdUsuarioAsignado = tarea.IdUsuarioAsignado
        };
        return View(crearTareaModel);

    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
        var tarea = _tareaRepository.GetById(id);

        if (tarea.Id == 0)
        {
            return NotFound($"No se encontró la tarea con ID {id}");
        }
        var editarTareaModel = new ModificarTareaViewModel
        {
            Id = tarea.Id,
            IdTablero = tarea.IdTablero,
            Estado = tarea.Estado,
            Nombre = tarea.Nombre,
            Descripcion = tarea.Descripcion,
            Color = tarea.Color,
            IdUsuarioAsignado = tarea.IdUsuarioAsignado,
        };
        return View(editarTareaModel);

    }

    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, [FromForm] ModificarTareaViewModel tareaViewModel)
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Home");
        if (!ModelState.IsValid) return RedirectToAction("Index", "Home");

        var task = _tareaRepository.GetById(id);

        if (task.Id == 0)
        {
            return NotFound($"No se encontró la tarea con ID {id}");
        }
        var tarea = new Tarea
        {
            Id = tareaViewModel.Id,
            IdTablero = tareaViewModel.IdTablero,
            Estado = tareaViewModel.Estado,
            Nombre = tareaViewModel.Nombre,
            Descripcion = tareaViewModel.Descripcion,
            Color = tareaViewModel.Color,
            IdUsuarioAsignado = tareaViewModel.IdUsuarioAsignado,
        };

        _tareaRepository.Update(id, tarea);
        return RedirectToAction("ListByBoard", new { id = task.IdTablero });
    }

    [HttpPost("eliminar/{id}")]
    public IActionResult Eliminar(int id)
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
        var tarea = _tareaRepository.GetById(id);

        if (tarea.Id == 0)
        {
            return NotFound();
        }

        _tareaRepository.Delete(id);
        return RedirectToAction("ListByBoard", new { id = tarea.IdTablero });
    }

    [HttpGet("usuario/{id}")]
    public IActionResult Index(int id)
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
        var tareas = _tareaRepository.ListByUser(id);
        var listaTareasModel = new List<ListarTareasViewModel>();
        foreach (var tarea in tareas)
        {
            var listarTareasModel = new ListarTareasViewModel
            {
                Id = tarea.Id,
                IdTablero = tarea.IdTablero,
                Nombre = tarea.Nombre,
                Descripcion = tarea.Descripcion,
                Estado = tarea.Estado,
            };
            listaTareasModel.Add(listarTareasModel);
        }
        return View(listaTareasModel);
    }

    [HttpGet("tablero/{id}")]
    public IActionResult ListByBoard(int id)
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        ViewBag.EsAdmin = LoginHelper.IsAdmin(HttpContext);

        if (!LoginHelper.IsAdmin(HttpContext))
        {
            var UserBoards = _tableroRepository.ListUserBoards(int.Parse(LoginHelper.GetUserId(HttpContext)));
            var FoundBoard = UserBoards.Find(board => board.Id == id);
            if (FoundBoard == null) return NotFound($"No existe el tablero de Id {id}");
        }

        var tareas = _tareaRepository.ListByBoard(id);
        var listaTareasModel = new List<ListarTareasViewModel>();
        foreach (var tarea in tareas)
        {
            var listarTareasModel = new ListarTareasViewModel
            {
                Id = tarea.Id,
                IdTablero = tarea.IdTablero,
                Nombre = tarea.Nombre,
                Descripcion = tarea.Descripcion,
                Estado = tarea.Estado,
            };
            listaTareasModel.Add(listarTareasModel);
        }

        return View(listaTareasModel);
    }
}
