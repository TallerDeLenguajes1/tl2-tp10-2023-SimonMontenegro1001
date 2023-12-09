using kanban.Models;
using kanban.Repository;
using kanban.Controllers.Helpers;
using kanban.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace kanban.Controllers;

[ApiController]
[Route("tableros")]
public class TableroController : Controller
{
    private readonly ITableroRepository _tableroRepository;
    private readonly ITareaRepository _tareaRepository;
    private readonly ILogger<TableroController> _logger;

    public TableroController(ILogger<TableroController> logger, ITableroRepository tableroRepository, ITareaRepository tareaRepository)
    {
        _logger = logger;
        _tableroRepository = tableroRepository;
        _tareaRepository = tareaRepository;
    }

    [HttpGet("crear")]
    public IActionResult Crear()
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
        CrearTableroViewModel crearTableroModel = new()
        {
            IsAdmin = LoginHelper.IsAdmin(HttpContext)
        };
        return View(crearTableroModel);
    }

    [HttpPost("crear")]
    public IActionResult Crear([FromForm] CrearTableroViewModel crearTableroModel)
    {

        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        if (!ModelState.IsValid) return RedirectToAction("Index", "Home");
        var tablero = new Tablero()
        {
            Nombre = crearTableroModel.Nombre,
            Descripcion = crearTableroModel.Descripcion,
        };

        if (!LoginHelper.IsAdmin(HttpContext))
        {
            tablero.IdUsuarioPropietario = int.Parse(LoginHelper.GetUserId(HttpContext));
        }

        _tableroRepository.Create(tablero);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        List<Tablero> boards = new();

        ViewBag.EsAdmin = LoginHelper.IsAdmin(HttpContext);

        if (ViewBag.EsAdmin) boards = _tableroRepository.List();

        else boards = _tableroRepository.ListUserBoards(int.Parse(LoginHelper.GetUserId(HttpContext)));

        List<ListarTablerosViewModel> ListarTablerosModel = new();

        foreach (var board in boards)
        {
            ListarTablerosViewModel modelo = new(board.Nombre, board.Descripcion, board.Id, board.IdUsuarioPropietario);
            ListarTablerosModel.Add(modelo);
        }

        return View(ListarTablerosModel);
    }

    [HttpPost("eliminar/{id}")]
    public IActionResult Eliminar(int id)
    {

        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        var board = _tableroRepository.GetById(id);

        if (board.Id == 0) return NotFound($"No existe el tablero con ID {id}");

        if (!LoginHelper.IsAdmin(HttpContext))
        {
            var userBoards = _tableroRepository.ListUserBoards(int.Parse(LoginHelper.GetUserId(HttpContext)));
            var foundBoard = userBoards.Find(board => board.Id == id);
            if (foundBoard != null)
            {
                var tasks = _tareaRepository.ListByBoard(id);

                foreach (var task in tasks)
                {
                    _tareaRepository.Delete(task.Id);
                }

                _tableroRepository.Delete(id);
            }
            else
            {
                return NotFound($"No existe el tablero con ID {id}");
            }
        }

        var tasksToDelete = _tareaRepository.ListByBoard(id);

        foreach (var task in tasksToDelete)
        {
            _tareaRepository.Delete(task.Id);
        }

        _tableroRepository.Delete(id);

        return RedirectToAction("Index");
    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {

        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        var board = _tableroRepository.GetById(id);

        var modificarTableroModel = new ModificarTableroViewModel
        {
            Nombre = board.Nombre,
            Descripcion = board.Descripcion,
            Id = board.Id
        };

        if (board.Id == 0) return NotFound($"No existe el tablero con ID {id}");

        ViewBag.EsAdmin = LoginHelper.IsAdmin(HttpContext);

        if (!LoginHelper.IsAdmin(HttpContext))
        {
            var userBoards = _tableroRepository.ListUserBoards(int.Parse(LoginHelper.GetUserId(HttpContext)));
            var foundBoard = userBoards.Find(board => board.Id == id);
            if (foundBoard != null) return View(modificarTableroModel);
            else return NotFound($"No existe el tablero con ID {id}");
        }
        else
        {
            modificarTableroModel.IdUsuarioPropietario = board.IdUsuarioPropietario;
        }

        return View(modificarTableroModel);

    }

    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, [FromForm] ModificarTableroViewModel newBoardViewModel)
    {

        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        if (!ModelState.IsValid) return RedirectToAction("Index", "Home");
        var board = _tableroRepository.GetById(id);

        if (board.Id == 0) return NotFound($"No existe el tablero con ID {id}");

        var newBoard = new Tablero
        {
            Nombre = newBoardViewModel.Nombre,
            Descripcion = newBoardViewModel.Descripcion,
            Id = newBoardViewModel.Id,
            IdUsuarioPropietario = newBoardViewModel.IdUsuarioPropietario == 0
            ? board.IdUsuarioPropietario
            : newBoardViewModel.IdUsuarioPropietario
        };

        if (!LoginHelper.IsAdmin(HttpContext))
        {
            var userBoards = _tableroRepository.ListUserBoards(int.Parse(LoginHelper.GetUserId(HttpContext)));
            var foundBoard = userBoards.Find(board => board.Id == id);
            if (foundBoard != null)
            {
                newBoard.IdUsuarioPropietario = int.Parse(LoginHelper.GetUserId(HttpContext));
                _tableroRepository.Update(id, newBoard);

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound($"No existe el tablero con ID {id}");
            }
        }
        _tableroRepository.Update(id, newBoard);

        return RedirectToAction("Index");
    }
}

