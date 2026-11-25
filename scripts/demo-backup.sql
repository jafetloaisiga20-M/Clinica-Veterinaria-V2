/* ============================================================
   DEMO DE RESPALDOS - Clinica Veterinaria Salud Animal
   Ejecutar en SSMS conectado a la instancia local (.).
   Cada bloque se puede ejecutar con F5 por separado.

   NOTA: ajusta las rutas C:\Users\yeske\... y MSSQL17.MSSQLSERVER
   a las de tu maquina antes de ejecutar.
   ============================================================ */

USE master;
GO

/* ------------------------------------------------------------
   1) RESPALDO COMPLETO (.bak)
   - INIT: sobrescribe el archivo si ya existe.
   - COMPRESSION: reduce el tamano del archivo.
   - CHECKSUM: agrega suma de verificacion al respaldo.
   - STATS = 10: muestra avance cada 10%.
   ------------------------------------------------------------ */
BACKUP DATABASE [Veterinaria_SaludAnimal]
TO DISK = N'C:\Users\yeske\Documents\GitHub\Clinica-Veterinaria-V2\backups\Veterinaria_SaludAnimal_FULL.bak'
WITH INIT, COMPRESSION, CHECKSUM, STATS = 10;
GO

/* ------------------------------------------------------------
   2) VERIFICAR EL RESPALDO SIN RESTAURARLO
   Solo lectura: valida que el archivo este integro.
   ------------------------------------------------------------ */
RESTORE VERIFYONLY
FROM DISK = N'C:\Users\yeske\Documents\GitHub\Clinica-Veterinaria-V2\backups\Veterinaria_SaludAnimal_FULL.bak'
WITH CHECKSUM;
GO

/* ------------------------------------------------------------
   3) VER LA CABECERA Y LOS ARCHIVOS DENTRO DEL .bak
   ------------------------------------------------------------ */
RESTORE HEADERONLY
FROM DISK = N'C:\Users\yeske\Documents\GitHub\Clinica-Veterinaria-V2\backups\Veterinaria_SaludAnimal_FULL.bak';
GO

RESTORE FILELISTONLY
FROM DISK = N'C:\Users\yeske\Documents\GitHub\Clinica-Veterinaria-V2\backups\Veterinaria_SaludAnimal_FULL.bak';
GO

/* ------------------------------------------------------------
   4) HISTORIAL DE RESPALDOS (tabla del sistema msdb)
   ------------------------------------------------------------ */
SELECT
    bs.database_name            AS Base,
    CASE bs.type
        WHEN 'D' THEN 'Completo'
        WHEN 'I' THEN 'Diferencial'
        WHEN 'L' THEN 'Log'
        ELSE bs.type
    END                         AS Tipo,
    bs.backup_start_date        AS Inicio,
    bs.backup_finish_date       AS Fin,
    CAST(bs.backup_size / 1024.0 / 1024.0 AS DECIMAL(10, 2)) AS TamanoMB,
    CAST(bs.compressed_backup_size / 1024.0 / 1024.0 AS DECIMAL(10, 2)) AS ComprimidoMB,
    CASE WHEN bs.compression_algorithm IS NOT NULL THEN 'Si' ELSE 'No' END AS Comprimido,
    bs.compression_algorithm    AS Algoritmo,
    mf.physical_device_name     AS Archivo
FROM msdb.dbo.backupset AS bs
INNER JOIN msdb.dbo.backupmediafamily AS mf
    ON bs.media_set_id = mf.media_set_id
WHERE bs.database_name = N'Veterinaria_SaludAnimal'
ORDER BY bs.backup_finish_date DESC;
GO

/* ------------------------------------------------------------
   5) RESTAURAR EN UNA BASE DE PRUEBA
   - MOVE cambia la ruta fisica para no tocar la base original.
   - REPLACE sobrescribe la base de prueba si ya existia.
   - RECOVERY deja la base lista para usar.
   ------------------------------------------------------------ */
IF DB_ID(N'Veterinaria_SaludAnimal_RestoreTest') IS NOT NULL
BEGIN
    ALTER DATABASE [Veterinaria_SaludAnimal_RestoreTest]
        SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
END
GO

RESTORE DATABASE [Veterinaria_SaludAnimal_RestoreTest]
FROM DISK = N'C:\Users\yeske\Documents\GitHub\Clinica-Veterinaria-V2\backups\Veterinaria_SaludAnimal_FULL.bak'
WITH
    MOVE N'Veterinaria_SaludAnimal'
        TO N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\Veterinaria_SaludAnimal_RestoreTest.mdf',
    MOVE N'Veterinaria_SaludAnimal_log'
        TO N'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\Veterinaria_SaludAnimal_RestoreTest_log.ldf',
    REPLACE,
    RECOVERY,
    STATS = 10;
GO

ALTER DATABASE [Veterinaria_SaludAnimal_RestoreTest] SET MULTI_USER;
GO

/* ------------------------------------------------------------
   6) COMPARAR LA BASE ORIGINAL CONTRA LA RESTAURADA
   ------------------------------------------------------------ */
SELECT
    'Original' AS Base,
    (SELECT COUNT(*) FROM Veterinaria_SaludAnimal.dbo.CLIENTE)    AS Clientes,
    (SELECT COUNT(*) FROM Veterinaria_SaludAnimal.dbo.PACIENTE)   AS Pacientes,
    (SELECT COUNT(*) FROM Veterinaria_SaludAnimal.dbo.CITA)       AS Citas,
    (SELECT COUNT(*) FROM Veterinaria_SaludAnimal.dbo.SERVICIO)   AS Servicios
UNION ALL
SELECT
    'Restaurada',
    (SELECT COUNT(*) FROM Veterinaria_SaludAnimal_RestoreTest.dbo.CLIENTE),
    (SELECT COUNT(*) FROM Veterinaria_SaludAnimal_RestoreTest.dbo.PACIENTE),
    (SELECT COUNT(*) FROM Veterinaria_SaludAnimal_RestoreTest.dbo.CITA),
    (SELECT COUNT(*) FROM Veterinaria_SaludAnimal_RestoreTest.dbo.SERVICIO);
GO

/* ------------------------------------------------------------
   7) LIMPIEZA OPCIONAL DE LA BASE DE PRUEBA
   Ejecutar solo cuando ya no se necesite revisar.
   ------------------------------------------------------------ */
-- ALTER DATABASE [Veterinaria_SaludAnimal_RestoreTest]
--     SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
-- DROP DATABASE [Veterinaria_SaludAnimal_RestoreTest];
-- GO
