using System.Security.Claims;
using DonaAhora.Data;
using DonaAhora.Models;
using DonaAhora.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonaAhora.Controllers
{
    [Authorize(Roles = "Solicitante")]
    public class SolicitanteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SolicitanteController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<IActionResult> Index()
        {
            var misSolicitudes = await _context.Solicitudes
                .Include(s => s.Intereses)
                .Where(s => s.SolicitanteId == UsuarioId)
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync();

            return View(new SolicitanteDashboardViewModel { MisSolicitudes = misSolicitudes });
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new Solicitud { Fecha = DateTime.Now.AddDays(3) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Solicitud model)
        {
            ModelState.Remove(nameof(Solicitud.Solicitante));
            ModelState.Remove(nameof(Solicitud.SolicitanteId));

            if (model.Fecha < DateTime.Now.Date)
                ModelState.AddModelError(nameof(model.Fecha), "La fecha no puede ser en el pasado");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.SolicitanteId = UsuarioId;
            model.Estado = EstadoSolicitud.Activa;
            model.FechaCreacion = DateTime.Now;

            _context.Solicitudes.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tu solicitud de sangre fue publicada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var solicitud = await _context.Solicitudes
                .Include(s => s.Intereses)
                    .ThenInclude(i => i.Donante)
                        .ThenInclude(d => d!.Usuario)
                .FirstOrDefaultAsync(s => s.Id == id && s.SolicitanteId == UsuarioId);

            if (solicitud == null) return NotFound();

            return View(solicitud);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarAyuda(int interesId)
        {
            var interes = await _context.InteresesDonacion
                .Include(i => i.Solicitud)
                .FirstOrDefaultAsync(i => i.Id == interesId && i.Solicitud!.SolicitanteId == UsuarioId);

            if (interes == null) return NotFound();

            interes.Estado = EstadoInteres.Confirmado;

            if (interes.Solicitud!.Estado == EstadoSolicitud.Activa)
                interes.Solicitud.Estado = EstadoSolicitud.EnProceso;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Aceptaste la ayuda de este donante. La solicitud pasó a estado 'En proceso'.";
            return RedirectToAction(nameof(Detalle), new { id = interes.SolicitudId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RechazarAyuda(int interesId)
        {
            var interes = await _context.InteresesDonacion
                .Include(i => i.Solicitud)
                .FirstOrDefaultAsync(i => i.Id == interesId && i.Solicitud!.SolicitanteId == UsuarioId);

            if (interes == null) return NotFound();

            interes.Estado = EstadoInteres.Cancelado;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rechazaste la ayuda de este donante.";
            return RedirectToAction(nameof(Detalle), new { id = interes.SolicitudId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarCompletada(int id)
        {
            var solicitud = await _context.Solicitudes
                .FirstOrDefaultAsync(s => s.Id == id && s.SolicitanteId == UsuarioId);

            if (solicitud == null) return NotFound();

            solicitud.Estado = EstadoSolicitud.Completada;
            await _context.SaveChangesAsync();

            TempData["Success"] = "La solicitud fue marcada como completada.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var solicitud = await _context.Solicitudes
                .FirstOrDefaultAsync(s => s.Id == id && s.SolicitanteId == UsuarioId);

            if (solicitud == null) return NotFound();

            if (solicitud.Estado == EstadoSolicitud.Completada)
            {
                TempData["Error"] = "No puedes editar una solicitud ya completada.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            return View(solicitud);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Solicitud model)
        {
            var solicitud = await _context.Solicitudes
                .FirstOrDefaultAsync(s => s.Id == model.Id && s.SolicitanteId == UsuarioId);

            if (solicitud == null) return NotFound();

            ModelState.Remove(nameof(Solicitud.Solicitante));
            ModelState.Remove(nameof(Solicitud.SolicitanteId));

            if (model.Fecha < DateTime.Now.Date)
                ModelState.AddModelError(nameof(model.Fecha), "La fecha no puede ser en el pasado");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            solicitud.TipoSangre = model.TipoSangre;
            solicitud.Cantidad = model.Cantidad;
            solicitud.Ciudad = model.Ciudad;
            solicitud.Hospital = model.Hospital;
            solicitud.Fecha = model.Fecha;
            solicitud.Urgencia = model.Urgencia;
            solicitud.Descripcion = model.Descripcion;

            await _context.SaveChangesAsync();

            TempData["Success"] = "La solicitud fue actualizada correctamente.";
            return RedirectToAction(nameof(Detalle), new { id = solicitud.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var solicitud = await _context.Solicitudes
                .FirstOrDefaultAsync(s => s.Id == id && s.SolicitanteId == UsuarioId);

            if (solicitud == null) return NotFound();

            _context.Solicitudes.Remove(solicitud);
            await _context.SaveChangesAsync();

            TempData["Success"] = "La solicitud fue eliminada.";
            return RedirectToAction(nameof(Index));
        }
    }
}
