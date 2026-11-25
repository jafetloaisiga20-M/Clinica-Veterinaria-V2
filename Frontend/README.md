# Frontend - Clinica Veterinaria

React + Vite. Consume la API ASP.NET Core.

## Desarrollo

```powershell
copy .env.example .env
npm install
npm run dev
```

Abre http://localhost:5173.

La URL de la API se configura en `.env`:

```
VITE_API_URL=http://localhost:5000/api
```

No hardcodear IPs ni puertos en `App.jsx`; usar siempre `import.meta.env`.

## Compilar

```powershell
npm run build
```

Genera `dist/`, que es lo que se publica.

## APK Android

La app Android esta hecha con **Capacitor**: el APK abre el mismo build de React
en un WebView y consume la API. La URL de la API se compila como valor inicial,
pero dentro de la app se puede cambiar con el boton **Servidor** (se guarda en
`localStorage`).

Compilacion manual desde esta carpeta:

```powershell
$env:VITE_API_URL = "http://192.168.80.119:5000/api"
npm ci
npm run build:android
npm run apk:debug
```

El APK queda en:

```text
android\app\build\outputs\apk\debug\app-debug.apk
```

Desde la raiz del repositorio tambien esta el script completo, que copia el APK
al Escritorio:

```powershell
.\scripts\build-apk.ps1 -ServerUrl "http://192.168.80.119:5000/api"
```

Las imagenes fuente del icono y splash estan en `assets/` y se pueden regenerar
con `..\scripts\make-android-assets.ps1` y `npm run assets:android`.

Guia completa (firewall, instalar en el telefono, problemas):
[../docs/EQUIPO/APK-ANDROID.md](../docs/EQUIPO/APK-ANDROID.md).

## Docker

```powershell
docker build -t clinica-frontend .
```

El contenedor usa nginx y redirige `/api/*` al servicio `api` (definido en
`docker-compose.yml`).

## Despliegue en Netlify

El repositorio incluye `netlify.toml` en la raiz. Solo hay que:

1. Importar el repositorio en Netlify.
2. Definir la variable `VITE_API_URL` con la URL publica de la API
   (ejemplo: `https://tu-api.azurewebsites.net/api`).
3. Confirmar que esa URL este en `Cors__AllowedOrigins` de la API.
