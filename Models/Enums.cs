namespace DonaAhora.Models
{
    public enum RolUsuario
    {
        Donante,
        Solicitante,
        Administrador
    }

    public enum TipoSangre
    {
        [System.ComponentModel.Description("O+")] OPositivo,
        [System.ComponentModel.Description("O-")] ONegativo,
        [System.ComponentModel.Description("A+")] APositivo,
        [System.ComponentModel.Description("A-")] ANegativo,
        [System.ComponentModel.Description("B+")] BPositivo,
        [System.ComponentModel.Description("B-")] BNegativo,
        [System.ComponentModel.Description("AB+")] ABPositivo,
        [System.ComponentModel.Description("AB-")] ABNegativo
    }

    public enum NivelUrgencia
    {
        Baja,
        Media,
        Alta,
        Critica
    }

    public enum EstadoSolicitud
    {
        Activa,
        EnProceso,
        Completada,
        Cancelada
    }

    public enum EstadoInteres
    {
        Pendiente,
        Confirmado,
        Cancelado
    }
}
