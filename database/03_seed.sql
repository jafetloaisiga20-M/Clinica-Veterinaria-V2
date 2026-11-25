/*
    Datos iniciales para desarrollo y QA.
    Es idempotente: si los datos ya existen no los duplica.
    Todos los textos van sin acentos para que el script funcione con
    cualquier configuracion regional de sqlcmd.
*/
USE Veterinaria_SaludAnimal;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
GO

/* ---------------- Roles ---------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.ROL WHERE NOMBRE_ROL = N'Administrador')
    INSERT INTO dbo.ROL (NOMBRE_ROL) VALUES (N'Administrador');
IF NOT EXISTS (SELECT 1 FROM dbo.ROL WHERE NOMBRE_ROL = N'Veterinario')
    INSERT INTO dbo.ROL (NOMBRE_ROL) VALUES (N'Veterinario');
IF NOT EXISTS (SELECT 1 FROM dbo.ROL WHERE NOMBRE_ROL = N'Recepcion')
    INSERT INTO dbo.ROL (NOMBRE_ROL) VALUES (N'Recepcion');
IF NOT EXISTS (SELECT 1 FROM dbo.ROL WHERE NOMBRE_ROL = N'Cliente')
    INSERT INTO dbo.ROL (NOMBRE_ROL) VALUES (N'Cliente');
GO

/* ---------------- Sucursales ---------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.SUCURSAL WHERE NOMBRE_SUCURSAL = N'Sucursal Central')
BEGIN
    INSERT INTO dbo.SUCURSAL (NOMBRE_SUCURSAL, DIRECCION, CIUDAD, TELEFONO, ESTADO)
    VALUES (N'Sucursal Central', N'Km 5 Carretera Norte', N'Managua', N'2222-3333', N'Activa');
END

IF NOT EXISTS (SELECT 1 FROM dbo.SUCURSAL WHERE NOMBRE_SUCURSAL = N'Sucursal Carretera Sur')
BEGIN
    INSERT INTO dbo.SUCURSAL (NOMBRE_SUCURSAL, DIRECCION, CIUDAD, TELEFONO, ESTADO)
    VALUES (N'Sucursal Carretera Sur', N'Km 9 Carretera Sur', N'Managua', N'2222-4444', N'Activa');
END
GO

/* ---------------- Servicios ---------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.SERVICIO WHERE NOMBRE_SERVICIO = N'Consulta general')
    INSERT INTO dbo.SERVICIO (NOMBRE_SERVICIO, CATEGORIA, DESCRIPCION_DE_SERVICIO, PRECIO)
    VALUES (N'Consulta general', N'Consulta', N'Evaluacion clinica general de la mascota', 350.00);

IF NOT EXISTS (SELECT 1 FROM dbo.SERVICIO WHERE NOMBRE_SERVICIO = N'Vacunacion')
    INSERT INTO dbo.SERVICIO (NOMBRE_SERVICIO, CATEGORIA, DESCRIPCION_DE_SERVICIO, PRECIO)
    VALUES (N'Vacunacion', N'Preventivo', N'Aplicacion de vacuna segun calendario', 500.00);

IF NOT EXISTS (SELECT 1 FROM dbo.SERVICIO WHERE NOMBRE_SERVICIO = N'Desparasitacion')
    INSERT INTO dbo.SERVICIO (NOMBRE_SERVICIO, CATEGORIA, DESCRIPCION_DE_SERVICIO, PRECIO)
    VALUES (N'Desparasitacion', N'Preventivo', N'Tratamiento antiparasitario', 250.00);

IF NOT EXISTS (SELECT 1 FROM dbo.SERVICIO WHERE NOMBRE_SERVICIO = N'Bano y corte')
    INSERT INTO dbo.SERVICIO (NOMBRE_SERVICIO, CATEGORIA, DESCRIPCION_DE_SERVICIO, PRECIO)
    VALUES (N'Bano y corte', N'Estetica', N'Bano, secado y corte de pelo', 300.00);

IF NOT EXISTS (SELECT 1 FROM dbo.SERVICIO WHERE NOMBRE_SERVICIO = N'Cirugia menor')
    INSERT INTO dbo.SERVICIO (NOMBRE_SERVICIO, CATEGORIA, DESCRIPCION_DE_SERVICIO, PRECIO)
    VALUES (N'Cirugia menor', N'Cirugia', N'Procedimiento quirurgico ambulatorio', 2500.00);

IF NOT EXISTS (SELECT 1 FROM dbo.SERVICIO WHERE NOMBRE_SERVICIO = N'Rayos X')
    INSERT INTO dbo.SERVICIO (NOMBRE_SERVICIO, CATEGORIA, DESCRIPCION_DE_SERVICIO, PRECIO)
    VALUES (N'Rayos X', N'Diagnostico', N'Estudio radiografico simple', 1200.00);
GO

/* ---------------- Veterinarios ---------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.VETERINARIO WHERE NOMBRE_COMPLETO = N'Dra. Maria Lopez')
BEGIN
    INSERT INTO dbo.VETERINARIO (ID_SUCURSAL, NOMBRE_COMPLETO, ESPECIALIDAD, CV_DESCRIPCION, FOTO_URL, TELEFONO, ESTADO)
    VALUES (
        (SELECT TOP 1 ID_SUCURSAL FROM dbo.SUCURSAL WHERE NOMBRE_SUCURSAL = N'Sucursal Central'),
        N'Dra. Maria Lopez', N'Medicina general',
        N'Egresada de la UNAM, 8 anos de experiencia en clinica de pequenos animales.',
        N'https://placehold.co/300x300?text=Veterinaria', N'8888-1111', N'Activo');
END

IF NOT EXISTS (SELECT 1 FROM dbo.VETERINARIO WHERE NOMBRE_COMPLETO = N'Dr. Carlos Ruiz')
BEGIN
    INSERT INTO dbo.VETERINARIO (ID_SUCURSAL, NOMBRE_COMPLETO, ESPECIALIDAD, CV_DESCRIPCION, FOTO_URL, TELEFONO, ESTADO)
    VALUES (
        (SELECT TOP 1 ID_SUCURSAL FROM dbo.SUCURSAL WHERE NOMBRE_SUCURSAL = N'Sucursal Central'),
        N'Dr. Carlos Ruiz', N'Cirugia',
        N'Especialista en cirugia de tejidos blandos.',
        N'https://placehold.co/300x300?text=Veterinaria', N'8888-2222', N'Activo');
END
GO

/* ---------------- Clientes ---------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.CLIENTE WHERE EMAIL = N'juan.perez@example.com')
BEGIN
    INSERT INTO dbo.CLIENTE (NOMBRE_COMPLETO, TELEFONO, WHATSAPP, EMAIL, DIRECCION, ESTADO)
    VALUES (N'Juan Perez', N'7777-1001', N'7777-1001', N'juan.perez@example.com', N'Barrio Los Robles', N'Activo');
END

IF NOT EXISTS (SELECT 1 FROM dbo.CLIENTE WHERE EMAIL = N'ana.martinez@example.com')
BEGIN
    INSERT INTO dbo.CLIENTE (NOMBRE_COMPLETO, TELEFONO, WHATSAPP, EMAIL, DIRECCION, ESTADO)
    VALUES (N'Ana Martinez', N'7777-1002', N'7777-1002', N'ana.martinez@example.com', N'Colonia Centroamericana', N'Activo');
END

IF NOT EXISTS (SELECT 1 FROM dbo.CLIENTE WHERE EMAIL = N'luis.gomez@example.com')
BEGIN
    INSERT INTO dbo.CLIENTE (NOMBRE_COMPLETO, TELEFONO, WHATSAPP, EMAIL, DIRECCION, ESTADO)
    VALUES (N'Luis Gomez', N'7777-1003', N'7777-1003', N'luis.gomez@example.com', N'Reparto San Juan', N'Activo');
END
GO

/* ---------------- Pacientes (mascotas) ---------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.PACIENTE WHERE NOMBRE = N'Firulais'
               AND ID_CLIENTE = (SELECT TOP 1 ID_CLIENTE FROM dbo.CLIENTE WHERE EMAIL = N'juan.perez@example.com'))
BEGIN
    INSERT INTO dbo.PACIENTE (ID_CLIENTE, NOMBRE, ESPECIE, RAZA, SEXO, FECHA_NACIMIENTO, PESO_ACTUAL, ESTADO)
    VALUES (
        (SELECT TOP 1 ID_CLIENTE FROM dbo.CLIENTE WHERE EMAIL = N'juan.perez@example.com'),
        N'Firulais', N'Perro', N'Labrador', N'Macho', '2021-05-14', 28.50, N'Activo');
END

IF NOT EXISTS (SELECT 1 FROM dbo.PACIENTE WHERE NOMBRE = N'Michi'
               AND ID_CLIENTE = (SELECT TOP 1 ID_CLIENTE FROM dbo.CLIENTE WHERE EMAIL = N'ana.martinez@example.com'))
BEGIN
    INSERT INTO dbo.PACIENTE (ID_CLIENTE, NOMBRE, ESPECIE, RAZA, SEXO, FECHA_NACIMIENTO, PESO_ACTUAL, ESTADO)
    VALUES (
        (SELECT TOP 1 ID_CLIENTE FROM dbo.CLIENTE WHERE EMAIL = N'ana.martinez@example.com'),
        N'Michi', N'Gato', N'Siames', N'Hembra', '2022-11-02', 4.20, N'Activo');
END

IF NOT EXISTS (SELECT 1 FROM dbo.PACIENTE WHERE NOMBRE = N'Luna'
               AND ID_CLIENTE = (SELECT TOP 1 ID_CLIENTE FROM dbo.CLIENTE WHERE EMAIL = N'luis.gomez@example.com'))
BEGIN
    INSERT INTO dbo.PACIENTE (ID_CLIENTE, NOMBRE, ESPECIE, RAZA, SEXO, FECHA_NACIMIENTO, PESO_ACTUAL, ESTADO)
    VALUES (
        (SELECT TOP 1 ID_CLIENTE FROM dbo.CLIENTE WHERE EMAIL = N'luis.gomez@example.com'),
        N'Luna', N'Perro', N'Criollo', N'Hembra', '2020-08-21', 16.00, N'Activo');
END
GO

/* ---------------- Usuarios de prueba ----------------
   PASSWORD_HASH es un marcador: el modulo de autenticacion
   todavia no esta implementado. No usar estas cuentas en produccion.
--------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.USUARIO WHERE NOMBRE_USUARIO = N'admin')
BEGIN
    INSERT INTO dbo.USUARIO (ID_ROL, ID_SUCURSAL, NOMBRE_USUARIO, PASSWORD_HASH, ESTADO)
    VALUES (
        (SELECT TOP 1 ID_ROL FROM dbo.ROL WHERE NOMBRE_ROL = N'Administrador'),
        (SELECT TOP 1 ID_SUCURSAL FROM dbo.SUCURSAL WHERE NOMBRE_SUCURSAL = N'Sucursal Central'),
        N'admin', N'$2a$11$U/WMV0ri.AuhD0q4MqL2auqlSWIk1EVIsoA6mgQIierRIP8V6n2Ha', N'Activo');
END

IF NOT EXISTS (SELECT 1 FROM dbo.USUARIO WHERE NOMBRE_USUARIO = N'recepcion')
BEGIN
    INSERT INTO dbo.USUARIO (ID_ROL, ID_SUCURSAL, NOMBRE_USUARIO, PASSWORD_HASH, ESTADO)
    VALUES (
        (SELECT TOP 1 ID_ROL FROM dbo.ROL WHERE NOMBRE_ROL = N'Recepcion'),
        (SELECT TOP 1 ID_SUCURSAL FROM dbo.SUCURSAL WHERE NOMBRE_SUCURSAL = N'Sucursal Central'),
        N'recepcion', N'$2a$11$L0NKmw7FQadu7aL9e59pU.yguLb6cSZn5FAgU25tSsIXDQTc1tTha', N'Activo');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.USUARIO WHERE NOMBRE_USUARIO = N'veterinario')
BEGIN
    INSERT INTO dbo.USUARIO (ID_ROL, ID_SUCURSAL, NOMBRE_USUARIO, PASSWORD_HASH, ESTADO)
    VALUES (
        (SELECT TOP 1 ID_ROL FROM dbo.ROL WHERE NOMBRE_ROL = N'Veterinario'),
        (SELECT TOP 1 ID_SUCURSAL FROM dbo.SUCURSAL WHERE NOMBRE_SUCURSAL = N'Sucursal Central'),
        N'veterinario', N'$2a$11$vZVs9Sl83Y4t9NlZWaK3aO6ZVxmAthj.ewT62PzxMo7kT06oVdn6m', N'Activo');
END
GO

/* ---------------- Una cita y una guardia de ejemplo ---------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.CITA)
BEGIN
    INSERT INTO dbo.CITA (ID_PACIENTE, ID_SUCURSAL, ID_VETERINARIO, FECHA_HORA, ORIGEN_CONTACTO, ESTADO_CITA)
    VALUES (
        (SELECT TOP 1 ID_PACIENTE FROM dbo.PACIENTE ORDER BY ID_PACIENTE),
        (SELECT TOP 1 ID_SUCURSAL FROM dbo.SUCURSAL ORDER BY ID_SUCURSAL),
        (SELECT TOP 1 ID_VETERINARIO FROM dbo.VETERINARIO ORDER BY ID_VETERINARIO),
        DATEADD(HOUR, 9, CAST(DATEADD(DAY, 1, CAST(SYSDATETIME() AS DATE)) AS DATETIME2(0))),
        N'WhatsApp', N'Programada');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TURNO_GUARDIA)
BEGIN
    INSERT INTO dbo.TURNO_GUARDIA (ID_VETERINARIO, FECHA, HORA_INICIO, HORA_FIN, ESTADO)
    VALUES (
        (SELECT TOP 1 ID_VETERINARIO FROM dbo.VETERINARIO ORDER BY ID_VETERINARIO),
        DATEADD(DAY, 1, CAST(SYSDATETIME() AS DATE)),
        CAST('18:00' AS TIME), CAST('22:00' AS TIME), N'Activo');
END
GO

PRINT N'Datos de ejemplo aplicados correctamente.';
GO
