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

        if (LoginHelper.IsLogged(HttpContext)) return View(new Tablero());
        return RedirectToAction("Index", "Login");
    }

    [HttpPost("crear")]
    public IActionResult Crear([FromForm] Tablero board)
    {

        if (LoginHelper.IsLogged(HttpContext))
        {
            tableroRepository.Create(board);
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index", "Login");

    }

    [HttpGet]
    public IActionResult Index()
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            List<Tablero> boards = new();
            if (LoginHelper.IsAdmin(HttpContext))
            {
                boards = tableroRepository.List();
            }
            else
            {
                boards = tableroRepository.ListUserBoards(LoginHelper.GetUserId(HttpContext));
            }

            if (boards != null) return View(boards);

            return NotFound();
        }
        return RedirectToAction("Index", "Login");
    }

    [HttpPost("eliminar/{id}")]
    public IActionResult Eliminar(int id)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            var board = tableroRepository.GetById(id);

            if (board.Id == 0) return NotFound($"No existe el tablero con ID {id}");

            var tasksToDelete = tareaRepository.ListByBoard(id);
            foreach (var task in tasksToDelete)
            {
                tareaRepository.Delete(task.Id);
            }

            tableroRepository.Delete(id);

            return RedirectToAction("Index");
        }
        return RedirectToAction("Index", "Login");


    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {

        if (LoginHelper.IsLogged(HttpContext))
        {
            var board = tableroRepository.GetById(id);

            if (board.Id == 0)
            {
                return NotFound($"No se encontró el tablero con ID {id}");
            }
            return View(board);
        }
        return RedirectToAction("Index", "Login");

    }

    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, [FromForm] Tablero newBoard)
    {
        if (LoginHelper.IsLogged(HttpContext))
        {
            var existingBoard = tableroRepository.GetById(id);

            if (existingBoard.Id == 0)
            {
                return NotFound($"No se encontró el tablero con ID {id}");
            }

            tableroRepository.Update(id, newBoard);

            return RedirectToAction("Index");

        }
        return RedirectToAction("Index", "Login");
    }
}
