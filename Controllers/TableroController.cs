using kanban.Models;
using kanban.Repository;
using kanban.Controllers.Helpers;
using kanban.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace kanban.Controllers
{
    [ApiController]
    [Route("tableros")]
    public class TableroController : Controller
    {
        private readonly ITableroRepository _tableroRepository;
        private readonly ITareaRepository _tareaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<TableroController> _logger;

        public TableroController(ILogger<TableroController> logger, ITableroRepository tableroRepository, ITareaRepository tareaRepository, IUsuarioRepository usuarioRepository)
        {
            _logger = logger;
            _tableroRepository = tableroRepository;
            _tareaRepository = tareaRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("crear")]
        public IActionResult Crear()
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
                
                var crearTableroModel = new CrearTableroViewModel();

                if(LoginHelper.IsAdmin(HttpContext)) {
                    var usuarios = _usuarioRepository.List();
                    List<UsuarioDropBoxViewModel> usuariosViewModel = new();

                    foreach (var usuario in usuarios)
                    {
                        UsuarioDropBoxViewModel modelo = new(usuario.Id,usuario.NombreDeUsuario);
                        usuariosViewModel.Add(modelo);
                    }
                    crearTableroModel.Usuarios = usuariosViewModel;
                    crearTableroModel.IdUsuarioPropietario = int.Parse(LoginHelper.GetUserId(HttpContext));
                    return View("Administrador/Crear",crearTableroModel);
                } else {
                    return View("Operador/Crear",crearTableroModel);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Crear de TableroController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost("crear")]
        public IActionResult Crear([FromForm] CrearTableroViewModel crearTableroModel)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                if (!ModelState.IsValid) return RedirectToAction("Index", "Home");
                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));
                var tablero = new Tablero()
                {
                    Nombre = crearTableroModel.Nombre,
                    Descripcion = crearTableroModel.Descripcion,
                    IdUsuarioPropietario = sesionId
                };

                if (LoginHelper.IsAdmin(HttpContext)) tablero.IdUsuarioPropietario = crearTableroModel.IdUsuarioPropietario;

                _tableroRepository.Create(tablero);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Crear (HttpPost) de TableroController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                List<Tablero> boards = new();

                ViewBag.EsAdmin = LoginHelper.IsAdmin(HttpContext);

                if (ViewBag.EsAdmin) boards = _tableroRepository.List();

                else boards = _tableroRepository.ListUserBoards(int.Parse(LoginHelper.GetUserId(HttpContext)));

                List<ListarTablerosViewModel> ListarTablerosModel = new();

                foreach (var board in boards)   
                {
                    ListarTablerosViewModel modelo = new(board.Nombre, board.Descripcion, board.Id, board.IdUsuarioPropietario, _usuarioRepository);
                    ListarTablerosModel.Add(modelo);
                }

                return View(ListarTablerosModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Index de TableroController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost("eliminar/{id}")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                var board = _tableroRepository.GetById(id);

                if (board.Id == 0) return NotFound($"No existe el recurso.");

                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));

                var isOwner = board.IdUsuarioPropietario == sesionId;

                var isAdmin = LoginHelper.IsAdmin(HttpContext);

                if (isAdmin || isOwner)
                {
                    var tasks = _tareaRepository.ListByBoard(id);

                    foreach (var task in tasks)
                    {
                        _tareaRepository.Delete(task.Id);
                    }
                    
                    _tableroRepository.Delete(id);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Eliminar de TableroController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet("editar/{id}")]
        public IActionResult Editar(int id)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                var board = _tableroRepository.GetById(id);

                if (board.Id == 0) return NotFound($"No existe el recurso.");

                var modificarTableroModel = new ModificarTableroViewModel
                {
                    Nombre = board.Nombre,
                    Descripcion = board.Descripcion,
                    Id = board.Id
                };

                if(LoginHelper.IsAdmin(HttpContext)) {
                var usuarios = _usuarioRepository.List();
                List<UsuarioDropBoxViewModel> usuariosViewModel = new();

                foreach (var usuario in usuarios)
                {
                    UsuarioDropBoxViewModel modelo = new(usuario.Id,usuario.NombreDeUsuario);
                    usuariosViewModel.Add(modelo);
                }
                modificarTableroModel.Usuarios = usuariosViewModel;
                }

                if (!LoginHelper.IsAdmin(HttpContext))
                {
                    var userBoards = _tableroRepository.ListUserBoards(int.Parse(LoginHelper.GetUserId(HttpContext)));
                    var foundBoard = userBoards.Find(board => board.Id == id);
                    if (foundBoard != null) return View("Operador/Editar",modificarTableroModel);
                    else return NotFound($"No existe el tablero con ID {id}");
                }
                else
                {
                    modificarTableroModel.IdUsuarioPropietario = board.IdUsuarioPropietario;
                    return View("Administrador/Editar",modificarTableroModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Editar (HttpGet) de TableroController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost("editar/{id}")]
        public IActionResult Editar(int id, [FromForm] ModificarTableroViewModel newBoardViewModel)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                if (!ModelState.IsValid) return RedirectToAction("Index", "Home");
                var board = _tableroRepository.GetById(id);

                if (board.Id == 0) return NotFound($"No existe el recurso.");
                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));
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
                    var userBoards = _tableroRepository.ListUserBoards(sesionId);
                    var foundBoard = userBoards.Find(board => board.Id == id);
                    if (foundBoard != null)
                    {
                        newBoard.IdUsuarioPropietario = sesionId;
                        _tableroRepository.Update(id, newBoard);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return NotFound($"Recurso no encontrado.");
                    }
                }
                _tableroRepository.Update(id, newBoard);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Editar (HttpPost) de TableroController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
