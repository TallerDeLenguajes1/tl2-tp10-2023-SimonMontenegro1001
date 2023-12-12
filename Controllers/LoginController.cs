using Microsoft.AspNetCore.Mvc;
using kanban.Models;
using System.Diagnostics;
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
            try
            {
                return View(new LoginViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en el endpoint Index de LoginCorntoller: {ex.Message}");
                return View("Error");
            }
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
                    return RedirectToAction("Index", "Home");
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
                return View("Error");
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void LogInUser(Usuario user)
        {
            HttpContext.Session.SetString("id", user.Id.ToString());
            HttpContext.Session.SetString("usuario", user.NombreDeUsuario);
            HttpContext.Session.SetString("rol", user.Rol.ToString());
        }
    }
}
