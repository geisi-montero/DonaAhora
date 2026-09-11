using DonaAhora.Data;
using DonaAhora.Models;
using DonaAhora.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonaAhora.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                TotalDonantes = await _context.Donantes.CountAsync(),
                TotalSolicitudesActivas = await _context.Solicitudes.CountAsync(s => s.Estado == EstadoSolicitud.Activa),
                TotalVidasSalvadas = await _context.Solicitudes.CountAsync(s => s.Estado == EstadoSolicitud.Completada) * 3,
                TotalCiudades = await _context.Donantes.Select(d => d.Ciudad).Distinct().CountAsync(),
                SolicitudesRecientes = await _context.Solicitudes
                    .Include(s => s.Solicitante)
                    .Where(s => s.Estado == EstadoSolicitud.Activa)
                    .OrderByDescending(s => s.Urgencia)
                    .ThenByDescending(s => s.FechaCreacion)
                    .Take(6)
                    .ToListAsync()
            };

            return View(vm);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}
