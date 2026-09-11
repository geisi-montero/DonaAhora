using System.Security.Claims;
using DonaAhora.Data;
using DonaAhora.Models;
using DonaAhora.Models.ViewModels;
using DonaAhora.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonaAhora.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model.Rol == RolUsuario.Donante)
            {
                if (model.TipoSangre == null)
                    ModelState.AddModelError(nameof(model.TipoSangre), "Selecciona tu tipo de sangre");
                if (string.IsNullOrWhiteSpace(model.Ciudad))
                    ModelState.AddModelError(nameof(model.Ciudad), "La ciudad es obligatoria para donantes");
            }

            if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Ya existe una cuenta con este correo");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                Email = model.Email,
                Telefono = model.Telefono,
                Rol = model.Rol,
                PasswordHash = PasswordHelper.Hash(model.Password),
                FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            if (model.Rol == RolUsuario.Donante)
            {
                var donante = new Donante
                {
                    UsuarioId = usuario.Id,
                    TipoSangre = model.TipoSangre!.Value,
                    Ciudad = model.Ciudad!,
                    Disponible = true
                };
                _context.Donantes.Add(donante);
                await _context.SaveChangesAsync();
            }

            await IniciarSesion(usuario);

            TempData["Success"] = $"¡Bienvenido/a a Dona Ahora, {usuario.Nombre}! Tu cuenta fue creada exitosamente.";
            return RedirectToDashboard(usuario.Rol);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (usuario == null || !PasswordHelper.Verify(model.Password, usuario.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos");
                return View(model);
            }

            if (!usuario.Activo)
            {
                ModelState.AddModelError(string.Empty, "Esta cuenta ha sido desactivada. Contacta al administrador.");
                return View(model);
            }

            await IniciarSesion(usuario);

            TempData["Success"] = $"¡Hola de nuevo, {usuario.Nombre}!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToDashboard(usuario.Rol);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task IniciarSesion(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Nombre),
                new(ClaimTypes.Email, usuario.Email),
                new(ClaimTypes.Role, usuario.Rol.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = true });
        }

        private IActionResult RedirectToDashboard(RolUsuario rol)
        {
            return rol switch
            {
                RolUsuario.Donante => RedirectToAction("Panel", "Donante"),
                RolUsuario.Solicitante => RedirectToAction("Index", "Solicitante"),
                RolUsuario.Administrador => RedirectToAction("Index", "Admin"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}
