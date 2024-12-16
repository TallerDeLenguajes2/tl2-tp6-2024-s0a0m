using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_s0a0m.Models;
using tl2_tp6_2024_s0a0m.Repositorios;
using tl2_tp6_2024_s0a0m.ViewModels;
using tl2_tp6_2024_s0a0m.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace tl2_tp6_2024_s0a0m.Controllers
{
    [Route("[controller]")]
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly IAuthenticationService _authenticationService;

        public UsuarioController(ILogger<UsuarioController> logger, IAuthenticationService authenticationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
        }

        public IActionResult Index()
        {
            var model = new UsuarioViewModel
            {
                IsAuthenticated = User.Identity.IsAuthenticated // Uso de User.Identity para verificar autenticación
            };
            return View(model);
        }

        [HttpPost("Login")]
        public IActionResult Login(UsuarioViewModel usuarioViewModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Intento de acceso inválido - Usuario: {Usuario} - Motivo: Modelo inválido", usuarioViewModel.NombreUsuario);
                return RedirectToAction("Index");
            }

            // Aquí se recomienda usar algún sistema seguro para manejar contraseñas
            if (_authenticationService.Login(usuarioViewModel.NombreUsuario, usuarioViewModel.Contrasena))
            {
                _logger.LogInformation("El usuario {Usuario} ingresó correctamente.", usuarioViewModel.NombreUsuario);

                // Utilizar cookies o tokens en lugar de session para mejor manejo de autenticación
                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("NombreUsuario", usuarioViewModel.NombreUsuario);

                return RedirectToAction("Index", "Presupuesto");
            }

            // Log para intentos fallidos
            _logger.LogWarning("Intento de acceso inválido - Usuario: {Usuario} - Clave ingresada: {Clave}",
                usuarioViewModel.NombreUsuario, usuarioViewModel.Contrasena);

            // Preparar el modelo para reintento
            usuarioViewModel.ErrorMessage = "Credenciales inválidas.";
            usuarioViewModel.IsAuthenticated = false;

            return View("Index", usuarioViewModel);
        }

        [HttpGet("Logout")]
        [AccessLevelAuthorize("Administrador", "Cliente")]
        public IActionResult Logout()
        {
            _logger.LogInformation("El usuario {Usuario} ha cerrado sesión.", User.Identity.Name);

            HttpContext.Session.Clear(); // Limpiar todas las variables de sesión

            // Redirigir a la página de inicio
            return RedirectToAction("Index");
        }
    }
}
