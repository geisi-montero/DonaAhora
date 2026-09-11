
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'DonaAhoraDB')
BEGIN
    ALTER DATABASE DonaAhoraDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE DonaAhoraDB;
END
GO

CREATE DATABASE DonaAhoraDB;
GO

USE DonaAhoraDB;
GO

CREATE TABLE Usuarios (
    Id            INT IDENTITY(1,1)   NOT NULL,
    Nombre        NVARCHAR(100)       NOT NULL,
    Email         NVARCHAR(150)       NOT NULL,
    PasswordHash  NVARCHAR(MAX)       NOT NULL,
    Rol           NVARCHAR(20)        NOT NULL,
    Telefono      NVARCHAR(20)        NULL,
    FechaRegistro DATETIME2           NOT NULL DEFAULT (SYSDATETIME()),
    Activo        BIT                 NOT NULL DEFAULT (1),
    CONSTRAINT PK_Usuarios PRIMARY KEY (Id),
    CONSTRAINT UQ_Usuarios_Email UNIQUE (Email),
    CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN (N'Donante', N'Solicitante', N'Administrador'))
);
GO

CREATE TABLE Donantes (
    Id               INT IDENTITY(1,1) NOT NULL,
    UsuarioId        INT               NOT NULL,
    TipoSangre       NVARCHAR(10)      NOT NULL,
    Ciudad           NVARCHAR(80)      NOT NULL,
    Disponible       BIT               NOT NULL DEFAULT (1),
    UltimaDonacion   DATETIME2         NULL,
    TotalDonaciones  INT               NOT NULL DEFAULT (0),
    CONSTRAINT PK_Donantes PRIMARY KEY (Id),
    CONSTRAINT UQ_Donantes_UsuarioId UNIQUE (UsuarioId),
    CONSTRAINT FK_Donantes_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Donantes_TipoSangre CHECK (TipoSangre IN
        (N'OPositivo', N'ONegativo', N'APositivo', N'ANegativo',
         N'BPositivo', N'BNegativo', N'ABPositivo', N'ABNegativo'))
);
GO

CREATE TABLE Solicitudes (
    Id             INT IDENTITY(1,1)  NOT NULL,
    SolicitanteId  INT                NOT NULL,
    TipoSangre     NVARCHAR(10)       NOT NULL,
    Cantidad       INT                NOT NULL,
    Ciudad         NVARCHAR(80)       NOT NULL,
    Hospital       NVARCHAR(150)      NOT NULL,
    Fecha          DATETIME2          NOT NULL,
    Urgencia       NVARCHAR(15)       NOT NULL,
    Descripcion    NVARCHAR(500)      NULL,
    Estado         NVARCHAR(15)       NOT NULL DEFAULT (N'Activa'),
    FechaCreacion  DATETIME2          NOT NULL DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_Solicitudes PRIMARY KEY (Id),
    CONSTRAINT FK_Solicitudes_Usuarios FOREIGN KEY (SolicitanteId) REFERENCES Usuarios(Id),
    CONSTRAINT CK_Solicitudes_TipoSangre CHECK (TipoSangre IN
        (N'OPositivo', N'ONegativo', N'APositivo', N'ANegativo',
         N'BPositivo', N'BNegativo', N'ABPositivo', N'ABNegativo')),
    CONSTRAINT CK_Solicitudes_Cantidad CHECK (Cantidad BETWEEN 1 AND 20),
    CONSTRAINT CK_Solicitudes_Urgencia CHECK (Urgencia IN (N'Baja', N'Media', N'Alta', N'Critica')),
    CONSTRAINT CK_Solicitudes_Estado CHECK (Estado IN (N'Activa', N'EnProceso', N'Completada', N'Cancelada'))
);
GO

CREATE TABLE InteresesDonacion (
    Id            INT IDENTITY(1,1) NOT NULL,
    DonanteId     INT               NOT NULL,
    SolicitudId   INT               NOT NULL,
    FechaInteres  DATETIME2         NOT NULL DEFAULT (SYSDATETIME()),
    Estado        NVARCHAR(15)      NOT NULL DEFAULT (N'Pendiente'),
    CONSTRAINT PK_InteresesDonacion PRIMARY KEY (Id),
    CONSTRAINT FK_Intereses_Donantes FOREIGN KEY (DonanteId) REFERENCES Donantes(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Intereses_Solicitudes FOREIGN KEY (SolicitudId) REFERENCES Solicitudes(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Intereses_Donante_Solicitud UNIQUE (DonanteId, SolicitudId),
    CONSTRAINT CK_Intereses_Estado CHECK (Estado IN (N'Pendiente', N'Confirmado', N'Cancelado'))
);
GO

CREATE INDEX IX_Solicitudes_Estado ON Solicitudes(Estado);
CREATE INDEX IX_Solicitudes_TipoSangre ON Solicitudes(TipoSangre);
CREATE INDEX IX_Solicitudes_Ciudad ON Solicitudes(Ciudad);
CREATE INDEX IX_Donantes_TipoSangre ON Donantes(TipoSangre);
GO

-- Todos los usuarios de prueba usan la contraseña: 123456
DECLARE @PwdHash NVARCHAR(MAX) = N'ghIUm5WSziB8t4+bRHZ79oPMD9Bk2aXQE9miaUkKbSk=';

INSERT INTO Usuarios (Nombre, Email, PasswordHash, Rol, Telefono, FechaRegistro, Activo) VALUES
(N'Administrador General',   N'admin@donaahora.com',      @PwdHash, N'Administrador', N'809-555-0001', DATEADD(DAY, -120, SYSDATETIME()), 1),
-- Donantes
(N'Carlos Martínez',         N'carlos.martinez@mail.com', @PwdHash, N'Donante',       N'809-555-0101', DATEADD(DAY, -90, SYSDATETIME()), 1),
(N'María Fernández',         N'maria.fernandez@mail.com', @PwdHash, N'Donante',       N'809-555-0102', DATEADD(DAY, -85, SYSDATETIME()), 1),
(N'José Ramírez',            N'jose.ramirez@mail.com',    @PwdHash, N'Donante',       N'809-555-0103', DATEADD(DAY, -80, SYSDATETIME()), 1),
(N'Ana López',               N'ana.lopez@mail.com',       @PwdHash, N'Donante',       N'809-555-0104', DATEADD(DAY, -75, SYSDATETIME()), 1),
(N'Luis Peña',               N'luis.pena@mail.com',       @PwdHash, N'Donante',       N'809-555-0105', DATEADD(DAY, -70, SYSDATETIME()), 1),
(N'Carmen Rodríguez',        N'carmen.rodriguez@mail.com',@PwdHash, N'Donante',       N'809-555-0106', DATEADD(DAY, -65, SYSDATETIME()), 1),
(N'Pedro Gómez',             N'pedro.gomez@mail.com',     @PwdHash, N'Donante',       N'809-555-0107', DATEADD(DAY, -60, SYSDATETIME()), 1),
(N'Rosa Jiménez',            N'rosa.jimenez@mail.com',    @PwdHash, N'Donante',       N'809-555-0108', DATEADD(DAY, -55, SYSDATETIME()), 1),
-- Solicitantes
(N'Hospital Plaza de la Salud (Rep.)', N'solicitante1@mail.com', @PwdHash, N'Solicitante', N'809-555-0201', DATEADD(DAY, -50, SYSDATETIME()), 1),
(N'Elena Castillo',          N'elena.castillo@mail.com',  @PwdHash, N'Solicitante',   N'809-555-0202', DATEADD(DAY, -40, SYSDATETIME()), 1),
(N'Rafael Ortiz',            N'rafael.ortiz@mail.com',    @PwdHash, N'Solicitante',   N'809-555-0203', DATEADD(DAY, -30, SYSDATETIME()), 1),
(N'Cruz Roja Santiago',      N'cruzroja.santiago@mail.com',@PwdHash, N'Solicitante',  N'809-555-0204', DATEADD(DAY, -20, SYSDATETIME()), 1);
GO

INSERT INTO Donantes (UsuarioId, TipoSangre, Ciudad, Disponible, UltimaDonacion, TotalDonaciones)
SELECT Id, N'OPositivo', N'Santo Domingo', 1, DATEADD(DAY, -100, SYSDATETIME()), 4 FROM Usuarios WHERE Email = N'carlos.martinez@mail.com'
UNION ALL SELECT Id, N'ONegativo', N'Santiago', 1, DATEADD(DAY, -200, SYSDATETIME()), 6 FROM Usuarios WHERE Email = N'maria.fernandez@mail.com'
UNION ALL SELECT Id, N'APositivo', N'Santo Domingo', 1, NULL, 0 FROM Usuarios WHERE Email = N'jose.ramirez@mail.com'
UNION ALL SELECT Id, N'ANegativo', N'La Vega', 0, DATEADD(DAY, -30, SYSDATETIME()), 2 FROM Usuarios WHERE Email = N'ana.lopez@mail.com'
UNION ALL SELECT Id, N'BPositivo', N'Santiago', 1, DATEADD(DAY, -150, SYSDATETIME()), 3 FROM Usuarios WHERE Email = N'luis.pena@mail.com'
UNION ALL SELECT Id, N'BNegativo', N'Puerto Plata', 1, NULL, 0 FROM Usuarios WHERE Email = N'carmen.rodriguez@mail.com'
UNION ALL SELECT Id, N'ABPositivo', N'Santo Domingo', 1, DATEADD(DAY, -60, SYSDATETIME()), 1 FROM Usuarios WHERE Email = N'pedro.gomez@mail.com'
UNION ALL SELECT Id, N'ABNegativo', N'San Cristóbal', 1, NULL, 0 FROM Usuarios WHERE Email = N'rosa.jimenez@mail.com';
GO

INSERT INTO Solicitudes (SolicitanteId, TipoSangre, Cantidad, Ciudad, Hospital, Fecha, Urgencia, Descripcion, Estado, FechaCreacion)
SELECT Id, N'OPositivo', 3, N'Santo Domingo', N'Hospital Plaza de la Salud', DATEADD(DAY, 2, SYSDATETIME()), N'Critica',
       N'Paciente en cirugía de emergencia requiere transfusión urgente.', N'Activa', DATEADD(HOUR, -5, SYSDATETIME())
FROM Usuarios WHERE Email = N'solicitante1@mail.com'
UNION ALL
SELECT Id, N'ANegativo', 2, N'Santiago', N'Hospital Regional José María Cabral', DATEADD(DAY, 5, SYSDATETIME()), N'Alta',
       N'Paciente con anemia severa necesita donación esta semana.', N'Activa', DATEADD(DAY, -1, SYSDATETIME())
FROM Usuarios WHERE Email = N'elena.castillo@mail.com'
UNION ALL
SELECT Id, N'OPositivo', 4, N'Santo Domingo', N'Hospital Dr. Darío Contreras', DATEADD(DAY, 1, SYSDATETIME()), N'Critica',
       N'Accidente de tránsito, se requieren varias unidades con urgencia.', N'Activa', DATEADD(HOUR, -2, SYSDATETIME())
FROM Usuarios WHERE Email = N'rafael.ortiz@mail.com'
UNION ALL
SELECT Id, N'BPositivo', 2, N'Santiago', N'Cruz Roja Dominicana - Santiago', DATEADD(DAY, 7, SYSDATETIME()), N'Media',
       N'Campaña de reposición de reservas para el banco de sangre.', N'Activa', DATEADD(DAY, -3, SYSDATETIME())
FROM Usuarios WHERE Email = N'cruzroja.santiago@mail.com'
UNION ALL
SELECT Id, N'ABPositivo', 1, N'Puerto Plata', N'Hospital Ricardo Limardo', DATEADD(DAY, 10, SYSDATETIME()), N'Baja',
       N'Cirugía programada, se necesita unidad de reserva.', N'Activa', DATEADD(DAY, -2, SYSDATETIME())
FROM Usuarios WHERE Email = N'solicitante1@mail.com'
UNION ALL
SELECT Id, N'ONegativo', 5, N'Santo Domingo', N'Hospital General Plaza de la Salud', DATEADD(DAY, 3, SYSDATETIME()), N'Critica',
       N'Paciente politraumatizado requiere donante universal con urgencia.', N'Activa', DATEADD(HOUR, -8, SYSDATETIME())
FROM Usuarios WHERE Email = N'elena.castillo@mail.com'
UNION ALL
SELECT Id, N'BNegativo', 2, N'San Cristóbal', N'Hospital Pedro Iglesias', DATEADD(DAY, -2, SYSDATETIME()), N'Alta',
       N'Solicitud ya cubierta, mantenida como referencia histórica.', N'Completada', DATEADD(DAY, -10, SYSDATETIME())
FROM Usuarios WHERE Email = N'rafael.ortiz@mail.com'
UNION ALL
SELECT Id, N'APositivo', 3, N'La Vega', N'Hospital Luis Morillo King', DATEADD(DAY, 4, SYSDATETIME()), N'Media',
       N'Paciente en tratamiento oncológico requiere apoyo periódico.', N'EnProceso', DATEADD(DAY, -4, SYSDATETIME())
FROM Usuarios WHERE Email = N'cruzroja.santiago@mail.com';
GO

INSERT INTO InteresesDonacion (DonanteId, SolicitudId, FechaInteres, Estado)
SELECT d.Id, s.Id, DATEADD(HOUR, -1, SYSDATETIME()), N'Pendiente'
FROM Donantes d
JOIN Usuarios u ON u.Id = d.UsuarioId AND u.Email = N'carlos.martinez@mail.com'
JOIN Solicitudes s ON s.TipoSangre = N'OPositivo' AND s.Ciudad = N'Santo Domingo' AND s.Urgencia = N'Critica'
WHERE s.Descripcion LIKE N'Paciente en cirugía%'
UNION ALL
SELECT d.Id, s.Id, DATEADD(HOUR, -3, SYSDATETIME()), N'Confirmado'
FROM Donantes d
JOIN Usuarios u ON u.Id = d.UsuarioId AND u.Email = N'maria.fernandez@mail.com'
JOIN Solicitudes s ON s.TipoSangre = N'ONegativo' AND s.Ciudad = N'Santo Domingo'
WHERE s.Descripcion LIKE N'Paciente politraumatizado%';
GO

PRINT N'Base de datos DonaAhoraDB creada correctamente con datos de prueba.';
PRINT N'Todas las cuentas de ejemplo usan la contraseña: 123456';
PRINT N'Cuenta administrador: admin@donaahora.com';
GO
