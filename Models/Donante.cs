using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonaAhora.Models
{
    public class Donante
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }

        [Required(ErrorMessage = "El tipo de sangre es obligatorio")]
        [Display(Name = "Tipo de sangre")]
        public TipoSangre TipoSangre { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        [StringLength(80)]
        public string Ciudad { get; set; } = string.Empty;

        [Display(Name = "Disponible para donar")]
        public bool Disponible { get; set; } = true;

        public DateTime? UltimaDonacion { get; set; }

        public int TotalDonaciones { get; set; } = 0;

        public ICollection<InteresDonacion> Intereses { get; set; } = new List<InteresDonacion>();
    }
}
