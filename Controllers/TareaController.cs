using kanban.Models;
using kanban.ViewModels;
using kanban.Repository;
using kanban.Controllers.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.CodeDom;



namespace kanban.Controllers;

[ApiController]
[Route("tareas")]

public class TareaController : Controller
{
    private readonly ITareaRepository tareaRepository;
    private readonly ITableroRepository tableroRepository;
    private readonly ILogger<TareaController> _logger;

    public TareaController(ILogger<TareaController> logger)
    {
        _logger = logger;
        tareaRepository = new TareaRepository();
        tableroRepository = new TableroRepository();
    }

    [HttpPost("crear/{boardId}")]
    public IActionResult Crear(int boardId, [FromForm] CrearTareaViewModel tareaViewModel)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
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
            tareaRepository.Create(boardId, task);
            return RedirectToAction("ListByBoard", new { id = boardId });

        }
        return RedirectToAction("Index", "Login");
    }

    [HttpGet("crear/{id}")]
    public IActionResult Crear(int tableroId)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
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
        return RedirectToAction("Index", "Login");
    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {

            var tarea = tareaRepository.GetById(id);

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
        return RedirectToAction("Index", "Login");
    }

    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, [FromForm] ModificarTareaViewModel tareaViewModel)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {

            var task = tareaRepository.GetById(id);

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

            tareaRepository.Update(id, tarea);
            return RedirectToAction("ListByBoard", new { id = task.IdTablero });
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpPost("eliminar/{id}")]
    public IActionResult Eliminar(int id)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {

            var tarea = tareaRepository.GetById(id);

            if (tarea.Id == 0)
            {
                return NotFound();
            }

            tareaRepository.Delete(id);
            return RedirectToAction("ListByBoard", new { id = tarea.IdTablero });
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpGet("usuario/{id}")]
    public IActionResult Index(int id)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            var tareas = tareaRepository.ListByUser(id);
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
        return RedirectToAction("Index", "Login");
    }

    [HttpGet("tablero/{id}")]
    public IActionResult ListByBoard(int id)
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");


        ViewBag.EsAdmin = LoginHelper.IsAdmin(HttpContext);

        if (!LoginHelper.IsAdmin(HttpContext))
        {
            var UserBoards = tableroRepository.ListUserBoards(int.Parse(LoginHelper.GetUserId(HttpContext)));
            var FoundBoard = UserBoards.Find(board => board.Id == id);
            if (FoundBoard == null) return NotFound($"No existe el tablero de Id {id}");
        }

        var tareas = tareaRepository.ListByBoard(id);
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
