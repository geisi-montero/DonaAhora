using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonaAhora.Models
{
    // Interés de un donante en una solicitud (botón "Quiero ayudar")
    public class InteresDonacion
    {
        public int Id { get; set; }

        [Required]
        public int DonanteId { get; set; }

        [ForeignKey(nameof(DonanteId))]
        public Donante? Donante { get; set; }

        [Required]
        public int SolicitudId { get; set; }

        [ForeignKey(nameof(SolicitudId))]
        public Solicitud? Solicitud { get; set; }

        public DateTime FechaInteres { get; set; } = DateTime.Now;

        public EstadoInteres Estado { get; set; } = EstadoInteres.Pendiente;
    }
}
