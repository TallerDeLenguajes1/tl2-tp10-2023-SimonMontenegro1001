using kanban.Models;
using kanban.Repository;
using kanban.Controllers.helpers;
using Microsoft.AspNetCore.Mvc;

namespace kanban.Controllers;

[ApiController]
[Route("tableros")]
public class TableroController : Controller
{
    private readonly ITableroRepository tableroRepository;
    private readonly ITareaRepository tareaRepository;
    private readonly ILogger<TableroController> _logger;

    public TableroController(ILogger<TableroController> logger)
    {
        _logger = logger;
        tableroRepository = new TableroRepository();
        tareaRepository = new TareaRepository();
    }

    [HttpGet("crear")]
    public IActionResult Crear()
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        ViewBag.EsAdmin = LoginHelper.IsAdmin(HttpContext);
        return View(new Tablero());
    }

    [HttpPost("crear")]
    public IActionResult Crear([FromForm] Tablero board)
    {

        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        if (!LoginHelper.IsAdmin(HttpContext)) board.IdUsuarioPropietario = LoginHelper.GetUserId(HttpContext);

        tableroRepository.Create(board);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        List<Tablero> boards = new();

        ViewBag.EsAdmin = LoginHelper.IsAdmin(HttpContext);

        if (LoginHelper.IsAdmin(HttpContext)) boards = tableroRepository.List();
        else boards = tableroRepository.ListUserBoards(LoginHelper.GetUserId(HttpContext));

        return View(boards);
    }

    [HttpPost("eliminar/{id}")]
    public IActionResult Eliminar(int id)
    {

        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        var board = tableroRepository.GetById(id);

        if (board.Id == 0) return NotFound($"No existe el tablero con ID {id}");

        if (!LoginHelper.IsAdmin(HttpContext))
        {
            var userBoards = tableroRepository.ListUserBoards(LoginHelper.GetUserId(HttpContext));
            var foundBoard = userBoards.Find(board => board.Id == id);
            if (foundBoard != null)
            {
                var tasks = tareaRepository.ListByBoard(id);

                foreach (var task in tasks)
                {
                    tareaRepository.Delete(task.Id);
                }

                tableroRepository.Delete(id);
            }
            else
            {
                return NotFound($"No existe el tablero con ID {id}");
            }
        }

        var tasksToDelete = tareaRepository.ListByBoard(id);

        foreach (var task in tasksToDelete)
        {
            tareaRepository.Delete(task.Id);
        }

        tableroRepository.Delete(id);

        return RedirectToAction("Index");
    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {

        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        var board = tableroRepository.GetById(id);

        if (board.Id == 0) return NotFound($"No existe el tablero con ID {id}");

        ViewBag.EsAdmin = LoginHelper.IsAdmin(HttpContext);

        if (!LoginHelper.IsAdmin(HttpContext))
        {
            var userBoards = tableroRepository.ListUserBoards(LoginHelper.GetUserId(HttpContext));
            var foundBoard = userBoards.Find(board => board.Id == id);
            if (foundBoard != null) return View(board);
            else return NotFound($"No existe el tablero con ID {id}");
        }

        return View(board);

    }

    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, [FromForm] Tablero newBoard)
    {

        if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

        var board = tableroRepository.GetById(id);

        if (board.Id == 0) return NotFound($"No existe el tablero con ID {id}");

        if (!LoginHelper.IsAdmin(HttpContext))
        {
            var userBoards = tableroRepository.ListUserBoards(LoginHelper.GetUserId(HttpContext));
            var foundBoard = userBoards.Find(board => board.Id == id);
            if (foundBoard != null)
            {
                newBoard.IdUsuarioPropietario = LoginHelper.GetUserId(HttpContext);
                tableroRepository.Update(id, newBoard);

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound($"No existe el tablero con ID {id}");
            }
        }
        tableroRepository.Update(id, newBoard);

        return RedirectToAction("Index");
    }
}

