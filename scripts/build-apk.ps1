# Compila el APK debug de Salud Animal y lo copia al Escritorio.
# Uso:
#   .\scripts\build-apk.ps1 -ServerUrl "http://192.168.1.50:5000/api"
# Requiere Node 24, JDK 21 y Android SDK (platforms;android-36 + build-tools;36.0.0).
param(
  [string]$ServerUrl = "http://10.38.63.212:5000/api",
  [string]$OutputApk = "",
  [string]$JavaHome = $env:JAVA_HOME,
  [string]$AndroidSdk = $env:ANDROID_HOME,
  [switch]$SkipInstall
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$frontend = Join-Path $root "Frontend"
$android = Join-Path $frontend "android"

if (-not (Test-Path (Join-Path $android "gradlew.bat"))) {
  throw "No existe Frontend\android. Ejecuta primero: cd Frontend; npx cap add android"
}
if (-not $OutputApk) {
  $OutputApk = Join-Path ([Environment]::GetFolderPath("Desktop")) "SaludAnimal.apk"
}

# Detectar JDK 21 si no vino por variable de entorno.
if (-not $JavaHome) {
  $jdkRoot = Join-Path $env:LOCALAPPDATA "Android\jdk"
  if (Test-Path $jdkRoot) {
    $jdk = Get-ChildItem $jdkRoot -Directory |
      Sort-Object Name -Descending |
      Select-Object -First 1
    if ($jdk -and (Test-Path (Join-Path $jdk.FullName "bin\java.exe"))) {
      $JavaHome = $jdk.FullName
    }
  }
}
if (-not $JavaHome) {
  $studioJdk = "C:\Program Files\Android\Android Studio\jbr"
  if (Test-Path (Join-Path $studioJdk "bin\java.exe")) { $JavaHome = $studioJdk }
}
if (-not $JavaHome -or -not (Test-Path (Join-Path $JavaHome "bin\java.exe"))) {
  throw "No encontre JDK 21. Instalalo o define JAVA_HOME antes de ejecutar este script."
}

# Detectar Android SDK si no vino por variable de entorno.
if (-not $AndroidSdk) {
  $sdkCandidates = @(
    $env:ANDROID_HOME,
    $env:ANDROID_SDK_ROOT,
    (Join-Path $env:LOCALAPPDATA "Android\sdk"),
    "C:\Android\sdk"
  )
  foreach ($candidate in $sdkCandidates) {
    if ($candidate -and (Test-Path $candidate)) { $AndroidSdk = $candidate; break }
  }
}
if (-not $AndroidSdk -or -not (Test-Path $AndroidSdk)) {
  throw "No encontre Android SDK. Define ANDROID_HOME o instala el SDK."
}

$platform = Join-Path $AndroidSdk "platforms\android-36"
$buildTools = Join-Path $AndroidSdk "build-tools\36.0.0"
if (-not (Test-Path $platform) -or -not (Test-Path $buildTools)) {
  throw "Faltan paquetes del SDK. Instala: sdkmanager `"platform-tools`" `"platforms;android-36`" `"build-tools;36.0.0`""
}

$env:JAVA_HOME = $JavaHome
$env:ANDROID_HOME = $AndroidSdk
$env:ANDROID_SDK_ROOT = $AndroidSdk
$env:PATH = "$JavaHome\bin;$AndroidSdk\cmdline-tools\latest\bin;$AndroidSdk\platform-tools;$env:PATH"

# Normalizar la URL que quedara compilada como valor inicial del APK.
$ServerUrl = $ServerUrl.Trim()
$ServerUrl = $ServerUrl -replace '/health$', ''
$ServerUrl = $ServerUrl.TrimEnd('/')
if ($ServerUrl -notmatch '^https?://') { $ServerUrl = "http://$ServerUrl" }
if ($ServerUrl -notmatch '/api$') { $ServerUrl = "$ServerUrl/api" }
$env:VITE_API_URL = $ServerUrl

# local.properties apunta al SDK de esta PC (ignorado por git).
Set-Content -Path (Join-Path $android "local.properties") -Value ("sdk.dir=" + ($AndroidSdk -replace '\\', '/')) -Encoding ASCII

Push-Location $frontend
try {
  if (-not $SkipInstall) {
    Write-Host "Instalando dependencias del frontend..."
    & npm ci
    if ($LASTEXITCODE -ne 0) {
      Write-Warning "npm ci fallo; probando npm install..."
      & npm install
      if ($LASTEXITCODE -ne 0) { throw "No se pudieron instalar las dependencias." }
    }
  }

  Write-Host "Compilando frontend con servidor inicial: $ServerUrl"
  & npm run build
  if ($LASTEXITCODE -ne 0) { throw "Fallo npm run build." }

  Write-Host "Sincronizando Capacitor..."
  & npx cap sync android
  if ($LASTEXITCODE -ne 0) { throw "Fallo npx cap sync android." }

  Write-Host "Compilando APK debug (la primera vez descarga Gradle)..."
  Push-Location $android
  try {
    & .\gradlew.bat assembleDebug --no-daemon
    if ($LASTEXITCODE -ne 0) { throw "Fallo Gradle assembleDebug." }
  } finally {
    Pop-Location
  }

  $builtApk = Join-Path $android "app\build\outputs\apk\debug\app-debug.apk"
  if (-not (Test-Path $builtApk)) { throw "Gradle no genero el APK esperado." }

  $outputDir = Split-Path -Parent $OutputApk
  if ($outputDir) { New-Item -ItemType Directory -Force -Path $outputDir | Out-Null }
  Copy-Item -Path $builtApk -Destination $OutputApk -Force
  $hash = (Get-FileHash $OutputApk -Algorithm SHA256).Hash

  Write-Host ""
  Write-Host "APK listo: $OutputApk"
  Write-Host "Bytes:    $((Get-Item $OutputApk).Length)"
  Write-Host "SHA256:   $hash"
  Write-Host "Servidor: $ServerUrl"
} finally {
  Pop-Location
}
