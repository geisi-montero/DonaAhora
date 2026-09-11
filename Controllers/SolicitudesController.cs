using DonaAhora.Data;
using DonaAhora.Models;
using DonaAhora.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonaAhora.Controllers
{
    public class SolicitudesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SolicitudesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(TipoSangre? tipoSangre, string? ciudad)
        {
            var query = _context.Solicitudes
                .Include(s => s.Solicitante)
                .Where(s => s.Estado == EstadoSolicitud.Activa)
                .AsQueryable();

            if (tipoSangre.HasValue)
                query = query.Where(s => s.TipoSangre == tipoSangre.Value);

            if (!string.IsNullOrWhiteSpace(ciudad))
                query = query.Where(s => s.Ciudad == ciudad);

            var solicitudes = await query
                .OrderByDescending(s => s.Urgencia)
                .ThenByDescending(s => s.FechaCreacion)
                .ToListAsync();

            var ciudades = await _context.Solicitudes
                .Select(s => s.Ciudad)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var vm = new SolicitudesFiltroViewModel
            {
                Solicitudes = solicitudes,
                FiltroTipoSangre = tipoSangre,
                FiltroCiudad = ciudad,
                Ciudades = ciudades
            };

            return View(vm);
        }
    }
}
