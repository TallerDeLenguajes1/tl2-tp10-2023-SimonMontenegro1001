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
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                if (!ModelState.IsValid) return View(tareaViewModel);

                var task = new Tarea
                {
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

                var crearTareaModel = new CrearTareaViewModel
                {
                    IdTablero = tableroId
                };

                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));

                if(!LoginHelper.IsAdmin(HttpContext)) {
                    var tableros = _tableroRepository.ListUserBoards(sesionId);
                    var tableroBuscado = tableros.Find(tablero => tablero.Id == tableroId);
                    if (tableroBuscado == null)  return NotFound($"No existe el recurso");
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
                if (tarea.Id == 0) return NotFound($"No se encontro el recurso.");
                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));
                var isAdmin = LoginHelper.IsAdmin(HttpContext);
                

                var tableroTarea = _tableroRepository.GetById(tarea.IdTablero);

                var isOwner = tableroTarea.IdUsuarioPropietario == sesionId;

                
                var usuarios = _usuarioRepository.List();
                List<UsuarioDropBoxViewModel> usuariosViewModel = new();

                foreach (var usuario in usuarios)
                {
                    UsuarioDropBoxViewModel modelo = new(usuario.Id,usuario.NombreDeUsuario);
                    usuariosViewModel.Add(modelo);
                }

                if(!isAdmin && !isOwner && tableroTarea.Id != 0) {    
                    var editarTareaModel = new EditarTareaViewModel
                        {
                            Estado = tarea.Estado,
                        };
                    return View("Operador/Editar",editarTareaModel);
                }

                if(!isAdmin && isOwner) {

                    var editarTareaModel = new EditarTareaViewModel
                        {
                            Id = tarea.Id,
                            Estado = tarea.Estado,
                            Nombre = tarea.Nombre,
                            Descripcion = tarea.Descripcion,
                            Color = tarea.Color,
                            IdUsuarioAsignado = tarea.IdUsuarioAsignado,
                            Usuarios = usuariosViewModel
                        };
                    return View("Operador/EditarAdminTablero",editarTareaModel);
                }

                if(isAdmin) {
                    var tableros = _tableroRepository.List();

                    List<TableroDropBoxViewModel> tableroViewModel = new();

                    foreach (var tablero in tableros)
                    {
                        TableroDropBoxViewModel modelo = new(tablero.Id,tablero.Nombre);
                        tableroViewModel.Add(modelo);
                    }

                    var editarTareaModel = new EditarTareaViewModel
                        {
                            Id = tarea.Id,
                            IdTablero = tarea.IdTablero,
                            Estado = tarea.Estado,
                            Nombre = tarea.Nombre,
                            Descripcion = tarea.Descripcion,
                            Color = tarea.Color,
                            IdUsuarioAsignado = tarea.IdUsuarioAsignado,
                            Usuarios = usuariosViewModel,
                            Tableros = tableroViewModel
                        };
                    return View("Administrador/Editar",editarTareaModel);
                }

                return NotFound($"No existe el recurso.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Editar (HttpGet) de TareaController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost("editar/{id}")]
        public IActionResult Editar(int id, [FromForm] EditarTareaViewModel tareaViewModel)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                if (!ModelState.IsValid) return View(tareaViewModel);

                var tarea = _tareaRepository.GetById(id);
                if (tarea.Id == 0) return NotFound($"No se encontro el recurso.");

                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));
      
                var isAdmin = LoginHelper.IsAdmin(HttpContext);

                var tableroTarea = _tableroRepository.GetById(tarea.IdTablero);

                var isOwner = tableroTarea.IdUsuarioPropietario == sesionId;
                
                if(!isAdmin && !isOwner && tableroTarea.Id != 0) {    
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
                if(!isAdmin && isOwner) { 
                    var tareaModel = new Tarea
                    {
                        Id = tareaViewModel.Id,
                        IdTablero = tarea.IdTablero,
                        Estado = tareaViewModel.Estado,
                        Nombre = tareaViewModel.Nombre,
                        Descripcion = tareaViewModel.Descripcion,
                        Color = tareaViewModel.Color,
                        IdUsuarioAsignado = tareaViewModel.IdUsuarioAsignado,
                    };

                    _tareaRepository.Update(id, tareaModel);
                }
                
                if(isAdmin) { 
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

        [HttpGet("CambiarEstado/{id}")]
        public IActionResult CambiarEstado(int id)
        {
            try
            {
                // Verificar si el usuario está autenticado
                if (!LoginHelper.IsLogged(HttpContext))
                    return RedirectToAction("Index", "Login");

                // Obtener la tarea por su ID
                var tarea = _tareaRepository.GetById(id);
                if (tarea.Id == 0)
                    return NotFound($"No se encontró la tarea.");

                var isAdmin = LoginHelper.IsAdmin(HttpContext);

                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));

                var tableroTarea = _tableroRepository.GetById(tarea.IdTablero);

                var isOwner = tableroTarea.IdUsuarioPropietario == sesionId;

                if(isAdmin || isOwner || tarea.IdUsuarioAsignado == sesionId) {
                    var editarTareaModel = new CambiarEstadoViewModel
                    {
                        Id = tarea.Id,
                        Estado = tarea.Estado
                    };    
                    return View(editarTareaModel);
                }
                return RedirectToAction("Error", "Home");
            }
            catch (Exception ex)
            {
                // En caso de error, se redirige a una página de error
                _logger.LogError($"Error en el endpoint CambiarEstado (HttpGet) de TareaController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost("CambiarEstado/{id}")]
        public IActionResult CambiarEstado(int id, [FromForm] CambiarEstadoViewModel cambiarEstadoViewModel) {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                var tarea = _tareaRepository.GetById(id);
                if (tarea.Id == 0) return NotFound($"No se encontro el recurso.");

                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));

                var isAdmin = LoginHelper.IsAdmin(HttpContext);

                var tableroTarea = _tableroRepository.GetById(tarea.IdTablero);

                var isOwner = tableroTarea.IdUsuarioPropietario == sesionId;

                if(isAdmin || isOwner || tarea.IdUsuarioAsignado == sesionId) {
                    tarea.Estado = cambiarEstadoViewModel.Estado;
                    _tareaRepository.Update(id, tarea);
                    return RedirectToAction("ListByBoard", new { id = tarea.IdTablero }); 
                }
                return NotFound($"No se encontro el recurso.");
        }

        [HttpPost("eliminar/{id}")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                var tarea = _tareaRepository.GetById(id);
                if (tarea.Id == 0) return NotFound($"No se encontro el recurso.");
                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));
                var isAdmin = LoginHelper.IsAdmin(HttpContext);
                

                var tableroTarea = _tableroRepository.GetById(tarea.IdTablero);

                var isOwner = tableroTarea.IdUsuarioPropietario == sesionId;

                if (!LoginHelper.IsAdmin(HttpContext) && isOwner)
                {
                    _tareaRepository.Delete(id);
                    return Ok("tarea eliminada"); 
                }

                if (LoginHelper.IsAdmin(HttpContext))
                {
                    _tareaRepository.Delete(id);
                    return Ok("tarea eliminada"); 
                }
                return NotFound("No se encontro el recuso."); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Eliminar de TareaController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        
        [HttpGet("ListarTodasLasTareas")]
        public IActionResult ListarTodasLasTareas() {
            if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
            var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));
            var usuario = _usuarioRepository.GetById(sesionId);
            if(usuario.Id == 0) return NotFound("No existe el recurso.");

            if(sesionId != usuario.Id) return NotFound("No existe el recurso.");
            if(!LoginHelper.IsAdmin(HttpContext)) return NotFound("No existe el recurso.");

            var tareas = _tareaRepository.List();
            var tableros = _tableroRepository.List();
            
            var tareasPorTablero = new List<ListarTablerosConTareasViewModel>();

            foreach (var tablero in tableros)
            {
                var tareasDelTablero = tareas.Where(t => t.IdTablero == tablero.Id).ToList();
                
                var tareasDelTableroViewModel = new List<ListarTareasViewModel>();

                foreach (var tarea in tareasDelTablero)
                    {   
                        var esPropietario = tarea.IdUsuarioAsignado == sesionId;
                        var listarTareasModel = new ListarTareasViewModel
                        {
                            Id = tarea.Id,
                            IdTablero = tarea.IdTablero,
                            Nombre = tarea.Nombre,
                            Descripcion = tarea.Descripcion,
                            Estado = tarea.Estado,
                            EsPropietario = esPropietario
                        };
                        tareasDelTableroViewModel.Add(listarTareasModel);
                    }
                
                var objetoTablero = new ListarTablerosConTareasViewModel()
                {
                    NombreTablero = tablero.Nombre,
                    EsPropietario = tablero.IdUsuarioPropietario == sesionId,
                    Tareas = tareasDelTableroViewModel
                };

                tareasPorTablero.Add(objetoTablero);
            }

            return View(tareasPorTablero);
        }

        [HttpGet]
        public IActionResult Index() {
            if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
            var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));
            var usuario = _usuarioRepository.GetById(sesionId);
            if(usuario.Id == 0) return NotFound("No existe el recurso.");

            if(sesionId != usuario.Id) return NotFound("No existe el recurso.");

            var tareas = _tareaRepository.ListByUser(sesionId);
            var tableros = _tableroRepository.ListUserAssignedBoards(sesionId);

            
            var tareasPorTablero = new List<ListarTablerosConTareasViewModel>();

            foreach (var tablero in tableros)
            {
                var tareasDelTablero = tareas.Where(t => t.IdTablero == tablero.Id).ToList();
                
                var tareasDelTableroViewModel = new List<ListarTareasViewModel>();

                foreach (var tarea in tareasDelTablero)
                    {   
                        var esPropietario = tarea.IdUsuarioAsignado == sesionId;
                        var listarTareasModel = new ListarTareasViewModel
                        {
                            Id = tarea.Id,
                            IdTablero = tarea.IdTablero,
                            Nombre = tarea.Nombre,
                            Descripcion = tarea.Descripcion,
                            Estado = tarea.Estado,
                            EsPropietario = esPropietario
                        };
                        tareasDelTableroViewModel.Add(listarTareasModel);
                    }
                
                var objetoTablero = new ListarTablerosConTareasViewModel()
                {
                    NombreTablero = tablero.Nombre,
                    EsPropietario = tablero.IdUsuarioPropietario == sesionId,
                    Tareas = tareasDelTableroViewModel
                };

                tareasPorTablero.Add(objetoTablero);
            }

            return View(tareasPorTablero);
        }   

        [HttpGet("tablero/{id}")]
        public IActionResult ListByBoard(int id)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));
                var tareas = new List<Tarea>();

                var tablero = _tableroRepository.GetById(id);
                if(tablero.Id == 0) return NotFound("No existe el recurso");


                if (!LoginHelper.IsAdmin(HttpContext))
                {
                    var UserBoards = _tableroRepository.ListUserAssignedBoards(sesionId);
                    var FoundBoard = UserBoards.Find(board => board.Id == id);
                    if (FoundBoard == null) return NotFound($"No existe el recurso");

                    if(FoundBoard.IdUsuarioPropietario != sesionId) {
                        tareas = _tareaRepository.ListByUserAndBoard(sesionId,FoundBoard.Id);
                    } else {
                        tareas = _tareaRepository.ListByBoard(FoundBoard.Id);
                    }
                } else {
                    tareas = _tareaRepository.ListByBoard(id);
                }

                    var listarTareasViewModel = new ListarTablerosConTareasViewModel() {
                        NombreTablero = tablero.Nombre,
                        EsPropietario = tablero.IdUsuarioPropietario == sesionId || LoginHelper.IsAdmin(HttpContext),
                        Tareas = new List<ListarTareasViewModel>()
                    };

                    foreach (var tarea in tareas)
                    {   
                        var listarTareasModel = new ListarTareasViewModel
                        {
                            Id = tarea.Id,
                            IdTablero = tarea.IdTablero,
                            Nombre = tarea.Nombre,
                            Descripcion = tarea.Descripcion,
                            Estado = tarea.Estado,
                            EsPropietario = tarea.IdUsuarioAsignado == sesionId
                        };
                        listarTareasViewModel.Tareas.Add(listarTareasModel);
                    }

                return View(listarTareasViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint ListByBoard de TareaController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet("ListByUser/{id}")]
        public IActionResult ListByUser(int id)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));

                var usuario = _usuarioRepository.GetById(id);
                if(usuario.Id == 0) return NotFound("No existe el recurso");


                if (!LoginHelper.IsAdmin(HttpContext)) return NotFound("No existe el recurso");;

                var tareas = _tareaRepository.ListByUser(id);
                var listarTareasViewModel = new List<ListarTareasViewModel>();
                

                foreach (var tarea in tareas)
                {   
                    var listarTareasModel = new ListarTareasViewModel
                    {
                        Id = tarea.Id,
                        IdTablero = tarea.IdTablero,
                        Nombre = tarea.Nombre,
                        Descripcion = tarea.Descripcion,
                        Estado = tarea.Estado,
                        EsPropietario = tarea.IdUsuarioAsignado == sesionId
                    };
                    listarTareasViewModel.Add(listarTareasModel);
                }

                return View("Administrador/ListByUser",listarTareasViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint ListByBoard de TareaController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
