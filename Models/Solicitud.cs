using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonaAhora.Models
{
    public class Solicitud
    {
        public int Id { get; set; }

        [Required]
        public int SolicitanteId { get; set; }

        [ForeignKey(nameof(SolicitanteId))]
        public Usuario? Solicitante { get; set; }

        [Required(ErrorMessage = "El tipo de sangre es obligatorio")]
        [Display(Name = "Tipo de sangre")]
        public TipoSangre TipoSangre { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, 20, ErrorMessage = "La cantidad debe estar entre 1 y 20 unidades")]
        [Display(Name = "Cantidad (unidades)")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        [StringLength(80)]
        public string Ciudad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El hospital es obligatorio")]
        [StringLength(150)]
        public string Hospital { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha necesaria")]
        public DateTime Fecha { get; set; } = DateTime.Now.AddDays(3);

        [Required]
        [Display(Name = "Nivel de urgencia")]
        public NivelUrgencia Urgencia { get; set; }

        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required]
        public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Activa;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public ICollection<InteresDonacion> Intereses { get; set; } = new List<InteresDonacion>();
    }
}
