# Infraestructura y despliegue

> Responsable: **Yeskel** (infraestructura y DevOps)
> Ultima actualizacion: 23/11/2026
>
> Este documento explica donde vive (o va a vivir) la aplicacion, como se
> despliega y como se respalda la base de datos.

## 1. Arquitectura

```
                    Internet
                       |
        +--------------+---------------+
        |                              |
  Frontend React                 API ASP.NET Core
  (Netlify/Vercel)               (.NET 10, App Service o contenedor)
        |                              |
        |  HTTPS /api/*                | ADO.NET
        +----------------------------->|
                                       |
                                 SQL Server / Azure SQL
                                 (Veterinaria_SaludAnimal)
                                       |
                                 GitHub Actions
                                 - CI (build + docker)
                                 - Deploy de la API
                                 - Backup diario BACPAC
```

- **Frontend**: React + Vite, se compila a archivos estaticos.
- **Backend**: ASP.NET Core .NET 10. Expone `/api/*` y usa procedimientos
  almacenados de SQL Server.
- **Base de datos**: SQL Server. En local puede ser SQLEXPRESS; en la nube,
  Azure SQL Database (compatible).
- **CI/CD**: GitHub Actions (`.github/workflows/`).
- **Health check**: `GET /health` valida API + base de datos.

## 2. Donde puede vivir la app

### Opcion recomendada: Azure (cuenta de estudiante)

| Componente | Servicio | Costo |
| --- | --- | --- |
| API | Azure App Service, plan **F1 (free)** | $0 |
| Base de datos | Azure SQL Database **free tier** (1 base por suscripcion) | $0 |
| Frontend | Netlify, Vercel o Cloudflare Pages (plan free) | $0 |
| CI/CD y backups | GitHub Actions (repo del equipo) | $0 |

Ventajas: SQL Server administrado, firewall, respaldos automaticos
(point-in-time restore), integracion directa con GitHub Actions y el mismo
motor que ya usa el proyecto.

Limitaciones:

- App Service F1 tiene 60 minutos de CPU al dia y se enfría si no hay trafico.
- Azure SQL free se auto-pausa; la primera conexion puede tardar unos segundos.
- Verifica los limites vigentes al crear los recursos, cambian con el tiempo.

La cuenta **Azure for Students** incluye credito sin tarjeta si Yeskel tiene
correo institucional. Es la via mas rapida para tener URLs publicas.

### Alternativa sin Azure: MonsterASP.NET

Hosting gratuito para ASP.NET Core que incluye una base SQL Server.

- Ventaja: todo en un panel, sin tarjeta.
- Desventajas: menos espacio, el despliegue es por Web Deploy/FTP (no hay
  integracion directa con GitHub Actions), y las conexiones remotas a la base
  suelen estar restringidas, lo que complica el backup automatico.

Si se elige esta opcion, los backups se descargan manualmente desde el panel o
se usa el mismo `scripts/backup-db.ps1` desde una IP permitida.

### Alternativa en contenedor: Render + Azure SQL

- La API se despliega con el `Dockerfile` del repositorio.
- Render free duerme el servicio tras unos minutos sin trafico (cold start).
- La base sigue en Azure SQL.

Sirve si Yeskel prefiere no usar App Service, pero no resuelve por si sola la
base de datos.

### Comparacion rapida

| Criterio | Azure | MonsterASP | Render + Azure SQL |
| --- | --- | --- | --- |
| Costo | $0 | $0 | $0 |
| SQL Server incluido | Si (Azure SQL) | Si | No (solo la API) |
| Deploy desde GitHub | Si | Manual | Si |
| Backup automatico | Si (workflow + PITR) | Manual | Si (workflow) |
| Se duerme por inactividad | F1 si | No | Si |

**Decision pendiente de Yeskel:** crear la cuenta y confirmar cual opcion usa.
Los archivos del repositorio ya funcionan con cualquiera de las tres.

## 3. Ambientes

| Ambiente | Frontend | API | Base de datos |
| --- | --- | --- | --- |
| Local (Docker) | http://localhost:8080 | http://localhost:8081 | contenedor `db` |
| Local (sin Docker) | http://localhost:5173 | http://localhost:5000 | SQL Server local |
| QA | _pendiente de URL_ | _pendiente de URL_ | Azure SQL |
| Produccion | _pendiente de URL_ | _pendiente de URL_ | Azure SQL |

Cuando existan las URLs, actualizar esta tabla y compartirlas con Angeles y
Julie para las pruebas.

## 4. Variables de entorno

### API

| Variable | Ejemplo | Obligatoria | Descripcion |
| --- | --- | --- | --- |
| `ConnectionStrings__default` | `Server=tcp:xxx.database.windows.net,1433;Database=Veterinaria_SaludAnimal;User ID=app;Password=***;Encrypt=True;` | Si | Cadena de conexion. Nunca se sube al repo. |
| `Cors__AllowedOrigins` | `https://clinica.netlify.app,http://localhost:5173` | Si en produccion | Origenes permitidos, separados por coma. |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Recomendada | Entorno de ejecucion. |
| `PORT` | `8080` | No | Render la inyecta; el entrypoint la respeta. |

### Frontend

| Variable | Ejemplo | Donde se configura |
| --- | --- | --- |
| `VITE_API_URL` | `https://api-clinica.azurewebsites.net/api` | Archivo `.env` local o variables del hosting (Netlify/Vercel). |

Importante: no incluir barra final y si incluir `/api`.

### GitHub Actions (secrets y variables)

| Nombre | Tipo | Uso |
| --- | --- | --- |
| `DB_CONNECTION_STRING` | Secret | Backup diario de la base. |
| `AZURE_WEBAPP_NAME` | Secret | Nombre del App Service. |
| `AZURE_WEBAPP_PUBLISH_PROFILE` | Secret | Contenido del publish profile de Azure. |
| `DB_BACKUP_ENABLED` | Variable | `true` para activar el cron del backup. |
| `API_HEALTH_URL` | Variable | URL de `/health` para verificar el deploy. |

## 5. Puesta en marcha en Azure (paso a paso)

### 5.1 Crear la base de datos

1. Entrar a [portal.azure.com](https://portal.azure.com).
2. **Create a resource -> SQL Database -> Create**.
3. En *Compute + storage* elegir la oferta gratuita si esta disponible.
4. Crear un servidor con usuario administrador y contrasena fuerte.
5. En **Networking**:
   - Agregar la IP publica de Yeskel (para cargar el esquema).
   - Activar **Allow Azure services and resources to access this server**
     (para que GitHub Actions haga el backup).
6. Copiar la cadena de conexion (incluye `Encrypt=True`).

### 5.2 Cargar el esquema

Desde la maquina de Yeskel, con `sqlcmd` y `sqlpackage` instalados:

```powershell
.\scripts\init-db.ps1 `
  -Server "miservidor.database.windows.net" `
  -User "sqladmin" `
  -Password "MiClaveFuerte!"
```

Esto crea tablas, procedimientos y datos de ejemplo. Es idempotente.

### 5.3 Publicar la API

1. Crear un **App Service** Linux con runtime .NET 10 y plan **F1**.
2. En **Configuration -> Environment variables** agregar:
   - `ConnectionStrings__default`
   - `Cors__AllowedOrigins` (URL del frontend; se puede ajustar despues)
   - `ASPNETCORE_ENVIRONMENT=Production`
3. En **Configuration -> General settings** activar **HTTPS Only = On** (redirige
   todo el trafico HTTP a HTTPS). La API incluye `UseHsts()` en produccion.
4. Descargar el **publish profile** del App Service.
5. En GitHub: **Settings -> Secrets and variables -> Actions**
   - `AZURE_WEBAPP_NAME`
   - `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Variable `API_HEALTH_URL=https://tu-app.azurewebsites.net/health`
6. Ejecutar el workflow **Deploy API (Azure App Service)** desde la pestana
   Actions.
7. Probar la API con el smoke test:

```powershell
.\scripts\smoke-test.ps1 -BaseUrl "https://tu-app.azurewebsites.net"
```

Debe terminar con `Todo responde correctamente.`

Alternativa sin App Service: desplegar el contenedor del `Dockerfile` en
Render/Railway y configurar las mismas variables.

### 5.4 Publicar el frontend

En Netlify (o Vercel):

1. Importar el repositorio.
2. Base directory: `Frontend`.
3. Build command: `npm run build`.
4. Publish directory: `Frontend/dist` (Netlify lo detecta como `dist`).
5. Variable de entorno: `VITE_API_URL=https://tu-app.azurewebsites.net/api`.
6. Cuando Netlify asigne la URL, agregarla a `Cors__AllowedOrigins` de la API.

### 5.5 Activar backups automaticos

1. GitHub -> **Settings -> Secrets and variables -> Actions**:
   - Secret `DB_CONNECTION_STRING` con la cadena de Azure SQL.
   - Variable `DB_BACKUP_ENABLED=true`.
2. El workflow **Backup base de datos** corre todos los dias a las 07:00 UTC
   (01:00 en Nicaragua).
3. Tambien se puede ejecutar manualmente desde **Actions -> Backup base de
   datos -> Run workflow**.
4. El `.bacpac` queda como artefacto por 90 dias. Descargarlo y guardar una
   copia en OneDrive/Drive si se necesita mas tiempo.
5. Cada cierto tiempo, probar una restauracion (ver seccion 6).

Ademas del workflow, Azure SQL tiene **point-in-time restore** integrado
(configurable en el portal). Es la primera opcion para recuperar algo borrado
por error hace pocas horas; el BACPAC es el respaldo externo.

**Antes de cada ronda de QA** ejecutar el workflow de backup manualmente
(Actions -> Backup base de datos -> Run workflow). Asi hay un punto de
restauracion previo a las pruebas, ademas del backup diario.

### 5.6 Monitoreo

- Liveness frecuente (cada 5 minutos): `GET /health/live`. No toca la base de
  datos, por lo que no despierta la auto-pausa de Azure SQL free.
- Readiness (cada 30-60 minutos o antes de QA): `GET /health`. Valida la
  conexion a la base de datos.
- Configurar ambos con [UptimeRobot](https://uptimerobot.com/) (plan free) o
  similar.
- Logs en vivo de App Service: **Log stream** o
  `az webapp log tail --name <app> --resource-group <rg>`.
- Alertas de Azure SQL para CPU/almacenamiento si se acerca al limite free.

### 5.7 Integracion con redes sociales (opcional)

La API expone endpoints para recibir leads de WhatsApp y Facebook:

- Verificacion (WhatsApp): `GET /api/MensajeSocial/webhooks/whatsapp?hub.mode=subscribe&hub.verify_token=<token>&hub.challenge=<challenge>`
- Recepcion WhatsApp: `POST /api/MensajeSocial/webhooks/whatsapp`
- Recepcion Facebook: `POST /api/MensajeSocial/webhooks/facebook`

Secrets/variables a configurar en App Service (y en GitHub si se automatiza):

- `Social__WhatsApp__VerifyToken`
- `Social__WhatsApp__AppSecret`
- `Social__Facebook__AppSecret`

Pasos en Meta:

1. Crear app en Meta for Developers y agregar WhatsApp Business / Messenger.
2. Configurar el webhook con la URL publica de la API y el verify token.
3. Suscribirse a los campos `messages`.
4. Enviar un mensaje de prueba a la pagina/numero y verificar que aparece en
   `GET /api/MensajeSocial` o en la pestana "Mensajes" del frontend.

Los mensajes se guardan en `dbo.MENSAJE_SOCIAL` para que recepcion los atienda
y los convierta en citas.

## 6. Runbook

### Desplegar una nueva version de la API

1. Hacer merge a `master`.
2. Ejecutar el workflow **Deploy API** (manual) o configurarlo para que corra
   en cada push cuando el equipo este listo.
3. Verificar `/health`.

### Revertir un deploy

1. `git revert` del commit que rompio algo (o volver a la version anterior).
2. Ejecutar de nuevo **Deploy API**.
3. Si el problema es la base, restaurar el ultimo BACPAC en una base nueva y
   cambiar `ConnectionStrings__default`.

### Reiniciar la base de QA

Solo si Angeles y Julie ya terminaron una ronda de pruebas:

```powershell
.\scripts\reset-db.ps1 -Server "miservidor.database.windows.net" -User "sqladmin" -Password "***"
```

Para QA en Docker: `docker compose down -v` y `docker compose up --build`.

### Restaurar un respaldo

1. Descargar el `.bacpac` desde GitHub Actions.
2. Crear una base **nueva** (no sobrescribir la actual), por ejemplo
   `Veterinaria_Restore`.
3. Ejecutar:

```powershell
.\scripts\restore-db.ps1 `
  -BacpacFile ".\backups\backup-20261123-070000.bacpac" `
  -TargetConnectionString "Server=tcp:xxx.database.windows.net,1433;Database=Veterinaria_Restore;User ID=sqladmin;Password=***;Encrypt=True;"
```

4. Verificar clientes, mascotas, citas y facturas.
5. Si todo esta bien, apuntar la API a la base restaurada.

> **Nota (Windows local):** `sqlpackage` 170+ requiere .NET 10.0.15 o
> superior. Si la maquina solo tiene .NET 10.0.12, instalar la version
> 162.5.57:
>
> ```powershell
> dotnet tool install --global microsoft.sqlpackage --version 162.5.57
> ```
>
> Los scripts `backup-db.ps1` y `restore-db.ps1` ya activan
> `DOTNET_ROLL_FORWARD=Major` para que esa version corra sobre .NET 10.

### Respaldo nativo `.bak` (Windows)

Ademas del BACPAC, SQL Server puede generar una copia completa del motor:

```sql
BACKUP DATABASE [Veterinaria_SaludAnimal]
TO DISK = N'C:\ruta\Veterinaria_SaludAnimal_FULL.bak'
WITH INIT, COMPRESSION, CHECKSUM, STATS = 10;

RESTORE VERIFYONLY
FROM DISK = N'C:\ruta\Veterinaria_SaludAnimal_FULL.bak'
WITH CHECKSUM;
```

`scripts/demo-backup.sql` contiene el flujo completo: respaldo, verificacion,
historial (`msdb.dbo.backupset`), restauracion en
`Veterinaria_SaludAnimal_RestoreTest` y comparacion de conteos. Es el script
recomendado para la demostracion en SSMS.

### Rotar la contrasena de la base

1. Cambiar la contrasena en Azure SQL.
2. Actualizar `ConnectionStrings__default` en App Service.
3. Actualizar el secret `DB_CONNECTION_STRING` en GitHub.
4. Reiniciar la API.

## 7. Seguridad

- **Nunca** subir `.env`, cadenas de conexion ni publish profiles al repo. Ya
  estan en `.gitignore`; usar secrets de GitHub.
- La cadena de conexion de produccion debe usar un usuario de aplicacion con
  permisos minimos, no el administrador del servidor:

```sql
-- Ejecutar en master (Azure SQL)
CREATE LOGIN vet_app WITH PASSWORD = 'ClaveFuerte!';
-- Ejecutar en Veterinaria_SaludAnimal
CREATE USER vet_app FOR LOGIN vet_app;
ALTER ROLE db_datareader ADD MEMBER vet_app;
ALTER ROLE db_datawriter ADD MEMBER vet_app;
GRANT EXECUTE ON SCHEMA::dbo TO vet_app;
```

- El firewall de Azure SQL debe permitir solo la IP de Yeskel y los servicios
  de Azure necesarios.
- Los BACPAC contienen datos de clientes. No compartirlos por canales publicos;
  guardarlos en almacenamiento privado y borrar los locales cuando ya no se
  necesiten.
- El seed tiene usuarios con `PASSWORD_HASH = 'PENDIENTE_DE_HASH'`: no usar
  esas cuentas en produccion hasta que Jean implemente autenticacion.

## 8. Checklist de entrega de infraestructura

- [x] Proyecto compila (.NET 10).
- [x] Scripts de base de datos probados en SQL Server local.
- [x] Procedimientos almacenados probados (listar, insertar, facturar, anular).
- [x] Dockerfile de la API y del frontend.
- [x] `docker-compose.yml` con SQL Server, API y frontend.
- [x] Health check `/health`.
- [x] CORS configurable por variable de entorno.
- [x] CI de GitHub Actions (backend, frontend y Docker).
- [x] Workflow de deploy a Azure (manual).
- [x] Workflow de backup diario BACPAC.
- [x] Scripts de backup/restauracion/reset.
- [ ] Crear cuenta de hosting y base en la nube.
- [ ] Cargar la base en la nube.
- [ ] Publicar API y frontend.
- [ ] Configurar secret `DB_CONNECTION_STRING` y probar un backup real.
- [ ] Probar una restauracion real.
- [ ] Compartir URLs con QA (Angeles y Julie).
- [ ] Configurar monitoreo con UptimeRobot.

## 9. Problemas comunes

| Sintoma | Causa probable | Solucion |
| --- | --- | --- |
| `/health` devuelve 503 | Cadena de conexion mala o firewall | Revisar `ConnectionStrings__default` y la IP permitida en Azure SQL. |
| El navegador muestra error de CORS | `Cors__AllowedOrigins` no incluye la URL del frontend | Agregar la URL exacta (sin barra final) y reiniciar la API. |
| La API no arranca en Azure | Falta una variable de entorno | Revisar **Log stream**; la app falla rapido si no hay cadena de conexion. |
| `Invalid object name 'dbo.CLIENTE'` | No se ejecutaron los scripts | Ejecutar `scripts/init-db.ps1`. |
| `Could not find stored procedure 'SP_...'` | Falta `02_stored_procedures.sql` | Ejecutar el script completo. |
| El backup falla por firewall | GitHub Actions no esta permitido | Activar **Allow Azure services** en Azure SQL. |
| Azure SQL tarda en responder | Auto-pausa del free tier | Es normal la primera conexion; el health check la despierta. |

## 10. Decisiones pendientes

1. Confirmar hosting (Azure recomendado) y crear la cuenta.
2. Definir URLs definitivas de QA y produccion.
3. Decidir si el dominio sera gratis (`*.azurewebsites.net`, `*.netlify.app`)
   o se compra uno.
4. Confirmar retencion de backups (90 dias en GitHub; Azure PITR aparte).
5. Coordinar con Jean la revision de los procedimientos y reglas de negocio.
