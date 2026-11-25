# Clinica Veterinaria V2

Sistema de gestion para una clinica veterinaria. Backend en **ASP.NET Core
(.NET 10)** con arquitectura por capas y frontend en **React + Vite**.

## Estructura

```
DOMAIN/          Entidades del negocio
APLICATION/      DTOs, interfaces, servicios
INFRASTRUCTURE/  Repositorios ADO.NET y conexion a SQL Server
PRESENTATION/    API + vistas MVC (proyecto de arranque)
Frontend/        React + Vite
database/        Scripts de esquema, procedimientos y datos de ejemplo
scripts/         Utilidades de base de datos y respaldos
docs/            Documentacion de infraestructura y del equipo
```

## Levantar todo con Docker (recomendado para QA)

Requisitos: Docker Desktop.

```powershell
copy .env.example .env
docker compose up --build
```

- Frontend: http://localhost:8080
- API: http://localhost:8081
- Health check: http://localhost:8081/health
- SQL Server: `localhost,14330` (usuario `sa`, contrasena en `.env`)

Para borrar la base y empezar de cero: `docker compose down -v`.

## Levantar en local (sin Docker)

Requisitos: .NET 10 SDK, Node 24 y SQL Server.

```powershell
# 1. Crear/cargar la base (ajusta -Server si tu instancia tiene otro nombre)
.\scripts\init-db.ps1 -Server ".\SQLEXPRESS" -WindowsAuth

# 2. API (usa la cadena de appsettings.json)
dotnet run --project PRESENTATION\PRESENTATION.csproj

# 3. Frontend, en otra terminal
cd Frontend
copy .env.example .env
npm install
npm run dev
```

Frontend: http://localhost:5173

> Si tu instancia de SQL Server no se llama `.\SQLEXPRESS`, cambia la cadena
> `ConnectionStrings:default` en `PRESENTATION/appsettings.json` o define la
> variable `ConnectionStrings__default`.

> Guia detallada para instalar en otra PC:
> [GUIA-ABRIR-EN-OTRA-PC.md](GUIA-ABRIR-EN-OTRA-PC.md)

## APK Android (misma Wi-Fi)

El APK ya compilado esta en `release\SaludAnimal.apk` y tambien en el
Escritorio. Es una app Android que muestra la misma interfaz React y consume la
misma API; no guarda una base de datos aparte.

- Servidor inicial compilado: `http://10.38.63.212:5000/api`.
- Si la IP de la PC cambia, dentro de la app se abre el boton **Servidor** y se
  escribe la nueva direccion. No hace falta recompilar.
- La API debe arrancar escuchando en toda la red, no solo en localhost:

  ```powershell
  $env:ASPNETCORE_URLS = "http://0.0.0.0:5000"
  dotnet run --project PRESENTATION\PRESENTATION.csproj --no-launch-profile
  ```

- Permitir TCP 5000 en el firewall de Windows para la red privada.
- El telefono y la PC deben estar en la misma Wi-Fi.

Guia completa de instalacion, firewall, selector de servidor, compilacion y
problemas comunes:
[docs/EQUIPO/APK-ANDROID.md](docs/EQUIPO/APK-ANDROID.md).

## Verificar que todo responde

```powershell
.\scripts\smoke-test.ps1 -BaseUrl "http://localhost:8081"
```

## Documentacion

- [Infraestructura y despliegue](docs/INFRAESTRUCTURA.md)
- [Base de datos](database/README.md)
- [APK Android](docs/EQUIPO/APK-ANDROID.md)
- [Avance de Yeskel y acuerdos del equipo](docs/EQUIPO/AVANCE-YESKEL.md)

## Estado

En desarrollo. La API expone endpoints REST bajo `/api/*` y el frontend consume
`VITE_API_URL`. El health check `/health` valida tambien la conexion a la base.

