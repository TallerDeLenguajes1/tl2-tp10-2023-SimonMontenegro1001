using kanban.Models;
using kanban.Repository;
using Microsoft.AspNetCore.Mvc;


namespace kanban.Controllers;

[ApiController]
[Route("api/tarea")]

public class TareaController : ControllerBase
{
    private readonly ITareaRepository tareaRepository;
    private readonly ILogger<TareaController> _logger;

    public TareaController(ILogger<TareaController> logger)
    {
        _logger = logger;
        tareaRepository = new TareaRepository();
    }

    [HttpPost]
    public ActionResult<Tarea> CreateTarea(int boardId, Tarea task)
    {
        tareaRepository.CreateTask(boardId, task);
        return Created("ubicacion del recurso", task);
    }

    [HttpPut("{id}/Nombre/{nombre}")]
    public ActionResult<Tarea> UpdateNombreTarea(int id, string nombre)
    {
        var tarea = tareaRepository.GetTaskById(id);

        if (tarea.Id == 0) return NotFound();

        tarea.Nombre = nombre;

        tareaRepository.UpdateTask(id, tarea);

        return Ok(tarea);
    }
    [HttpPut("{id}/Estado/{estado}")]
    public ActionResult<Tarea> UpdateEstadoTarea(int id, EstadoTarea estado)
    {
        var tarea = tareaRepository.GetTaskById(id);

        if (tarea.Id == 0) return NotFound();

        tarea.Estado = estado;

        tareaRepository.UpdateTask(id, tarea);

        return Ok(tarea);
    }
    [HttpDelete("{id}")]
    public ActionResult DeleteTarea(int id)
    {
        var tarea = tareaRepository.GetTaskById(id);

        if (tarea.Id == 0) return NotFound();

        tareaRepository.DeleteTask(id);

        return NoContent();
    }

    // faltaria agregar una funcion en la interfaz e implementar en el repositorio de tarea

    // [HttpGet("{estado}")]
    // public ActionResult<int> GetCantidadTareasPorEstado(string estado)
    // {
    //     var cantidad = tareaRepository.GetCantidadTareasPorEstado(estado);

    //     return Ok(cantidad);
    // }

    [HttpGet("Usuario/{id}")]
    public ActionResult<List<Tarea>> GetTareasPorUsuario(int id)
    {
        var tareas = tareaRepository.ListTasksByUser(id);

        return Ok(tareas);
    }

    [HttpGet("Tablero/{id}")]
    public ActionResult<List<Tarea>> GetTareasPorTablero(int id)
    {
        var tareas = tareaRepository.ListTasksByBoard(id);

        return Ok(tareas);
    }


}