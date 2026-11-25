# Guia para abrir el proyecto en otra PC

Esta guia deja el sistema completo funcionando en una computadora nueva con
**SQL Server local**, **API .NET** y **frontend React**.

Al final deben quedar arriba:

- API: `http://localhost:5000`
- Frontend: `http://localhost:5173`
- Base de datos: `Veterinaria_SaludAnimal`

> Tiempo estimado la primera vez: 30 a 45 minutos. La mayor parte es instalar
> SQL Server y .NET.

---

## Ruta rapida (si ya tienes todo instalado)

Abre **dos terminales** en la carpeta del proyecto.

**Terminal 1 - Base de datos y API:**

```powershell
# Cambia este valor por tu instancia: ".", ".\SQLEXPRESS" o ".\MIINSTANCIA"
$server = ".\SQLEXPRESS"
.\scripts\init-db.ps1 -Server $server -WindowsAuth

$env:ConnectionStrings__default = "Server=$server;Database=Veterinaria_SaludAnimal;Integrated Security=True;TrustServerCertificate=True"
$env:ASPNETCORE_ENVIRONMENT = "Development"
# 0.0.0.0 permite entrar desde el telefono en la misma Wi-Fi.
$env:ASPNETCORE_URLS = "http://0.0.0.0:5000"
dotnet run --project PRESENTATION\PRESENTATION.csproj --no-launch-profile
```

**Terminal 2 - Frontend:**

```powershell
cd Frontend
copy .env.example .env
npm ci
npm run dev
```

Abrir `http://localhost:5173` y entrar con:

- Usuario: `admin`
- Contrasena: `Admin123!`

---

## 1. Que hay que instalar

| Programa | Version | Para que |
| --- | --- | --- |
| Windows | 10 u 11, 64 bits | Sistema operativo |
| SQL Server | 2022 o superior (Express o Developer) | Base de datos |
| SSMS | 20, 21 o 22 | Administrar la base (recomendado) |
| .NET SDK | **10.0** | Compilar y ejecutar la API |
| Node.js | **22 LTS o 24** | Ejecutar el frontend |
| WinRAR o 7-Zip | Cualquiera | Extraer el RAR |

Descargas oficiales:

- SQL Server: <https://www.microsoft.com/sql-server/sql-server-downloads>
- SSMS: <https://aka.ms/ssmsfullsetup>
- .NET 10: <https://dotnet.microsoft.com/download/dotnet/10.0>
- Node.js: <https://nodejs.org/>

> El proyecto usa `net10.0`. Con .NET 8 o 9 no compila.

---

## 2. Extraer el proyecto

1. Copiar `Clinica-Veterinaria-V2.rar` a la PC nueva.
2. Extraerlo en una ruta corta y sin espacios, por ejemplo:

   ```text
   C:\ClinicaVeterinariaV2
   ```

3. Abrir PowerShell en esa carpeta:

   ```powershell
   cd C:\ClinicaVeterinariaV2
   ```

4. Si PowerShell bloquea los scripts:

   ```powershell
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
   ```

El RAR **no incluye** `bin`, `obj`, `node_modules`, `dist`, `backups` ni los
archivos generados de Android (`.gradle`, `build`). Eso es normal: las
dependencias se restauran con `dotnet` y `npm`. Si incluye el APK ya compilado
en `release\SaludAnimal.apk`.

---

## 3. Instalar y verificar SQL Server

1. Instalar SQL Server 2022 o superior (Express o Developer).
2. Elegir **Autenticacion de Windows**.
3. Anotar el nombre de la instancia. Ejemplos:

| Tipo de instancia | Nombre que se usa en `-Server` |
| --- | --- |
| Predeterminada | `.` o `localhost` |
| Express | `.\SQLEXPRESS` |
| Instancia con nombre | `.\NOMBREINSTANCIA` |
| Por nombre de PC | `NOMBRE-PC\SQLEXPRESS` |

4. Verificar que el servicio este iniciado. Abrir PowerShell **como
   administrador**:

   ```powershell
   Get-Service MSSQLSERVER -ErrorAction SilentlyContinue
   Get-Service 'MSSQL$SQLEXPRESS' -ErrorAction SilentlyContinue
   ```

   Si aparece `Stopped`, iniciarlo:

   ```powershell
   Start-Service MSSQLSERVER
   # o, si usas Express:
   Start-Service 'MSSQL$SQLEXPRESS'
   ```

5. Abrir SSMS y conectarse con **Autenticacion de Windows** al nombre de la
   instancia. Si no conecta, revisar el nombre de la instancia.

> Si no tienes SSMS, instala las Command Line Utilities para usar `sqlcmd`;
> si no tienes `sqlcmd`, instala SSMS. La seccion 4 explica ambas opciones.

---

## 4. Crear la base de datos

Desde PowerShell, en la carpeta del proyecto:

```powershell
.\scripts\init-db.ps1 -Server ".\SQLEXPRESS" -WindowsAuth
```

Cambiar `-Server` por el nombre real:

| Instancia | Comando |
| --- | --- |
| Predeterminada | `.\scripts\init-db.ps1 -Server "." -WindowsAuth` |
| Express | `.\scripts\init-db.ps1 -Server ".\SQLEXPRESS" -WindowsAuth` |
| Nombre personalizado | `.\scripts\init-db.ps1 -Server ".\MIINSTANCIA" -WindowsAuth` |

El script crea:

- Base `Veterinaria_SaludAnimal`
- 14 tablas
- 36 procedimientos almacenados
- Datos de ejemplo: 3 clientes, 3 mascotas, 1 cita, 2 veterinarios,
  6 servicios y 3 usuarios

Es seguro ejecutarlo varias veces: **no borra datos**.

### Si `sqlcmd` no esta instalado

Usar SSMS y ejecutar en este orden:

1. `database\00_create_database.sql` conectado a `master`.
2. `database\01_schema.sql` conectado a `Veterinaria_SaludAnimal`.
3. `database\02_stored_procedures.sql` conectado a `Veterinaria_SaludAnimal`.
4. `database\03_seed.sql` conectado a `Veterinaria_SaludAnimal`.

Verificar en SSMS:

```sql
USE Veterinaria_SaludAnimal;
SELECT COUNT(*) AS Clientes FROM dbo.CLIENTE;
```

En una base nueva deben aparecer **3 clientes**.

---

## 5. Configurar la conexion de la API

La API lee la cadena de `PRESENTATION/appsettings.json`. Por defecto apunta a
`.\SQLEXPRESS`.

Si tu instancia es distinta, **no hace falta editar el archivo**: define la
variable de entorno en la misma terminal donde vas a arrancar la API.

```powershell
# Cambia este valor por tu instancia: ".", ".\SQLEXPRESS" o ".\MIINSTANCIA"
$server = ".\SQLEXPRESS"
$env:ConnectionStrings__default = "Server=$server;Database=Veterinaria_SaludAnimal;Integrated Security=True;TrustServerCertificate=True"
```

> `TrustServerCertificate=True` es solo para desarrollo local. En produccion se
> debe usar un certificado valido.

Si prefieres dejarlo permanente, edita `PRESENTATION/appsettings.json` y cambia
`Data Source` por tu instancia. No subas ese cambio si contiene datos de tu PC.

---

## 6. Arrancar la API

En la misma terminal, con la variable de conexion ya definida:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://0.0.0.0:5000"
dotnet run --project PRESENTATION\PRESENTATION.csproj --no-launch-profile
```

Esperar el mensaje:

```text
Now listening on: http://0.0.0.0:5000
```

> `0.0.0.0` significa "todas las interfaces de red". En la PC se sigue entrando
> por `http://localhost:5000`; el telefono entra por `http://IP-DE-LA-PC:5000`.
> Si no se va a usar el APK, tambien sirve `http://localhost:5000`.

Probar en el navegador o en otra terminal:

```powershell
Invoke-RestMethod http://localhost:5000/health
```

Debe responder algo parecido a:

```json
{
  "status": "ok",
  "database": "ok",
  "environment": "Development"
}
```

Si `database` no es `ok`, la cadena de conexion o el nombre de la instancia
estan mal.

**Importante:** dejar esta terminal abierta. Si se cierra, la API se apaga.

---

## 7. Arrancar el frontend

Abrir una **segunda** terminal:

```powershell
cd C:\ClinicaVeterinariaV2\Frontend
copy .env.example .env
npm ci
npm run dev
```

Si `npm ci` falla, usar:

```powershell
npm install
```

Verificar que `Frontend\.env` tenga:

```text
VITE_API_URL=http://localhost:5000/api
VITE_WHATSAPP_NUMBER=50500000000
```

Abrir en el navegador:

```text
http://localhost:5173
```

> La primera vez, `dotnet` y `npm` necesitan internet para descargar
> dependencias.

---

## 8. Probar el sistema

1. **Sitio publico:** Inicio, Servicios, Agendar cita, Contacto.
2. **Agendar cita:** completa cliente, mascota y cita. No requiere login.
3. **Acceso interno:** boton en la pagina publica.
4. Usuarios de ejemplo:

| Rol | Usuario | Contrasena |
| --- | --- | --- |
| Administrador | `admin` | `Admin123!` |
| Recepcion | `recepcion` | `Recepc123!` |
| Veterinario | `veterinario` | `Vet123!` |

5. **Panel interno:** clientes, pacientes, citas, mensajes y expediente.
6. **Mensajes:** boton para simular un mensaje de WhatsApp.
7. **Expediente:** crear expediente y agregar enmiendas (no se edita ni se
   borra, es append-only).

Verificacion automatica de API + base:

```powershell
.\scripts\smoke-test.ps1 -BaseUrl "http://localhost:5000"
```

Debe terminar con:

```text
Todo responde correctamente.
```

---

## 9. Apagar todo

En cada terminal, presionar `Ctrl+C`.

Si algun puerto queda ocupado:

```powershell
$p = (Get-NetTCPConnection -State Listen -LocalPort 5000 -ErrorAction SilentlyContinue).OwningProcess
if ($p) { Stop-Process -Id $p -Force }

$p = (Get-NetTCPConnection -State Listen -LocalPort 5173 -ErrorAction SilentlyContinue).OwningProcess
if ($p) { Stop-Process -Id $p -Force }
```

SQL Server puede seguir corriendo. Para detenerlo (PowerShell como
administrador):

```powershell
Stop-Service MSSQLSERVER
# o
Stop-Service 'MSSQL$SQLEXPRESS'
```

Para volver a iniciarlo:

```powershell
Start-Service MSSQLSERVER
# o
Start-Service 'MSSQL$SQLEXPRESS'
```

---

## 10. Problemas comunes

| Problema | Causa probable | Solucion |
| --- | --- | --- |
| `dotnet` no se reconoce | Falta .NET 10 SDK | Instalar y reabrir PowerShell |
| `npm` no se reconoce | Falta Node.js | Instalar Node 22/24 y reabrir |
| `NETSDK1045` | SDK viejo | Instalar .NET 10 SDK |
| `sqlcmd no esta instalado` | Falta la utilidad | Instalar Command Line Utilities o usar SSMS (seccion 4) |
| `Cannot open database` | Base no creada o instancia incorrecta | Revisar `-Server` y ejecutar `init-db.ps1` |
| `Login failed for user` | Usuario de Windows sin permiso | Usar Autenticacion de Windows o revisar la instancia |
| `certificate chain ... not trusted` | Certificado local | Agregar `TrustServerCertificate=True` |
| `Failed to connect to localhost:5000` | API apagada o puerto distinto | Revisar la terminal de la API |
| Error de CORS | Origen no permitido | Usar `http://localhost:5173` o agregarlo en `Cors:AllowedOrigins` |
| 401 en el panel | Sin login o token vencido | Volver a iniciar sesion |
| Puerto 5000 o 5173 ocupado | Otro proceso | Cerrar el proceso o cambiar el puerto |
| `sqlpackage` pide .NET 10.0.15 | Version nueva de DacFx | Instalar `microsoft.sqlpackage --version 162.5.57` |

Si cambias el puerto de la API, actualiza tambien
`Frontend\.env` (`VITE_API_URL`) y reinicia el frontend.

---

## 11. Opcional: Docker

Si Docker Desktop funciona en la PC, se puede levantar todo sin instalar SQL
Server ni .NET:

```powershell
copy .env.example .env
docker compose up --build
```

- Frontend: `http://localhost:8080`
- API: `http://localhost:8081`
- SQL Server: `localhost,14330` (usuario `sa`, contrasena en `.env`)

Para borrar la base y empezar de cero:

```powershell
docker compose down -v
```

> En la maquina de desarrollo Docker Desktop no arranco, por eso la ruta
> principal de esta guia es la instalacion manual.

---

## 12. Opcional: backups

### BACPAC (recomendado por el proyecto)

```powershell
dotnet tool install --global microsoft.sqlpackage --version 162.5.57
.\scripts\backup-db.ps1 -ConnectionString "Server=.;Database=Veterinaria_SaludAnimal;Integrated Security=True;TrustServerCertificate=True"
```

El script genera un `.bacpac` y su `.sha256` en `backups\`.

### Respaldo nativo `.bak` y restauracion

1. Abrir `scripts\demo-backup.sql` en SSMS.
2. Ajustar las rutas `C:\Users\yeske\...` y `MSSQL17.MSSQLSERVER` a la PC nueva.
3. Ejecutar con `F5`.

El script hace respaldo, verificacion, historial, restauracion en una base de
prueba y comparacion de conteos.

Mas detalle en [`docs/INFRAESTRUCTURA.md`](docs/INFRAESTRUCTURA.md).

---

## 13. APK Android (opcional, misma Wi-Fi)

El RAR incluye `release\SaludAnimal.apk`; tambien esta en el Escritorio. Sirve
para usar el sistema desde un telefono Android en la misma Wi-Fi. No crea otra
base: habla con la API y SQL Server de la PC.

Requisitos:

- API con `ASPNETCORE_URLS=http://0.0.0.0:5000` (seccion 6).
- TCP 5000 permitido en el firewall para red privada.
- Telefono y PC en la misma Wi-Fi; no usar la red de invitados.

Pasos:

1. Obtener la IP de la PC con `ipconfig` (IPv4 de la conexion Wi-Fi).
2. Probar desde el navegador del telefono: `http://IP-DE-LA-PC:5000/health`.
3. Instalar `SaludAnimal.apk` (permitir instalar apps desconocidas).
4. Abrir la app; en el modal **Servidor de la clinica** escribir
   `http://IP-DE-LA-PC:5000/api`, tocar **Probar conexion** y luego
   **Guardar y recargar**.
5. Si el modal no aparece, usar el boton flotante **Servidor**.
6. Entrar con `admin` / `Admin123!`, crear un cliente o una cita y verificar
   que aparezca en la PC y en SQL Server.

Si la IP de la PC cambia, no hace falta recompilar: abrir **Servidor**, escribir
la nueva direccion, probar y guardar.

El APK compilado apunta inicialmente a `http://192.168.80.119:5000/api`.

Firewall (PowerShell como administrador):

```powershell
New-NetFirewallRule `
  -DisplayName "Salud Animal API 5000" `
  -Direction Inbound `
  -Protocol TCP `
  -LocalPort 5000 `
  -Action Allow `
  -Profile Private
```

Si la red esta marcada como Publica, cambiarla a Privada solo en una red de
confianza:

```powershell
Set-NetConnectionProfile -Name "SANDOVAL" -NetworkCategory Private
```

Guia completa (compilar, cambiar IP y problemas comunes):
[docs/EQUIPO/APK-ANDROID.md](docs/EQUIPO/APK-ANDROID.md).

> HTTP sin TLS es solo para la demostracion local. En produccion usar HTTPS,
> un APK release firmado y cambiar las claves y contrasenas de ejemplo.

---

## 14. Seguridad antes de publicar

- Cambiar `Jwt:Key` en `PRESENTATION/appsettings.json`.
- Cambiar las contrasenas de los usuarios de ejemplo.
- No compartir `.bak` ni `.bacpac`: contienen datos de clientes.
- No subir `.env`, cadenas de conexion ni secretos al repositorio.
- Usar HTTPS en produccion.

---

## Checklist final

- [ ] SQL Server corriendo
- [ ] Base `Veterinaria_SaludAnimal` creada
- [ ] API en `http://localhost:5000` con `/health` en `ok`
- [ ] Frontend en `http://localhost:5173`
- [ ] Login con `admin` / `Admin123!`
- [ ] Agendar una cita desde el sitio publico
- [ ] `smoke-test.ps1` termina en `Todo responde correctamente.`
- [ ] APK instalado en el telefono y con la IP correcta
- [ ] Un cliente/cita creado desde el telefono aparece en SQL Server
