using DonaAhora.Data;
using DonaAhora.Models;
using DonaAhora.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonaAhora.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalUsuarios = await _context.Usuarios.CountAsync(),
                TotalDonantes = await _context.Usuarios.CountAsync(u => u.Rol == RolUsuario.Donante),
                TotalSolicitantes = await _context.Usuarios.CountAsync(u => u.Rol == RolUsuario.Solicitante),
                TotalSolicitudes = await _context.Solicitudes.CountAsync(),
                TotalSolicitudesActivas = await _context.Solicitudes.CountAsync(s => s.Estado == EstadoSolicitud.Activa),
                TotalIntereses = await _context.InteresesDonacion.CountAsync(),
                UltimosUsuarios = await _context.Usuarios.OrderByDescending(u => u.FechaRegistro).Take(5).ToListAsync(),
                UltimasSolicitudes = await _context.Solicitudes.Include(s => s.Solicitante)
                    .OrderByDescending(s => s.FechaCreacion).Take(5).ToListAsync()
            };

            return View(vm);
        }

        public async Task<IActionResult> Usuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Donante)
                .OrderByDescending(u => u.FechaRegistro)
                .ToListAsync();
            return View(usuarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstadoUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.Activo = !usuario.Activo;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"El usuario {usuario.Nombre} fue {(usuario.Activo ? "activado" : "desactivado")}.";
            return RedirectToAction(nameof(Usuarios));
        }

        public async Task<IActionResult> Solicitudes()
        {
            var solicitudes = await _context.Solicitudes
                .Include(s => s.Solicitante)
                .Include(s => s.Intereses)
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync();
            return View(solicitudes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstadoSolicitud(int id, EstadoSolicitud nuevoEstado)
        {
            var solicitud = await _context.Solicitudes.FindAsync(id);
            if (solicitud == null) return NotFound();

            solicitud.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"El estado de la solicitud #{solicitud.Id} fue actualizado a {nuevoEstado}.";
            return RedirectToAction(nameof(Solicitudes));
        }
    }
}
