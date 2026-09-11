namespace DonaAhora.Models
{
    public static class TipoSangreExtensions
    {
        public static string ToDisplay(this TipoSangre tipo)
        {
            return tipo switch
            {
                TipoSangre.OPositivo => "O+",
                TipoSangre.ONegativo => "O-",
                TipoSangre.APositivo => "A+",
                TipoSangre.ANegativo => "A-",
                TipoSangre.BPositivo => "B+",
                TipoSangre.BNegativo => "B-",
                TipoSangre.ABPositivo => "AB+",
                TipoSangre.ABNegativo => "AB-",
                _ => tipo.ToString()
            };
        }

        // Tipos que pueden donar a un receptor dado
        public static List<TipoSangre> DonantesCompatibles(this TipoSangre receptor)
        {
            return receptor switch
            {
                TipoSangre.OPositivo => new() { TipoSangre.OPositivo, TipoSangre.ONegativo },
                TipoSangre.ONegativo => new() { TipoSangre.ONegativo },
                TipoSangre.APositivo => new() { TipoSangre.APositivo, TipoSangre.ANegativo, TipoSangre.OPositivo, TipoSangre.ONegativo },
                TipoSangre.ANegativo => new() { TipoSangre.ANegativo, TipoSangre.ONegativo },
                TipoSangre.BPositivo => new() { TipoSangre.BPositivo, TipoSangre.BNegativo, TipoSangre.OPositivo, TipoSangre.ONegativo },
                TipoSangre.BNegativo => new() { TipoSangre.BNegativo, TipoSangre.ONegativo },
                TipoSangre.ABPositivo => new() { TipoSangre.ABPositivo, TipoSangre.ABNegativo, TipoSangre.APositivo, TipoSangre.ANegativo, TipoSangre.BPositivo, TipoSangre.BNegativo, TipoSangre.OPositivo, TipoSangre.ONegativo },
                TipoSangre.ABNegativo => new() { TipoSangre.ABNegativo, TipoSangre.ANegativo, TipoSangre.BNegativo, TipoSangre.ONegativo },
                _ => new()
            };
        }
    }
}
