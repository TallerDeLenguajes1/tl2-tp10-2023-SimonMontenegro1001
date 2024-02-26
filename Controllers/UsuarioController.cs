using Microsoft.AspNetCore.Mvc;
using kanban.Repository;
using kanban.Controllers.Helpers;
using kanban.Models;
using kanban.ViewModels;

namespace kanban.Controllers
{
    [ApiController]
    [Route("usuarios")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ILogger<UsuarioController> logger, IUsuarioRepository usuarioRepository)
        {
            _logger = logger;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("ListarUsuarios")]
        public IActionResult ListarUsuarios()
        {
            try
            {

                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
               
                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));

                if (!LoginHelper.IsAdmin(HttpContext)) return NotFound("Recurso no encontrado.");


                List<Usuario> users = _usuarioRepository.List();

                List<ListarUsuariosViewModel> ListaUsuariosModel = new();

                foreach (var user in users)
                {
                    var newUser = new ListarUsuariosViewModel
                    {
                        Id = user.Id,
                        NombreDeUsuario = user.NombreDeUsuario,
                        Rol = user.Rol
                    };
                    ListaUsuariosModel.Add(newUser);
                }

                return View("Administrador/ListarUsuarios",ListaUsuariosModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Index de UsuarioController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Index() {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
               
                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));

                var usuario = _usuarioRepository.GetById(sesionId);

                if(usuario.Id == 0) return NotFound("No existe el recurso.");

                var usuarioViewModel = new UsuarioDropBoxViewModel(usuario.Id,usuario.NombreDeUsuario);

                if(LoginHelper.IsAdmin(HttpContext)) return View("Administrador/Index",usuarioViewModel);

                return View("Operador/Index",usuarioViewModel);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Perfil de UsuarioController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet("crear")]
        public IActionResult Crear()
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");

                if (!LoginHelper.IsAdmin(HttpContext)) return NotFound("El recurso no existe.");

                return View("Administrador/Crear",new CrearUsuarioViewModel());
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en la vista Crear de UsuarioController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost("crear")]
        public IActionResult Crear([FromForm] CrearUsuarioViewModel crearUsuarioModel)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
                
                if (!LoginHelper.IsAdmin(HttpContext)) return NotFound("El recurso no existe.");

                if (!ModelState.IsValid) return View(crearUsuarioModel);

                var user = new Usuario
                {
                    NombreDeUsuario = crearUsuarioModel.NombreDeUsuario,
                    Contrasena = crearUsuarioModel.Contrasena,
                    Rol = crearUsuarioModel.Rol
                };

                _usuarioRepository.Create(user);

                return RedirectToAction("ListarUsuarios");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear en UsuarioController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }
           
        [HttpGet("editar/{id}")]
        public IActionResult Editar(int id)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
                
                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));

                var usuario = _usuarioRepository.GetById(id);

                if (usuario.Id == 0) return NotFound("No se encontro el recurso.");

                var modificarUsuarioModel = new ModificarUsuarioViewModel
                {
                    Id = usuario.Id,
                    NombreDeUsuario = usuario.NombreDeUsuario,
                    Rol = usuario.Rol
                };

                if (!LoginHelper.IsAdmin(HttpContext) || sesionId == id) {
                    if(id != sesionId) NotFound("No se encontro el recurso.");
                    return View("Operador/Editar",modificarUsuarioModel);
                }
                
                return View("Administrador/Editar",modificarUsuarioModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Editar de UsuarioController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost("editar/{id}")]
        public IActionResult Editar(int id, [FromForm] ModificarUsuarioViewModel modificarUsuarioModel)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
                
                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));

                if (!ModelState.IsValid) return View(modificarUsuarioModel);

                var usuario = _usuarioRepository.GetById(id);

                if (usuario.Id == 0) return NotFound("No se encontro el recurso.");

                if (!LoginHelper.IsAdmin(HttpContext) || sesionId == id){
                    if(sesionId != id) return NotFound("No se encontro el recurso.");
                    var usuarioSesionEditado = new Usuario
                    {
                        Id = modificarUsuarioModel.Id,
                        NombreDeUsuario = modificarUsuarioModel.NombreDeUsuario,
                        Rol = usuario.Rol,
                        Contrasena = usuario.Contrasena
                    };
                    _usuarioRepository.Update(id, usuarioSesionEditado);
                    
                    return RedirectToAction("Index");
                }
                
                
                var usuarioEditado = new Usuario
                {
                    Id = modificarUsuarioModel.Id,
                    NombreDeUsuario = modificarUsuarioModel.NombreDeUsuario,
                    Rol = modificarUsuarioModel.Rol,
                    Contrasena = usuario.Contrasena
                };

                _usuarioRepository.Update(id, usuarioEditado);
                if(LoginHelper.IsAdmin(HttpContext) && id != sesionId) return RedirectToAction("ListarUsuarios","Usuarios");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Editar de UsuarioController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost("eliminar/{id}")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
                
                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));

                var usuario = _usuarioRepository.GetById(id);

                if (usuario.Id == 0) return NotFound("No se encontro el recurso.");

                if (!LoginHelper.IsAdmin(HttpContext)) {
                    if (sesionId != id) return NotFound("No se encontro el recurso.");
                    _usuarioRepository.Delete(id);
                    HttpContext.Session.Clear();
                    return RedirectToAction("Index", "Login");
                } else {
                    if(id == sesionId) return NotFound("No se encontro el recurso.");
                    _usuarioRepository.Delete(id);
                    return RedirectToAction("ListarUsuarios", "Usuarios");  
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Eliminar de UsuarioController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet("cambiarContrasena/{id}")]
        public IActionResult CambiarContrasena(int id)
        {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
                
                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));

                var usuario = _usuarioRepository.GetById(id);

                if (usuario.Id == 0) return NotFound("No se encontro el recurso.");

                var cambiarContrasenaModel = new CambiarContrasenaViewModel();

                if (!LoginHelper.IsAdmin(HttpContext) && id != sesionId) NotFound("No se encontro el recurso.");
                
                return View("CambiarContrasena",cambiarContrasenaModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Editar de UsuarioController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost("CambiarContrasena/{id}")]
        public IActionResult CambiarContrasena([FromRoute] int id, [FromForm] CambiarContrasenaViewModel cambiarContrasenaViewModel) {
            try
            {
                if (!LoginHelper.IsLogged(HttpContext)) return RedirectToAction("Index", "Login");
            
                var sesionId = int.Parse(LoginHelper.GetUserId(HttpContext));

                if (!ModelState.IsValid) return View(cambiarContrasenaViewModel);

                var usuario = _usuarioRepository.GetById(id);

                if (usuario.Id == 0) return NotFound("No se encontro el recurso.");
                if(!(sesionId == id || LoginHelper.IsAdmin(HttpContext))) return NotFound("No existe el recurso.");
                
                if (cambiarContrasenaViewModel.ContrasenaAnterior != usuario.Contrasena) {
                    ModelState.AddModelError("ContrasenaAnterior", "La contraseña anterior es incorrecta.");
                    return View(cambiarContrasenaViewModel);
                }

                if (cambiarContrasenaViewModel.NuevaContrasena != cambiarContrasenaViewModel.ConfirmarNuevaContrasena) {
                    ModelState.AddModelError("ConfirmarNuevaContrasena", "Las contraseñas no coinciden.");
                    return View(cambiarContrasenaViewModel);
                }

                var usuarioEditado = new Usuario() {
                    NombreDeUsuario = usuario.NombreDeUsuario,
                    Contrasena = cambiarContrasenaViewModel.NuevaContrasena,
                    Rol = usuario.Rol
                };

                _usuarioRepository.Update(id,usuarioEditado);

                if (LoginHelper.IsAdmin(HttpContext) && id != sesionId) return RedirectToAction("ListarUsuarios","Usuarios");

                return RedirectToAction("Index", "Usuarios"); 

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Editar de UsuarioController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
