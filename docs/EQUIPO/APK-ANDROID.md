# APK Android - Salud Animal

Aplicacion Android para usar el sistema desde un telefono en la **misma red
Wi-Fi** que la computadora donde corren la API y SQL Server.

El APK es una envoltura (WebView) de la misma interfaz React. **No guarda una
base de datos aparte**: todo lo que se registra en el telefono viaja por la API
a la base `Veterinaria_SaludAnimal` de la PC.

```text
Telefono Android
  APK Salud Animal (React dentro de WebView)
        |
        |  Wi-Fi de la clinica
        |  HTTP http://IP-DE-LA-PC:5000/api
        v
  API ASP.NET Core (.NET 10)
        |
        |  ADO.NET
        v
  SQL Server: Veterinaria_SaludAnimal
```

> Esta version usa **HTTP sin TLS** porque es una demostracion en red local.
> Antes de publicar en internet se debe usar HTTPS y una API con certificado.
> Consulta la seccion de seguridad al final.

---

## 1. Datos de esta entrega

| Dato | Valor |
| --- | --- |
| APK en el Escritorio | `C:\Users\yeske\Desktop\SaludAnimal.apk` |
| APK dentro del proyecto | `release\SaludAnimal.apk` |
| Tamano | 5.735.793 bytes (aprox. 5,47 MB) |
| SHA256 | `6E3D05431983C505731BCC15D6900E5D0F4F4361AA6F5837B86145C424AF80DE` |
| Paquete Android | `com.saludanimal.app` |
| Version | 1.0 (versionCode 1) |
| Android minimo | 7.0 (API 24) |
| Android objetivo | API 36 |
| Servidor inicial compilado | `http://192.168.80.119:5000/api` |

La IP `192.168.80.119` es la que tenia la PC de desarrollo al compilar. Si en
tu red cambia, no hace falta recompilar: se cambia desde el boton **Servidor**
de la app (seccion 5).

> El APK es **debug**. Sirve para instalar por WhatsApp, USB o Drive y probar el
> sistema, pero no esta firmado para Google Play. Para produccion hay que crear
> una version release firmada.

---

## 2. Requisitos

- La PC con **SQL Server** y la **API** encendidos.
- Telefono y PC conectados a la **misma red Wi-Fi** (no una red de invitados).
- El puerto **TCP 5000** permitido en el firewall de Windows.
- La API escuchando en `0.0.0.0:5000`, no solo en `localhost`.
- Android 7.0 o superior.
- El APK instalado con permiso para "instalar apps desconocidas".

No se necesita internet ni Docker para usar el APK. Tampoco se necesita abrir
SQL Server a la red: el telefono solo habla con la API.

---

## 3. Preparar la PC

### 3.1. Iniciar SQL Server

Verificar que el servicio este corriendo (PowerShell como administrador si hace
falta):

```powershell
Get-Service MSSQLSERVER
# Si usas SQL Server Express:
Get-Service 'MSSQL$SQLEXPRESS'
```

Si esta detenido:

```powershell
Start-Service MSSQLSERVER
```

### 3.2. Arrancar la API para la red local

En una terminal, desde la carpeta del proyecto:

```powershell
$env:ConnectionStrings__default = "Server=.;Database=Veterinaria_SaludAnimal;Integrated Security=True;TrustServerCertificate=True"
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://0.0.0.0:5000"
dotnet run --project PRESENTATION\PRESENTATION.csproj --no-launch-profile
```

Cambia `Server=.` por tu instancia si es `.\SQLEXPRESS` o similar.

Debe aparecer algo parecido a:

```text
Now listening on: http://0.0.0.0:5000
```

En la PC, la API sigue respondiendo en `http://localhost:5000`. El `0.0.0.0`
permite ademas que el telefono entre por la IP de la PC.

Si Windows pregunta si permitir `dotnet` en la red, marca **Red privada**.
Dejar esta terminal abierta mientras se usa el telefono.

### 3.3. Obtener la IP de la PC

```powershell
ipconfig
```

Buscar la seccion de la conexion **Wi-Fi** y anotar la **Direccion IPv4**.
Ejemplo de la maquina de desarrollo:

```text
192.168.80.119
```

Con esa IP se forma la direccion de la API:

```text
http://192.168.80.119:5000/api
```

> En el telefono, `localhost` significa "el telefono mismo", no la PC. Por eso
> hay que usar la IP de la computadora.

### 3.4. Permitir el puerto 5000 en Windows

La red de la PC de desarrollo quedo marcada como **Publica**. En ese perfil
Windows bloquea las conexiones entrantes por defecto, asi que el telefono puede
no conectarse aunque la API este encendida.

Abrir **PowerShell como administrador** y ejecutar:

```powershell
New-NetFirewallRule `
  -DisplayName "Salud Animal API 5000" `
  -Direction Inbound `
  -Protocol TCP `
  -LocalPort 5000 `
  -Action Allow `
  -Profile Private
```

Si la red esta marcada como Publica y no quieres cambiarla, se puede usar
`-Profile Public`, pero es menos seguro. Lo recomendable en una red de confianza
es cambiarla a Privada:

```powershell
Set-NetConnectionProfile -Name "SANDOVAL" -NetworkCategory Private
```

Cambia `SANDOVAL` por el nombre de tu red. Esto tambien requiere PowerShell como
administrador. No lo hagas en una red publica o desconocida.

Para comprobar si la regla ya existe:

```powershell
Get-NetFirewallRule -DisplayName "Salud Animal API 5000"
```

Si no puedes ejecutar comandos como administrador, pide a quien administre la PC
que cree la regla. Sin ese paso, el telefono no podra entrar al puerto 5000.

### 3.5. Probar la API desde la PC

```powershell
Invoke-RestMethod http://192.168.80.119:5000/health
```

Debe responder algo parecido a:

```json
{
  "status": "ok",
  "database": "ok",
  "environment": "Development"
}
```

Si `database` no es `ok`, la API esta viva pero la cadena de conexion o SQL
Server tienen un problema.

---

## 4. Instalar el APK en el telefono

1. Copiar `SaludAnimal.apk` al telefono por WhatsApp, Google Drive, USB o
   cualquier otro medio.
2. Abrir el archivo desde el telefono.
3. Android puede pedir permitir "Instalar apps desconocidas" para la app de
   archivos o el navegador. Aceptar solo para esa instalacion.
4. Instalar y abrir **Salud Animal**.
5. Si Android muestra un aviso de que es una app de desarrollo, es normal: el
   APK es debug y no viene de Google Play.

Verificar que el archivo no se haya corrompido (opcional, en la PC):

```powershell
Get-FileHash C:\Users\yeske\Desktop\SaludAnimal.apk -Algorithm SHA256
```

Debe coincidir con:

```text
6E3D05431983C505731BCC15D6900E5D0F4F4361AA6F5837B86145C424AF80DE
```

---

## 5. Configurar el servidor dentro de la app

La primera vez que se abre el APK, si no hay un servidor guardado, aparece el
modal **Servidor de la clinica**.

1. En el campo **Direccion de la API** escribir:

   ```text
   http://192.168.80.119:5000/api
   ```

   Usar la IP real de la PC. Se puede escribir tambien sin `http://` y sin
   `/api`; la app lo completa, pero la forma completa es la mas clara.

2. Tocar **Probar conexion**.
   - Correcto: "Conexion correcta. La API y la base de datos responden."
   - Error: revisar la seccion 8.

3. Tocar **Guardar y recargar**. La app guarda la direccion en el telefono y
   recarga la sesion.

4. Si el modal no aparece, tocar el boton flotante **Servidor** (icono de
   engranaje) en la esquina inferior.

5. Si mas adelante cambia la IP de la PC, abrir **Servidor**, escribir la nueva
   direccion, probar y guardar. No hace falta reinstalar el APK.

6. **Restablecer** vuelve a la direccion que quedo compilada en el APK y borra
   la direccion guardada.

La app normaliza la direccion:

- Agrega `http://` si falta.
- Agrega `/api` si falta.
- Quita una barra final.
- Si se pega la URL de `/health`, la convierte a la base de la API.

La prueba de conexion consulta `http://IP:5000/health` y tambien revisa que el
campo `database` sea `ok`.

---

## 6. Usar el sistema y verificar que los datos llegan a SQL Server

1. En el APK, entrar al sitio publico o al acceso interno.
2. Usuarios de ejemplo:

   | Rol | Usuario | Contrasena |
   | --- | --- | --- |
   | Administrador | `admin` | `Admin123!` |
   | Recepcion | `recepcion` | `Recepc123!` |
   | Veterinario | `veterinario` | `Vet123!` |

3. Crear un cliente, una mascota o una cita desde el telefono.
4. Abrir el sistema en la PC, o SSMS, y comprobar el registro.

```sql
USE Veterinaria_SaludAnimal;

SELECT TOP 10
    ID_CLIENTE,
    NOMBRE_COMPLETO,
    TELEFONO,
    ESTADO
FROM dbo.CLIENTE
ORDER BY ID_CLIENTE DESC;
```

Si el registro aparece en la PC, el telefono esta usando la misma base. La app
no crea una base local ni sincroniza en segundo plano.

---

## 7. Compilar el APK de nuevo

Solo es necesario si cambia el codigo, los iconos o la URL inicial.

### 7.1. Forma recomendada: script

Requisitos: Node 24, JDK 21 y Android SDK. En Windows:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\scripts\build-apk.ps1 -ServerUrl "http://192.168.80.119:5000/api"
```

El script:

1. Detecta el JDK y el Android SDK (o usa `JAVA_HOME` y `ANDROID_HOME`).
2. Normaliza la URL y la compila como `VITE_API_URL`.
3. Ejecuta `npm ci` (o `npm install` si hace falta).
4. Compila React con `npm run build`.
5. Sincroniza Capacitor con `npx cap sync android`.
6. Compila el APK debug con Gradle.
7. Copia el resultado al Escritorio como `SaludAnimal.apk`.
8. Imprime el tamano y el SHA256.

Opciones utiles:

```powershell
# Usar una salida distinta
.\scripts\build-apk.ps1 -ServerUrl "http://192.168.80.119:5000/api" -OutputApk "C:\Temp\SaludAnimal.apk"

# Reutilizar node_modules sin ejecutar npm ci
.\scripts\build-apk.ps1 -ServerUrl "http://192.168.80.119:5000/api" -SkipInstall
```

### 7.2. Forma manual

```powershell
cd Frontend

$env:JAVA_HOME = "C:\ruta\al\jdk-21"
$env:ANDROID_HOME = "C:\ruta\al\Android\sdk"
$env:ANDROID_SDK_ROOT = $env:ANDROID_HOME
$env:VITE_API_URL = "http://192.168.80.119:5000/api"

npm ci
npm run build
npx cap sync android

cd android
.\gradlew.bat assembleDebug
```

El APK queda en:

```text
Frontend\android\app\build\outputs\apk\debug\app-debug.apk
```

### 7.3. Instalar el SDK de Android

Si no esta instalado, usar los command-line tools oficiales:

```powershell
sdkmanager "platform-tools" "platforms;android-36" "build-tools;36.0.0"
sdkmanager --licenses
```

Luego crear `Frontend\android\local.properties`:

```properties
sdk.dir=C:/ruta/al/Android/sdk
```

`local.properties` es especifico de cada PC y esta ignorado por git. El script
lo crea automaticamente.

`Frontend\android\gradlew.bat` descarga Gradle 8.14.3 la primera vez; necesita
internet solo en esa primera compilacion.

### 7.4. Iconos y splash

Las imagenes fuente estan en `Frontend\assets\` y se pueden regenerar con:

```powershell
.\scripts\make-android-assets.ps1
cd Frontend
npm run assets:android
```

Para generar de nuevo el APK despues de cambiar iconos o splash:

```powershell
cd Frontend
npm run build:android
npm run apk:debug
```

---

## 8. Problemas comunes

| Sintoma | Causa probable | Solucion |
| --- | --- | --- |
| "No se pudo conectar" en la app | API apagada, IP equivocada o firewall | Verificar `/health` desde el navegador del telefono |
| La PC responde en `localhost` pero el telefono no | API escuchando solo en localhost o puerto bloqueado | Usar `ASPNETCORE_URLS=http://0.0.0.0:5000` y la regla de firewall |
| El telefono abre Google pero no la API | Red de invitados o aislamiento de clientes | Usar la misma Wi-Fi principal; desactivar aislamiento en el router |
| La IP de la PC cambio | DHCP asigno otra IP | Abrir **Servidor**, escribir la nueva y guardar; o reservar IP en el router |
| "La API responde, pero la base de datos no esta OK" | Cadena de conexion o SQL Server | Revisar `ConnectionStrings__default` y el servicio de SQL |
| Error de CORS en el APK | Origen `http://localhost` no permitido | En `appsettings.json`, agregar `http://localhost` a `Cors:AllowedOrigins` |
| `ERR_CLEARTEXT_NOT_PERMITTED` | APK compilado sin cleartext | Usar el APK entregado o mantener `usesCleartextTraffic="true"` en debug |
| Android no deja instalar | Origen desconocido deshabilitado o archivo incompleto | Permitir la instalacion y verificar el SHA256 |
| La app abre en blanco | No pudo cargar el WebView o la sesion quedo corrupta | Revisar la direccion en **Servidor**; cerrar y abrir la app |
| La sesion no entra | Token viejo o usuario cambiado | Cerrar sesion y volver a entrar; cambiar el servidor limpia el token |

Prueba de CORS desde la PC (util para diagnosticar el WebView):

```powershell
curl.exe -i -X OPTIONS `
  -H "Origin: http://localhost" `
  -H "Access-Control-Request-Method: GET" `
  -H "Access-Control-Request-Headers: authorization,content-type" `
  http://192.168.80.119:5000/api/Cliente
```

Debe devolver `204` y `Access-Control-Allow-Origin: http://localhost`.

---

## 9. Seguridad y limites de esta version

- **HTTP sin TLS** solo para la red local de demostracion. En produccion usar
  HTTPS y quitar `usesCleartextTraffic`.
- El APK **no esta firmado para produccion**. Es un build debug.
- Cambiar `Jwt:Key` en `PRESENTATION/appsettings.json` y las contrasenas de los
  usuarios de ejemplo antes de publicar.
- No abrir el puerto de SQL Server (1433) a la red ni a internet. El telefono
  solo necesita el puerto 5000 de la API.
- Mantener CORS limitado a los origenes necesarios. El APK usa
  `http://localhost` como origen del WebView.
- No hay modo offline, cola de sincronizacion, iPhone ni integracion en la nube
  en esta version.
- Si se pierde el telefono, cambiar la contrasena del usuario y la clave JWT
  invalida los tokens emitidos.
- Las copias `.bak`/`.bacpac` contienen datos reales: no compartirlas.

Checklist de seguridad antes de una entrega real:

- [ ] API con HTTPS y certificado valido
- [ ] APK release firmado con keystore propio
- [ ] `usesCleartextTraffic` deshabilitado
- [ ] `Jwt:Key` y contrasenas de ejemplo cambiadas
- [ ] CORS restringido
- [ ] Firewall solo en red privada
- [ ] Respaldos cifrados/protegidos
- [ ] SQL Server no expuesto a la red

---

## 10. Checklist de prueba en la clinica

- [ ] SQL Server corriendo
- [ ] API con `/health` en `ok` desde la PC
- [ ] API escuchando en `0.0.0.0:5000`
- [ ] Regla de firewall para TCP 5000 creada
- [ ] Telefono en la misma Wi-Fi
- [ ] `/health` abre desde el navegador del telefono
- [ ] APK instalado
- [ ] Boton **Servidor** con la IP correcta
- [ ] "Probar conexion" responde correctamente
- [ ] Login `admin` funciona
- [ ] Se crea un cliente/cita desde el telefono
- [ ] El cliente/cita aparece en la PC y en SQL Server
- [ ] Se cierra y abre la app y la URL guardada sigue funcionando

---

## 11. Validacion realizada en esta entrega

- APK compilado con Gradle y `assembleDebug`.
- Manifiesto verificado con `aapt2`:
  - `android.permission.INTERNET`
  - `android:usesCleartextTraffic="true"`
  - `minSdkVersion 24`, `targetSdkVersion 36`
- Assets web dentro del APK verificados: incluyen el selector de servidor y la
  IP inicial `192.168.80.119`.
- API real probada en la PC:
  - `/health` responde `200` con `database: ok`.
  - CORS preflight para `Origin: http://localhost` responde `204` y permite
    `authorization` y `content-type`.
  - Login `admin` y consulta de clientes responden `200`.
- **Pendiente:** instalar y probar en un telefono fisico. En la maquina de
  desarrollo no habia un dispositivo conectado por ADB y la red estaba marcada
  como Publica, sin regla de firewall para TCP 5000. La guia de la seccion 3.4
  cubre ese paso.

---

## 12. Referencias

- [Guia para abrir el proyecto en otra PC](../../GUIA-ABRIR-EN-OTRA-PC.md)
- [Infraestructura y despliegue](../INFRAESTRUCTURA.md)
- [README del proyecto](../../README.md)
- [Frontend / Android](../../Frontend/README.md)
