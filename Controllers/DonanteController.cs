using System.Security.Claims;
using DonaAhora.Data;
using DonaAhora.Models;
using DonaAhora.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonaAhora.Controllers
{
    [Authorize(Roles = "Donante")]
    public class DonanteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonanteController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<IActionResult> Panel()
        {
            var donante = await _context.Donantes
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(d => d.UsuarioId == UsuarioId);

            if (donante == null) return RedirectToAction("Index", "Home");

            var idsConInteres = await _context.InteresesDonacion
                .Where(i => i.DonanteId == donante.Id)
                .Select(i => i.SolicitudId)
                .ToListAsync();

            // solicitudes que este tipo de sangre puede cubrir
            var tiposCompatibles = Enum.GetValues<TipoSangre>()
                .Where(tipoReceptor => tipoReceptor.DonantesCompatibles().Contains(donante.TipoSangre))
                .ToList();

            var solicitudesCompatibles = await _context.Solicitudes
                .Include(s => s.Solicitante)
                .Where(s => s.Estado == EstadoSolicitud.Activa && tiposCompatibles.Contains(s.TipoSangre))
                .OrderByDescending(s => s.Urgencia)
                .ThenByDescending(s => s.FechaCreacion)
                .ToListAsync();

            var misIntereses = await _context.InteresesDonacion
                .Include(i => i.Solicitud)
                .Where(i => i.DonanteId == donante.Id)
                .OrderByDescending(i => i.FechaInteres)
                .ToListAsync();

            var vm = new DonantePanelViewModel
            {
                Donante = donante,
                SolicitudesCompatibles = solicitudesCompatibles,
                MisIntereses = misIntereses
            };

            ViewBag.IdsConInteres = idsConInteres;

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Editar()
        {
            var donante = await _context.Donantes.FirstOrDefaultAsync(d => d.UsuarioId == UsuarioId);
            if (donante == null) return RedirectToAction("Index", "Home");
            return View(donante);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Donante model)
        {
            var donante = await _context.Donantes.FirstOrDefaultAsync(d => d.UsuarioId == UsuarioId);
            if (donante == null) return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(model.Ciudad))
                ModelState.AddModelError(nameof(model.Ciudad), "La ciudad es obligatoria");

            if (!ModelState.IsValid)
            {
                model.Id = donante.Id;
                return View(model);
            }

            donante.TipoSangre = model.TipoSangre;
            donante.Ciudad = model.Ciudad;
            donante.Disponible = model.Disponible;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Tu información se actualizó correctamente.";
            return RedirectToAction(nameof(Panel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuieroAyudar(int solicitudId)
        {
            var donante = await _context.Donantes.FirstOrDefaultAsync(d => d.UsuarioId == UsuarioId);
            if (donante == null) return RedirectToAction("Index", "Home");

            var solicitud = await _context.Solicitudes.FindAsync(solicitudId);
            if (solicitud == null || solicitud.Estado != EstadoSolicitud.Activa)
            {
                TempData["Error"] = "Esta solicitud ya no está disponible.";
                return RedirectToAction(nameof(Panel));
            }

            var yaExiste = await _context.InteresesDonacion
                .AnyAsync(i => i.DonanteId == donante.Id && i.SolicitudId == solicitudId);

            if (yaExiste)
            {
                TempData["Error"] = "Ya registraste tu interés en esta solicitud anteriormente.";
                return RedirectToAction(nameof(Panel));
            }

            var interes = new InteresDonacion
            {
                DonanteId = donante.Id,
                SolicitudId = solicitudId,
                FechaInteres = DateTime.Now,
                Estado = EstadoInteres.Pendiente
            };

            _context.InteresesDonacion.Add(interes);
            await _context.SaveChangesAsync();

            TempData["Success"] = "¡Gracias! Tu interés en donar fue registrado. El solicitante y el hospital podrán contactarte.";
            return RedirectToAction(nameof(Panel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarInteres(int interesId)
        {
            var donante = await _context.Donantes.FirstOrDefaultAsync(d => d.UsuarioId == UsuarioId);
            if (donante == null) return RedirectToAction("Index", "Home");

            var interes = await _context.InteresesDonacion
                .FirstOrDefaultAsync(i => i.Id == interesId && i.DonanteId == donante.Id);

            if (interes == null) return NotFound();

            if (interes.Estado == EstadoInteres.Confirmado)
            {
                TempData["Error"] = "El solicitante ya confirmó tu ayuda. Contáctalo directamente si necesitas cancelar.";
                return RedirectToAction(nameof(Panel));
            }

            _context.InteresesDonacion.Remove(interes);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cancelaste tu interés en esta solicitud.";
            return RedirectToAction(nameof(Panel));
        }
    }
}
