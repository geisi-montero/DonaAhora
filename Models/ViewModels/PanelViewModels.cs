namespace DonaAhora.Models.ViewModels
{
    public class HomeViewModel
    {
        public int TotalDonantes { get; set; }
        public int TotalSolicitudesActivas { get; set; }
        public int TotalVidasSalvadas { get; set; }
        public int TotalCiudades { get; set; }
        public List<Solicitud> SolicitudesRecientes { get; set; } = new();
    }

    public class DonantePanelViewModel
    {
        public Donante Donante { get; set; } = null!;
        public List<Solicitud> SolicitudesCompatibles { get; set; } = new();
        public List<InteresDonacion> MisIntereses { get; set; } = new();
    }

    public class SolicitanteDashboardViewModel
    {
        public List<Solicitud> MisSolicitudes { get; set; } = new();
    }

    public class AdminDashboardViewModel
    {
        public int TotalUsuarios { get; set; }
        public int TotalDonantes { get; set; }
        public int TotalSolicitantes { get; set; }
        public int TotalSolicitudes { get; set; }
        public int TotalSolicitudesActivas { get; set; }
        public int TotalIntereses { get; set; }
        public List<Usuario> UltimosUsuarios { get; set; } = new();
        public List<Solicitud> UltimasSolicitudes { get; set; } = new();
    }

    public class SolicitudesFiltroViewModel
    {
        public List<Solicitud> Solicitudes { get; set; } = new();
        public TipoSangre? FiltroTipoSangre { get; set; }
        public string? FiltroCiudad { get; set; }
        public List<string> Ciudades { get; set; } = new();
    }
}
