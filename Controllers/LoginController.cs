using Microsoft.AspNetCore.Mvc;
using kanban.Models;
using kanban.Repository;
using kanban.Controllers.Helpers;
using kanban.ViewModels;

namespace kanban.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IUsuarioRepository _usuarioRepository;

        public LoginController(ILogger<LoginController> logger, IUsuarioRepository usuarioRepository)
        {
            _logger = logger;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel loginModel)
        {
            try
            {
                if (!ModelState.IsValid) return RedirectToAction("Index", "Home");
                var sessionUsername = LoginHelper.GetUserName(HttpContext);
                var sessionId = LoginHelper.GetUserId(HttpContext);

                if (!string.IsNullOrEmpty(sessionUsername) && !string.IsNullOrEmpty(sessionId))
                {
                    var sessionUser = _usuarioRepository.GetById(int.Parse(sessionId));
                    if (sessionUser.NombreDeUsuario == loginModel.Username)
                    {
                        _logger.LogInformation($"El usuario {loginModel.Username} ingresó correctamente");
                        return RedirectToAction("Index", "Home");
                    }
                }

                var userFound = _usuarioRepository.GetByUsername(loginModel.Username);

                if (userFound != null && userFound.Contrasena == loginModel.Password)
                {
                    LogInUser(userFound);
                    _logger.LogInformation($"El usuario {loginModel.Username} ingresó correctamente");
                    return RedirectToAction("Index", "Tablero");
                }
                else
                {
                    _logger.LogWarning($"Intento de acceso inválido - Usuario: {loginModel.Username} Clave ingresada: {loginModel.Password}");
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Login de LoginController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        private void LogInUser(Usuario user)
        {
            HttpContext.Session.SetString("id", user.Id.ToString());
            HttpContext.Session.SetString("usuario", user.NombreDeUsuario);
            HttpContext.Session.SetString("rol", user.Rol.ToString());
        }

        [HttpPost]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();

                _logger.LogInformation("Usuario desconectado correctamente");

                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Logout de LoginController: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
