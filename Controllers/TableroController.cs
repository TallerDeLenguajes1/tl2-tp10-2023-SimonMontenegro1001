using kanban.Models;
using kanban.Repository;
using Microsoft.AspNetCore.Mvc;

namespace kanban.Controllers;

[ApiController]
[Route("tableros")]
public class TableroController : Controller
{
    private readonly ITableroRepository tableroRepository;
    private readonly ILogger<TableroController> _logger;

    public TableroController(ILogger<TableroController> logger)
    {
        _logger = logger;
        tableroRepository = new TableroRepository();
    }

    [HttpGet("crear")]
    public IActionResult Crear()
    {
        return View(new Tablero());
    }

    [HttpPost("crear")]
    public IActionResult Crear([FromForm] Tablero board)
    {
        tableroRepository.Create(board);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<Tablero> boards = tableroRepository.List();

        if (boards != null) return View(boards);

        return NotFound();
    }

    [HttpPost("eliminar/{id}")]
    public IActionResult Eliminar(int id)
    {
        var board = tableroRepository.GetById(id);

        if (board.Id == 0) return NotFound($"No existe el tablero con ID {id}");

        tableroRepository.Delete(id);

        return RedirectToAction("Index");
    }

    [HttpGet("editar/{id}")]
    public IActionResult Editar(int id)
    {
        var board = tableroRepository.GetById(id);

        if (board.Id == 0)
        {
            return NotFound($"No se encontró el tablero con ID {id}");
        }
        return View(board);
    }

    [HttpPost("editar/{id}")]
    public IActionResult Editar(int id, [FromForm] Tablero newBoard)
    {
        var existingBoard = tableroRepository.GetById(id);

        if (existingBoard.Id == 0)
        {
            return NotFound($"No se encontró el tablero con ID {id}");
        }

        tableroRepository.Update(id, newBoard);

        return RedirectToAction("Index");
    }

}
