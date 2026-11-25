/*
    Crea la base de datos si no existe.
    Ejecutar conectado a "master" (asi lo hace scripts/init-db.ps1).
*/
IF DB_ID(N'Veterinaria_SaludAnimal') IS NULL
BEGIN
    CREATE DATABASE Veterinaria_SaludAnimal;
END
GO
