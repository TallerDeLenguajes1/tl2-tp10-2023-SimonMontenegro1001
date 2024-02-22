using kanban.Models;
using kanban.ViewModels;
using kanban.Repository;
using kanban.Controllers.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace kanban.Controllers
{
    [ApiController]
    [Route("tareas")]
    public class TareaController : Controller
    {
        private readonly ITareaRepository _tareaRepository;
        private readonly ITableroRepository _tableroRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<TareaController> _logger;

        public TareaController(ILogger<TareaController> logger, ITableroRepository tableroRepository, ITareaRepository tareaRepository,IUsuarioRepository usuarioRepository)
        {
            _logger = logger;
            _tableroRepository = tableroRepository;
            _tareaRepository = tareaRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost("crear/{boardId}")]
        public IActionResult Crear(int boardId, [FromForm] CrearTareaViewModel tareaViewModel)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Crear de TareaController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet("crear/{tableroId}")]
        public IActionResult Crear(int tableroId)
        {
            try
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

                if(!LoginHelper.IsAdmin(HttpContext)) {
                    var tableros = _tableroRepository.ListUserBoards(int.Parse(LoginHelper.GetUserId(HttpContext)));
                    var foundBoard = tableros.Find(tablero => tablero.Id == tableroId);
                    if (foundBoard == null)  return NotFound($"No existe el tablero con ID {tableroId}");
                }

                var usuarios = _usuarioRepository.List();
                List<UsuarioDropBoxViewModel> usuariosViewModel = new();

                foreach (var usuario in usuarios)
                {
                    UsuarioDropBoxViewModel modelo = new(usuario.Id,usuario.NombreDeUsuario);
                    usuariosViewModel.Add(modelo);
                }
                crearTareaModel.Usuarios = usuariosViewModel;

                return View(crearTareaModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Crear (HttpGet) de TareaController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet("editar/{id}")]
        public IActionResult Editar(int id)
        {
            try
            {
                
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                var tarea = _tareaRepository.GetById(id);
                if (tarea.Id == 0) return NotFound($"No se encontró la tarea con ID {id}");

                var tableros = _tableroRepository.ListUserBoards(int.Parse(LoginHelper.GetUserId(HttpContext)));
                var foundBoard = tableros.Find(tablero => tablero.IdUsuarioPropietario == int.Parse(LoginHelper.GetUserId(HttpContext)));
                
                if(!LoginHelper.IsAdmin(HttpContext) && tarea.IdUsuarioAsignado == int.Parse(LoginHelper.GetUserId(HttpContext)) && foundBoard == null) {    
                    var editarTareaModel = new ModificarTareaViewModel
                        {
                            Estado = tarea.Estado,
                        };
                    return View(editarTareaModel);
                }

                if(LoginHelper.IsAdmin(HttpContext) || foundBoard != null) {
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
                return NotFound($"No tiene permisos necesarios para editar la tarea con ID {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Editar (HttpGet) de TareaController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost("editar/{id}")]
        public IActionResult Editar(int id, [FromForm] ModificarTareaViewModel tareaViewModel)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Home");
                if (!ModelState.IsValid) return RedirectToAction("Index", "Home");

                var tarea = _tareaRepository.GetById(id);
                if (tarea.Id == 0) return NotFound($"No se encontró la tarea con ID {id}");

                var tableros = _tableroRepository.ListUserBoards(int.Parse(LoginHelper.GetUserId(HttpContext)));
                var foundBoard = tableros.Find(tablero => tablero.IdUsuarioPropietario == int.Parse(LoginHelper.GetUserId(HttpContext)));
                
                if(!LoginHelper.IsAdmin(HttpContext) && tarea.IdUsuarioAsignado == int.Parse(LoginHelper.GetUserId(HttpContext)) && foundBoard == null) {    
                    var editarTareaModel = new Tarea
                        {
                            Estado = tareaViewModel.Estado,
                            Id = tarea.Id,
                            IdTablero = tarea.IdTablero,
                            Nombre = tarea.Nombre,
                            Descripcion = tarea.Descripcion,
                            Color = tarea.Color,
                            IdUsuarioAsignado = tarea.IdUsuarioAsignado,
                        };
                        
                    _tareaRepository.Update(id, editarTareaModel);
                }
                if(LoginHelper.IsAdmin(HttpContext) || foundBoard != null) { 
                    var tareaModel = new Tarea
                    {
                        Id = tareaViewModel.Id,
                        IdTablero = tareaViewModel.IdTablero,
                        Estado = tareaViewModel.Estado,
                        Nombre = tareaViewModel.Nombre,
                        Descripcion = tareaViewModel.Descripcion,
                        Color = tareaViewModel.Color,
                        IdUsuarioAsignado = tareaViewModel.IdUsuarioAsignado,
                    };

                    _tareaRepository.Update(id, tareaModel);
                }
                
                return RedirectToAction("ListByBoard", new { id = tarea.IdTablero });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Editar (HttpPost) de TareaController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost("eliminar/{id}")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
                var tarea = _tareaRepository.GetById(id);

                if (tarea.Id == 0)  return NotFound($"No existe la tarea con ID {id}");

                var tableros = _tableroRepository.ListUserBoards(int.Parse(LoginHelper.GetUserId(HttpContext)));
                var foundBoard = tableros.Find(tablero => tablero.IdUsuarioPropietario == int.Parse(LoginHelper.GetUserId(HttpContext)));

                if (!LoginHelper.IsAdmin(HttpContext) && foundBoard == null)
                {
                    return NotFound($"No es propietario del tablero, no tiene permitido eliminar tareas");
                }
                else if (LoginHelper.IsAdmin(HttpContext) || foundBoard != null)
                {
                    _tareaRepository.Delete(id);
                    return Ok("tarea eliminada"); 
                }
                else
                {
                    return Forbid("No autorizado");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Eliminar de TareaController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet("usuario/{id}")]
        public IActionResult Index(int id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Index de TareaController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet("tablero/{id}")]
        public IActionResult ListByBoard(int id)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                ViewBag.EsAdmin = LoginHelper.IsAdmin(HttpContext);

                if (!LoginHelper.IsAdmin(HttpContext))
                {
                    var UserBoards = _tableroRepository.ListUserAssignedBoards(int.Parse(LoginHelper.GetUserId(HttpContext)));
                    var FoundBoard = UserBoards.Find(board => board.Id == id);
                
                    if (FoundBoard == null) return NotFound($"No existe el tablero de Id {id}");
                
                   }


               var tareas = _tareaRepository.ListByBoard(id);
                    var listaTareasModel = new List<ListarTareasViewModel>();
                    foreach (var tarea in tareas)
                    {
                         var esPropietario = tarea.IdUsuarioAsignado == int.Parse(LoginHelper.GetUserId(HttpContext));
                        var listarTareasModel = new ListarTareasViewModel
                        {
                            Id = tarea.Id,
                            IdTablero = tarea.IdTablero,
                            Nombre = tarea.Nombre,
                            Descripcion = tarea.Descripcion,
                            Estado = tarea.Estado,
                            EsPropietario = esPropietario
                        };
                        listaTareasModel.Add(listarTareasModel);
                    }

                return View(listaTareasModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint ListByBoard de TareaController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
