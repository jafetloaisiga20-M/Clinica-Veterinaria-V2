# Base de datos

Base de datos SQL Server `Veterinaria_SaludAnimal`. La API usa ADO.NET y
**procedimientos almacenados**, por eso no basta con crear las tablas: hay que
ejecutar tambien `02_stored_procedures.sql`.

## Archivos

| Archivo | Que hace |
| --- | --- |
| `00_create_database.sql` | Crea la base si no existe (ejecutar en `master`). |
| `01_schema.sql` | Crea tablas, llaves foraneas, checks e indices. |
| `02_stored_procedures.sql` | Crea/actualiza los `SP_*` que llaman los repositorios. |
| `03_seed.sql` | Datos de ejemplo: roles, sucursales, servicios, veterinarios, clientes, mascotas, usuarios y una cita. |

Los scripts son **idempotentes**: se pueden volver a ejecutar sin duplicar datos
ni borrar informacion.

## Opcion 1: script automatico (recomendado)

```powershell
# SQL Server local con autenticacion de Windows
.\scripts\init-db.ps1 -Server ".\SQLEXPRESS" -WindowsAuth

# SQL Server / Azure SQL con usuario y contrasena
.\scripts\init-db.ps1 -Server "miservidor.database.windows.net" -User "sqladmin" -Password "MiClave!"
```

Requiere `sqlcmd` instalado. En Windows viene con SQL Server Management Studio o
con [Microsoft SQL Server Command Line Utilities](https://learn.microsoft.com/sql/tools/sqlcmd/sqlcmd-utility).

## Opcion 2: Docker

```powershell
copy .env.example .env
docker compose up --build
```

El servicio `db-init` ejecuta los cuatro scripts automaticamente.

## Opcion 3: manual con SSMS

Abre y ejecuta en orden:

1. `00_create_database.sql` conectado a `master`.
2. `01_schema.sql`, `02_stored_procedures.sql` y `03_seed.sql` conectado a
   `Veterinaria_SaludAnimal`.

## Reiniciar la base (borra todo)

Solo para local o QA. Nunca contra la base compartida sin avisar:

```powershell
.\scripts\reset-db.ps1 -Server ".\SQLEXPRESS" -WindowsAuth
```

## Respaldo y restauracion

```powershell
# Respaldo (BACPAC)
.\scripts\backup-db.ps1 -ConnectionString "Server=.;Database=Veterinaria_SaludAnimal;Integrated Security=True;TrustServerCertificate=True"

# Restauracion en una base NUEVA
.\scripts\restore-db.ps1 -BacpacFile ".\backups\Veterinaria_SaludAnimal-20261123-120000.bacpac" `
  -TargetConnectionString "Server=.;Database=Veterinaria_Restore;Integrated Security=True;TrustServerCertificate=True"
```

Requiere `sqlpackage`:

```powershell
dotnet tool install --global microsoft.sqlpackage
```

## Relacion con el codigo

Los nombres de columnas y parametros se dedujeron de:

- `DOMAIN/Entities/*.cs` (propiedades de las entidades).
- `INFRASTRUCTURE/Repository/*.cs` (columnas leidas y parametros enviados).

Reglas de negocio implementadas en los SP:

- **Borrado logico** de clientes y veterinarios (`ESTADO = 'Inactivo'`) para no
  romper citas, facturas ni expedientes historicos.
- **Cancelar cita** cambia `ESTADO_CITA` a `Cancelada`.
- **Anular factura** cambia `ESTADO_FACTURA` a `Anulada` y bloquea agregar lineas.
- **Detalle de factura** toma el precio actual del servicio y calcula el subtotal.
- **Numero de factura** se genera como `FAC-AAAAMMDD-0001`.

Si Jean tiene el script original de la base local, conviene compararlo con este
y conservar el que tenga las reglas definitivas. Este esquema es la base
funcional para desplegar y para que QA empiece a probar.
