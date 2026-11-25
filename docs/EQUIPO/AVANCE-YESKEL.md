# Avance de infraestructura - Yeskel

**Fecha:** 23/11/2026
**Para:** Alfredo, Jean, Angeles y Julie
**De:** Yeskel (infraestructura y DevOps)

## Resumen

Ya deje preparada toda la parte de infraestructura para que la aplicacion se
pueda levantar y desplegar:

1. **Base de datos completa** (13 tablas, 36 procedimientos almacenados y datos
   de ejemplo) que antes no estaba en el repositorio.
2. **API lista para la nube**: cadena de conexion y CORS por variables de
   entorno, health check y Dockerfile.
3. **Stack completo con Docker** para que QA pruebe sin instalar SQL Server.
4. **CI/CD en GitHub Actions**: compilacion automatica, deploy a Azure y
   respaldo diario de la base.
5. **Scripts** de inicializacion, backup, restauracion y reset.
6. **Documentacion**: `docs/INFRAESTRUCTURA.md` y `database/README.md`.

Lo unico que falta de mi lado es crear la cuenta en la nube y publicar las URLs
reales. Mientras tanto, todo se puede probar en local con Docker.

## Como probarlo ya

### Opcion rapida (Docker)

```powershell
copy .env.example .env
docker compose up --build
```

- Frontend: http://localhost:8080
- API: http://localhost:8081
- Health check: http://localhost:8081/health

La base se crea y se llena sola la primera vez.

### Opcion sin Docker

```powershell
# 1. Base de datos (requiere sqlcmd y SQL Server local)
.\scripts\init-db.ps1 -Server ".\SQLEXPRESS" -WindowsAuth

# 2. API
dotnet run --project PRESENTATION\PRESENTATION.csproj

# 3. Frontend
cd Frontend
copy .env.example .env
npm install
npm run dev
```

## Que cambio

| Archivo / carpeta | Cambio |
| --- | --- |
| `database/` | Esquema, 36 procedimientos almacenados y datos de ejemplo. |
| `scripts/` | `init-db.ps1`, `reset-db.ps1`, `backup-db.ps1`, `restore-db.ps1`, `smoke-test.ps1`. |
| `Dockerfile`, `.dockerignore`, `docker-entrypoint.sh` | Imagen de la API .NET 10. |
| `Frontend/Dockerfile`, `Frontend/nginx.conf` | Frontend compilado y servido con nginx. |
| `docker-compose.yml`, `.env.example` | Stack completo para QA. |
| `.github/workflows/ci.yml` | Compila backend, frontend e imagen Docker en cada push/PR. |
| `.github/workflows/deploy-api.yml` | Deploy manual a Azure App Service. |
| `.github/workflows/backup-db.yml` | Backup diario de la base como artefacto. |
| `PRESENTATION/Program.cs` | CORS configurable, health check y soporte de proxy. |
| `PRESENTATION/appsettings.json` | Nueva seccion `Cors`. |
| `Frontend/src/App.jsx` | La URL de la API ya no esta hardcodeada: usa `VITE_API_URL`. |
| `README.md`, `docs/` | Documentacion para todo el equipo. |

## Bugs que encontre y corregi

Estos errores hacian que la API devolviera datos incorrectos o fallara al
listar. Los corregi porque bloqueaban las pruebas de QA:

| Archivo | Problema | Correccion |
| --- | --- | --- |
| `CitasRepository.ListarCitasAsync` | Leia `ID_VETERIANRIO` y `ESTADP_CITA` (typos). | Ahora lee `ID_VETERINARIO` y `ESTADO_CITA`. |
| `CitasRepository.EditarCitas` | Enviaba `ID_CITAS` en todos los parametros. | Envia el valor correcto de cada campo. |
| `PacienteRepository.ListarPacienteAsync` | `FECHA_NACIMIENTO` y `PESO_ACTUAL` se leian de `ID_CLIENTE`. | Lee las columnas correctas. |
| `FacturaRepository.ListarFacturaAsync` | Casi todas las columnas se leian de `ID_CLIENTE`. | Mapeo corregido a cada columna real. |
| `TurnoGuardiaRepository` | `ID_TURNO` se leia de `ID_CLIENTE`; al insertar enviaba parametros de cliente. | Corregido a `ID_TURNO` y a `@ID_VETERINARIO`, `@FECHA`, `@HORA_INICIO`, `@HORA_FIN`. |
| `ServicioRepository.ListarServicioAsync` | Leia `DESCRIPCION` y convertia el precio a entero. | Lee `DESCRIPCION_DE_SERVICIO` y usa `decimal`. |

## Pendientes de backend (para Jean)

No los toque porque son parte de tu area:

1. **Rutas con espacio**: `PacienteController` y `CitasController` usan
   `[HttpPost("Nueva cita")]`. En la URL queda `/Nueva%20cita`. Conviene
   cambiarlas a `NuevaCita` / `NuevoPaciente`.
2. **Nombres en JSON**: ASP.NET convierte `ID_CLIENTE` a `iD_CLIENTE` y
   `NOMBRE_COMPLETO` a `nombrE_COMPLETO`. Se puede fijar
   `PropertyNamingPolicy = null` o acordar snake_case con Alfredo.
3. **Mensajes de error**: hay respuestas informales y una que expone
   `ex.Message` (`PacienteController.ListarPaciente`). Antes de publicar hay que
   dejar mensajes neutros y registrar el error con `ILogger`.
4. **Validaciones**: los DTO no tienen atributos (`[Required]`, `[EmailAddress]`,
   etc.), asi que `ModelState` casi no valida nada.
5. **Funciones sin implementar**: crear veterinario, editar/eliminar enmienda,
   editar/eliminar detalle de factura y eliminar paciente siguen lanzando
   `NotImplementedException`.
6. **Autenticacion**: no existe. Los usuarios del seed tienen
   `PASSWORD_HASH = 'PENDIENTE_DE_HASH'`; no usar esas cuentas en produccion.
7. **Revision de los SP**: los procedimientos de `database/02_stored_procedures.sql`
   los deduje de los repositorios. Revisa las reglas de negocio (cancelar cita,
   anular factura, borrado logico) y dime si alguna debe cambiar.

## Pendientes de frontend (para Alfredo)

1. La URL de la API ahora se configura con `VITE_API_URL` (ejemplo en
   `Frontend/.env.example`). En local: `http://localhost:5000/api`; en Docker:
   `/api`.
2. Probar el flujo de clientes contra la API nueva y avisar si falta algun
   endpoint.
3. Manejar errores de red en la UI (hoy solo hace `console.error`).
4. Cuando tenga la URL de QA, se la paso para que la ponga en Netlify/Vercel.

## Pruebas para Angeles y Julie (QA)

Cuando este la URL de QA se las envio. Mientras tanto pueden usar Docker.
**Antes de cada ronda de pruebas** me avisan o ejecutan el workflow manual
**Backup base de datos** en GitHub Actions, para tener un punto de restauracion
por si se borra algo. En Azure ademas existe point-in-time restore.

Casos minimos:

| # | Caso | Pasos | Resultado esperado |
| --- | --- | --- | --- |
| 1 | Salud de la API | GET `/health` | 200, `status: ok`, `database: ok` |
| 2 | Listar clientes | GET `/api/Cliente` | 3 clientes de ejemplo |
| 3 | Crear cliente | POST `/api/Cliente/NuevoCliente` con nombre, telefono y email | 201 y aparece en el listado |
| 4 | Listar pacientes | GET `/api/Paciente` | 3 mascotas de ejemplo |
| 5 | Crear paciente | POST `/api/Paciente/Nueva%20cita` con `ID_CLIENTE` existente | 201 y aparece en el listado |
| 6 | Listar citas | GET `/api/Citas` | 1 cita de ejemplo |
| 7 | Crear cita | POST `/api/Citas/Nueva%20cita` | 201 y aparece en el listado |
| 8 | Reiniciar base | `docker compose down -v` y `docker compose up --build` | Vuelve a los datos de ejemplo |

Datos de ejemplo: clientes `Juan Perez`, `Ana Martinez`, `Luis Gomez`;
mascotas `Firulais`, `Michi`, `Luna`.

Para reportar un bug necesito: endpoint, cuerpo enviado, respuesta obtenida y
hora aproximada. Los logs de la API ayudan a ubicarlo.

## Lo que falta de mi lado

1. Crear la cuenta de hosting (Azure recomendado) y la base en la nube.
2. Cargar el esquema con `scripts/init-db.ps1`.
3. Publicar la API y el frontend; compartir las URLs.
4. Configurar `DB_CONNECTION_STRING` y `DB_BACKUP_ENABLED` en GitHub.
5. Ejecutar un backup real y probar una restauracion.
6. Configurar UptimeRobot para monitorear `/health`.

## Mensaje corto para el grupo

> Hola equipo, ya avance la parte de infraestructura. Deje en el repo:
> - Base de datos completa (tablas, procedimientos y datos de ejemplo) en `database/`.
> - Stack con Docker para levantar todo con `docker compose up --build` (frontend en :8080, API en :8081).
> - CI/CD en GitHub Actions, deploy a Azure y backup automatico diario.
> - Documentacion en `docs/INFRAESTRUCTURA.md` y `database/README.md`.
>
> Tambien corregi varios mapeos de los repositorios que hacian que las listas devolvieran datos mal (Citas, Pacientes, Facturas, Turnos y Servicios). Jean, revisa la seccion "Pendientes de backend" del documento `docs/EQUIPO/AVANCE-YESKEL.md`.
>
> Me falta crear la cuenta en la nube y publicar las URLs. Mientras tanto pueden probar en local con Docker. Cualquier bug de QA me lo mandan con el endpoint y el cuerpo que usaron.
